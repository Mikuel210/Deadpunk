using System.Collections.Generic;
using Helpers;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class BuildingSystem : Singleton<BuildingSystem>
{
    public Grid<Building> Grid { get; private set; }

    [SerializeField] private BuildingSO testBuilding;

    [Space, SerializeField] private Material visualBlueMaterial;
    [SerializeField] private Material visualRedMaterial;
    
    public bool PlacingBuilding { get; private set; }
    private BuildingSO _currentBuildingSO;
    private BuildingSO.Direction _currentDirection = BuildingSO.Direction.Down;
    private GameObject _buildingVisual;
    
    private bool _demolishing;
    
    void Awake()
    {
        int size = 256;
        float cellSize = 5f;
        
        float origin = -size * cellSize * 0.5f;
        
        Grid = new(
            size, 
            size, 
            cellSize, 
            new(origin, 0, origin)
        );
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            StopBuilding();
        
        UpdateBuildingRotation();
        UpdateBuildingPlacement();
        UpdateBuildingVisual();
        UpdateDemolishing();
    }

    public void StartBuilding(BuildingSO building)
    {
        StopBuilding();
        StopDemolishing();
        
        PlacingBuilding = true;
        _currentBuildingSO = building;
        _buildingVisual = Instantiate(building.visual);
    }

    public void StopBuilding()
    {
        PlacingBuilding = false;
        _currentBuildingSO = null;
        
        Destroy(_buildingVisual);
        _buildingVisual = null;
    }


    public void StartDemolishing() {
        _demolishing = true;
        StopBuilding();
    }

    public void StopDemolishing() {
        _demolishing = false;
    }


    private GameObject _previousBuildingMesh;
    
    private void UpdateDemolishing() {
        if (!_demolishing) return;
        
        // Stop demolishing
        if (Input.GetMouseButtonDown(1)) {
            if (_previousBuildingMesh)
                ChangeMeshColor(_previousBuildingMesh, Color.white);
            
            UIManager.Instance.StopDemolishing();
            StopDemolishing();
            
            return;
        }
        
        // Get building under mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GameObject buildingUnderMouse = null;
        GameObject buildingMesh = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 10_000f)) {
            if (hit.collider.CompareTag("Building"))
                buildingUnderMouse = hit.collider.gameObject;
        }
        
        buildingMesh = buildingUnderMouse?.transform.Find("Mesh").gameObject;

        // Update colors
        if (buildingMesh)
            ChangeMeshColor(buildingMesh, Color.red);
        
        if (_previousBuildingMesh && _previousBuildingMesh != buildingMesh)
            ChangeMeshColor(_previousBuildingMesh, Color.white);
        
        _previousBuildingMesh = buildingMesh;
        
        // Delete
        if (!Input.GetMouseButtonDown(0)) return;
        
        Building building = buildingUnderMouse?.GetComponent<Building>();
        building?.Demolish();
    }

    private void ChangeMeshColor(GameObject mesh, Color color) {
        List<MeshRenderer> renderers = mesh.GetComponentsInChildren<MeshRenderer>().ToList();
        
        if (mesh.TryGetComponent<MeshRenderer>(out var meshRenderer))
            renderers.Add(meshRenderer);

        foreach (MeshRenderer renderer in renderers)
            renderer.material.SetColor("_SecondColor", color);
    }
    
    private void UpdateBuildingRotation()
    {
        if (!PlacingBuilding) return;
        
        if (Input.GetKeyDown(KeyCode.Q))
            _currentDirection = BuildingSO.GetPreviousDirection(_currentDirection);
        else if (Input.GetKeyDown(KeyCode.E))
            _currentDirection = BuildingSO.GetNextDirection(_currentDirection);
    }

    private bool CanPlaceInGrid<T>(Grid<T> grid, Vector2Int gridPosition)
    {
        List<Vector2Int> gridPositions = _currentBuildingSO.GetGridPositions(gridPosition, _currentDirection);
        List<Vector3> worldPositions = gridPositions.Select(e => Grid.GetWorldPosition(e)).ToList();

        bool canBuild = true;

        foreach (Vector3 position in worldPositions)
        {
            if (!grid.GetGridObject(position).CanBuild())
            {
                canBuild = false;
                break;
            }
        }
        
        return canBuild;
    }

    private bool IsInProximityRange(Vector2Int gridPosition)
    {
        if (_currentBuildingSO.proximityResource == null) return true;
        
        bool output = false;
        
        List<Vector2Int> gridPositions = _currentBuildingSO.GetGridPositions(gridPosition, _currentDirection);
        List<Vector3> worldPositions = gridPositions.Select(e => Grid.GetWorldPosition(e)).ToList();
        
        List<Vector2Int> proximityGridPositions =
            worldPositions.Select(e => WorldManager.Instance.ProximityGrid.GetGridPosition(e)).ToList();

        foreach (Vector2Int proximityGridPosition in proximityGridPositions)
        {
            GridObject<Resource> gridObject = WorldManager.Instance.ProximityGrid.GetGridObject(proximityGridPosition);
            Resource gridObjectValue = gridObject.GetValue();

            if (gridObjectValue == null) continue;
            if (gridObjectValue.GetResourceSO() != _currentBuildingSO.proximityResource) continue;

            output = true;
            break;
        }

        return output;
    }

    private bool CanAfford()
    {
        bool canAfford = true;
        
        foreach (ResourceAmountPair resourceAmountPair in _currentBuildingSO.cost)
        {
            ResourceSO resource = resourceAmountPair.resource;
            int amount = resourceAmountPair.amount;
            
            if (resource.name == "Wood")
                canAfford = GameManager.Instance.Wood.Value >= amount;
            else if (resource.name == "Stone")
                canAfford = GameManager.Instance.Stone.Value >= amount;
            else if (resource.name == "Metal")
                canAfford = GameManager.Instance.Metal.Value >= amount;

            if (!canAfford) break;
        }
        
        return canAfford;
    }

    private bool CanBuildNormal(Vector2Int gridPosition) {
        bool canBuild = CanPlaceInGrid(Grid, gridPosition) && CanPlaceInGrid(WorldManager.Instance.Grid, gridPosition);
        bool isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        bool isInProximityRange = IsInProximityRange(gridPosition);
        bool canAfford = CanAfford();
            
        return canBuild && isInProximityRange && canAfford && !isMouseOverUI;
    }

    private bool CanBuildTurret(Vector2Int gridPosition) {
        bool canBuild = true;

        foreach (Vector2Int position in _currentBuildingSO.GetGridPositions(gridPosition, _currentDirection)) {
            GridObject<Building> gridObject = Grid.GetGridObject(position);
            BuildingSO buildingSO = gridObject.GetValue()?.BuildingSO;
            
            if (buildingSO && buildingSO.isSupport && !gridObject.GetValue2()) continue;

            canBuild = false;
            break;
        }
        
        bool isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        bool canAfford = CanAfford();
            
        return canBuild && canAfford && !isMouseOverUI;
    }
    
    private bool CanBuild(Vector2Int gridPosition) {
        if (!_currentBuildingSO.isTurret)
            return CanBuildNormal(gridPosition);
        
        return CanBuildTurret(gridPosition);
    }
        
    
    private void UpdateBuildingPlacement()
    {
        if (!PlacingBuilding) return;
        if (!Input.GetMouseButton(0)) return;

        Vector3 mousePosition = InputManager.GetMousePosition();
        Vector2Int gridPosition = Grid.GetGridPosition(mousePosition);

        bool canBuild = CanBuild(gridPosition);
        if (!canBuild) return;
        
        // Subtract resources
        int woodCost = _currentBuildingSO.cost.FirstOrDefault(e => e.resource.name == "Wood")?.amount ?? 0;
        int stoneCost = _currentBuildingSO.cost.FirstOrDefault(e => e.resource.name == "Stone")?.amount ?? 0;
        int metalCost = _currentBuildingSO.cost.FirstOrDefault(e => e.resource.name == "Metal")?.amount ?? 0;
        
        GameManager.Instance.Wood.Value -= woodCost;
        GameManager.Instance.Stone.Value -= stoneCost;
        GameManager.Instance.Metal.Value -= metalCost;

        // Place building
        Build(gridPosition, _currentBuildingSO, _currentDirection);
    }

    public void Build(Vector2Int gridPosition, BuildingSO buildingSO, BuildingSO.Direction direction) {
        Vector3 worldPosition = Grid.GetWorldPosition(gridPosition);
        Building building = Building.Create(worldPosition, buildingSO, gridPosition, direction);
        
        List<Vector2Int> gridPositions = buildingSO.GetGridPositions(gridPosition, direction);

        foreach (Vector2Int position in gridPositions) {
            if (buildingSO.isTurret)
                Grid.GetGridObject(position).SetValue2(building);
            else
                Grid.GetGridObject(position).SetValue(building);
        }
    }

    private void UpdateBuildingVisual()
    {
        if (_buildingVisual == null) return;
        
        Vector3 mousePosition = InputManager.GetMousePosition();
        Vector2Int gridPosition = Grid.GetGridPosition(mousePosition);
        Vector3 worldPosition = Grid.GetWorldPosition(gridPosition);
        
        Vector2Int rotationOffset = _currentBuildingSO.GetRotationOffset(_currentDirection);
        Vector3 gameObjectPosition = worldPosition + new Vector3(rotationOffset.x, 0, rotationOffset.y) * Grid.GetCellSize();
        Quaternion gameObjectRotation = Quaternion.Euler(0, BuildingSO.GetRotationAngle(_currentDirection), 0);
        
        _buildingVisual.transform.position = gameObjectPosition;
        _buildingVisual.transform.rotation = gameObjectRotation;
        
        bool canPlace = CanBuild(gridPosition);

        List<MeshRenderer> renderers = _buildingVisual.GetComponentsInChildren<MeshRenderer>().ToList();
        Material material = canPlace ? visualBlueMaterial : visualRedMaterial;

        foreach (MeshRenderer renderer in renderers) 
            renderer.SetMaterials(Enumerable.Repeat(material, renderer.materials.Length).ToList());
    }
}
