using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class WorldManager : Singleton<WorldManager>
{
    public Grid<Resource> Grid { get; private set; }
    public Grid<Resource> ProximityGrid { get; private set; }
    
    [SerializeField] private ResourceSO wood;
    [SerializeField] private ResourceSO stone;
    
    private Texture2D _texture;
    private int _textureSize;
    
    private readonly List<Vector2Int> _proximityPositions = new()
    {
        new(0, 0),
        new(1, 0),
        new(2, 0),
        new(-1, 0),
        new(-2, 0),
        new(0, 1),
        new(0, 2),
        new(0, -1),
        new(0, -2),
        new(1, 1),
        new(-1, 1),
        new(-1, -1),
        new(-1, 1)
    };

    void Awake()
    {
        _textureSize = 128;
        _texture = GenerateTexture(15f);
        
        float cellSize = 10f;
        float origin = -_textureSize * cellSize * 0.5f;
        
        Grid = MakeGrid(_textureSize, cellSize, origin);
        ProximityGrid = MakeGrid(_textureSize, cellSize, origin);
        
        for (int x = 0; x < _textureSize; x++)
        {
            for (int y = 0; y < _textureSize; y++)
            {
                // Generate resource from color
                Color color = _texture.GetPixel(x, y);
                GridObject<Resource> gridObject = Grid.GetGridObject(x, y);
                
                Vector2Int gridPosition = new Vector2Int(x, y);
                Vector3 worldPosition = Grid.GetWorldPosition(gridPosition);

                Resource resource = null;
                
                if (color.r < 0.15f)
                    resource = Resource.Create(worldPosition, wood, gridPosition);
                else if (color.r > 0.85f)
                    resource = Resource.Create(worldPosition, stone, gridPosition);
                
                gridObject.SetValue(resource);

                if (resource == null) continue;

                // Fill proimity grid
                foreach (Vector2Int relativePosition in _proximityPositions)
                {
                    Vector2Int absolutePosition = relativePosition + gridPosition;
                    ProximityGrid.GetGridObject(absolutePosition)?.SetValue(resource);
                }
            }   
        }
    }

    private Grid<Resource> MakeGrid(int size, float cellSize, float origin)
    {
        return new(
            size,
            size,
            cellSize,
            new(origin, 0, origin)
        );
    }

    private Texture2D GenerateTexture(float scale)
    {
        Texture2D texture2D = new Texture2D(_textureSize, _textureSize);

        for (int x = 0; x < _textureSize; x++)
        {
            for (int y = 0; y < _textureSize; y++)
            {
                Color color = CalculateColor(x, y, scale);
                texture2D.SetPixel(x, y, color);
            }   
        }
        
        texture2D.Apply();
        return texture2D;
    }

    private Color CalculateColor(int x, int y, float scale)
    {
        float xCoordinate = (float)x / _textureSize * scale;
        float yCoordinate = (float)y / _textureSize * scale;
        
        float sample = Mathf.PerlinNoise(xCoordinate, yCoordinate);
        return new Color(sample, sample, sample);
    }
}
