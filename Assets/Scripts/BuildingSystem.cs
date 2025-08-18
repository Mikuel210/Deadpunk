using System.Collections.Generic;
using Helpers;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class BuildingSystem : Singleton<BuildingSystem>
{
    public Grid<Building> Grid { get; private set; }

    [SerializeField] private BuildingSO testBuilding;

    [Space, SerializeField] private Material visualBlueMaterial;
    [SerializeField] private Material visualRedMaterial;
    
    private bool _placingBuilding;
    private BuildingSO _currentBuildingSO;
    private BuildingSO.Direction _currentDirection = BuildingSO.Direction.Down;
    private GameObject _buildingVisual;
    
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
    }

    public void StartBuilding(BuildingSO building)
    {
        StopBuilding();
        
        _placingBuilding = true;
        _currentBuildingSO = building;
        _buildingVisual = Instantiate(building.visual);
    }

    public void StopBuilding()
    {
        _placingBuilding = false;
        _currentBuildingSO = null;
        
        Destroy(_buildingVisual);
        _buildingVisual = null;
    }

    private void UpdateBuildingRotation()
    {
        if (!_placingBuilding) return;
        
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
            if (gridObject.CanBuild()) continue;

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

            if (!canAfford) break;
        }
        
        return canAfford;
    }
    
    private bool CanBuild(Vector2Int gridPosition)
    {
        bool canBuild = CanPlaceInGrid(Grid, gridPosition) && CanPlaceInGrid(WorldManager.Instance.Grid, gridPosition);
        bool isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        bool isInProximityRange = IsInProximityRange(gridPosition);
        bool canAfford = CanAfford();
            
        return canBuild && isInProximityRange && canAfford && !isMouseOverUI;
    }
        
    
    private void UpdateBuildingPlacement()
    {
        if (!_placingBuilding) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mousePosition = InputManager.GetMousePosition();
        Vector2Int gridPosition = Grid.GetGridPosition(mousePosition);

        bool canBuild = CanBuild(gridPosition);
        if (!canBuild) return;
        
        // Subtract resources
        int woodCost = _currentBuildingSO.cost.FirstOrDefault(e => e.resource.name == "Wood")?.amount ?? 0;
        int stoneCost = _currentBuildingSO.cost.FirstOrDefault(e => e.resource.name == "Stone")?.amount ?? 0;
        
        GameManager.Instance.Wood.Value -= woodCost;
        GameManager.Instance.Stone.Value -= stoneCost;
        
        // Place building
        Vector3 worldPosition = Grid.GetWorldPosition(gridPosition);
        Building building = Building.Create(worldPosition, _currentBuildingSO, gridPosition, _currentDirection);
        
        List<Vector2Int> gridPositions = _currentBuildingSO.GetGridPositions(gridPosition, _currentDirection);

        foreach (Vector2Int position in gridPositions)
            Grid.GetGridObject(position).SetValue(building);
    }

    private void UpdateBuildingVisual()
    {
        if (_placingBuilding) return;
        
        Vector3 mousePosition = InputManager.GetMousePosition();
        Vector2Int gridPosition = Grid.GetGridPosition(mousePosition);
        Vector3 worldPosition = Grid.GetWorldPosition(gridPosition);
        
        Vector2Int rotationOffset = _currentBuildingSO.GetRotationOffset(_currentDirection);
        Vector3 gameObjectPosition = worldPosition + new Vector3(rotationOffset.x, 0, rotationOffset.y) * Grid.GetCellSize();
        Quaternion gameObjectRotation = Quaternion.Euler(0, BuildingSO.GetRotationAngle(_currentDirection), 0);
        
        _buildingVisual.transform.position = gameObjectPosition;
        _buildingVisual.transform.rotation = gameObjectRotation;
        
        bool canPlace = CanBuild(gridPosition);
        Renderer renderer = _buildingVisual.transform.Find("Mesh").GetComponent<Renderer>();
        
        if (canPlace)
            renderer.material = visualBlueMaterial;
        else
            renderer.material = visualRedMaterial;
    }
}
