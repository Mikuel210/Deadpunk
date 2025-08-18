using UnityEngine;

public class Building : MonoBehaviour
{
    private BuildingSO _buildingSO;
    private Vector2Int _origin;
    private BuildingSO.Direction _direction;

    public static Building Create(Vector3 worldPosition, BuildingSO buildingSO, Vector2Int origin, BuildingSO.Direction direction)
    {
        Vector2Int rotationOffset = buildingSO.GetRotationOffset(direction);
        Vector3 gameObjectPosition = worldPosition + new Vector3(rotationOffset.x, 0, rotationOffset.y) * BuildingSystem.Instance.Grid.GetCellSize();
        
        Quaternion gameObjectRotation = Quaternion.Euler(0, BuildingSO.GetRotationAngle(direction), 0);
        Transform buildingTransform = Instantiate(buildingSO.prefab, gameObjectPosition, gameObjectRotation).transform;
        
        Building building = buildingTransform.GetComponent<Building>();
        building._buildingSO = buildingSO;
        building._origin = origin;
        building._direction = direction;

        return building;
    }

    public void Demolish()
    {
        Destroy(gameObject);
    }
}
