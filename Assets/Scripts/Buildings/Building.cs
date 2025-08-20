using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingSO BuildingSO { get; private set; }
    public Vector2Int Origin { get; private set; }
    public BuildingSO.Direction Direction { get; private set; }

    public static Building Create(Vector3 worldPosition, BuildingSO buildingSO, Vector2Int origin, BuildingSO.Direction direction)
    {
        Vector2Int rotationOffset = buildingSO.GetRotationOffset(direction);
        Vector3 gameObjectPosition = worldPosition + new Vector3(rotationOffset.x, 0, rotationOffset.y) * BuildingSystem.Instance.Grid.GetCellSize();
        
        Quaternion gameObjectRotation = Quaternion.Euler(0, BuildingSO.GetRotationAngle(direction), 0);
        Transform buildingTransform = Instantiate(buildingSO.prefab, gameObjectPosition, gameObjectRotation).transform;
        
        Building building = buildingTransform.GetComponent<Building>();
        building.BuildingSO = buildingSO;
        building.Origin = origin;
        building.Direction = direction;

        return building;
    }

    public void Demolish()
    {
        List<Vector2Int> gridPositions = BuildingSO.GetGridPositions(Origin, Direction);
        
        foreach (Vector2Int gridPosition in gridPositions)
            BuildingSystem.Instance.Grid.GetGridObject(gridPosition).ClearValue();
        
        Destroy(gameObject);
    }
}
