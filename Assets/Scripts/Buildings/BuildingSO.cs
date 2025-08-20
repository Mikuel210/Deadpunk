
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Scriptable Objects/Building")]
public class BuildingSO : ScriptableObject
{
    [Header("Data")]
    public List<ResourceAmountPair> cost;
    public Vector2Int dimensions;
    
    [Space] public ResourceSO proximityResource;
    
    [Space] public bool isTurret;
    public bool isWall;
    public bool isSupport;
    
    [Header("Prefabs")]
    public GameObject prefab;
    public GameObject visual;

    public enum Direction
    {
        Down,
        Left,
        Up,
        Right
    }

    public static int GetRotationAngle(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return 0;
            case Direction.Left: return 90;
            case Direction.Up: return 180;
            case Direction.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return new(0, 0);
            case Direction.Left: return new(0, dimensions.x);
            case Direction.Up: return new(dimensions.x, dimensions.y);
            case Direction.Right: return new(dimensions.y, 0);
        }
    }
    
    public static Direction GetNextDirection(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return Direction.Left;
            case Direction.Left: return Direction.Up;
            case Direction.Up: return Direction.Right;
            case Direction.Right: return Direction.Down;
        }
    }
    public static Direction GetPreviousDirection(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return Direction.Right;
            case Direction.Left: return Direction.Down;
            case Direction.Up: return Direction.Left;
            case Direction.Right: return Direction.Up;
        }
    }
    
    public List<Vector2Int> GetGridPositions(Vector2Int origin, Direction direction)
    {
        List<Vector2Int> output = new();

        switch (direction)
        {
            default:
            case Direction.Down:
            case Direction.Up:
                for (int x = 0; x < dimensions.x; x++)
                {
                    for (int z = 0; z < dimensions.y; z++)
                        output.Add(new Vector2Int(x, z) + origin);
                }

                break;
            
            case Direction.Left:
            case Direction.Right:
                for (int x = 0; x < dimensions.y; x++)
                {
                    for (int z = 0; z < dimensions.x; z++)
                        output.Add(new Vector2Int(x, z) + origin);
                }

                break;
        }
        
        return output;
    }
}
