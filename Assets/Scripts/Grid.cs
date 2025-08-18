using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T>
{
    private GridObject<T>[,] _gridArray;
    
    private Vector3 _origin;
    private int _width;
    private int _depth;
    private float _cellSize;
    
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int z;
    }
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    
    public Grid(int width, int depth, float cellSize, Vector3 origin)
    {
        _width = width;
        _depth = depth;
        _cellSize = cellSize;
        _origin = origin;
        
        _gridArray = new GridObject<T>[_width, _depth];

        // Make a default grid object for every cell
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                _gridArray[x, z] = new GridObject<T>(this, x, z);
                
                // Debug
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 1000f);
            }   
        }
        
        // Debug
        Debug.DrawLine(GetWorldPosition(0, _depth), GetWorldPosition(_width, _depth), Color.white, 1000f);
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _depth), Color.white, 1000f);
    }
    
    public float GetCellSize() => _cellSize;
    
    public Vector3 GetWorldPosition(int x, int z) => new Vector3(x, 0, z) * _cellSize + _origin;
    public Vector3 GetWorldPosition(Vector2Int gridPosition) => GetWorldPosition(gridPosition.x, gridPosition.y);
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - _origin).x / _cellSize);
        int z = Mathf.FloorToInt((worldPosition - _origin).z / _cellSize);
        
        return new(x, z);
    }
    
    public Vector3 SnapToGrid(Vector3 worldPosition)
    {
        Vector2Int gridPosition = GetGridPosition(worldPosition);
        return GetWorldPosition(gridPosition);
    }
    
    public void SetGridObject(int x, int z, GridObject<T> gridObject)
    {
        if (x < 0 || z < 0 || x >= _width || z >= _depth) return;
        
        _gridArray[x, z] = gridObject;
        OnGridObjectChanged?.Invoke(this, new() { x = x, z = z });
    }
    public void SetGridObject(Vector2Int gridPosition, GridObject<T> gridObject) => SetGridObject(gridPosition.x, gridPosition.y, gridObject);
    public void SetGridObject(Vector3 worldPosition, GridObject<T> gridObject)
    {
        Vector2Int gridPosition = GetGridPosition(worldPosition);
        SetGridObject(gridPosition.x, gridPosition.y, gridObject);
    }

    public GridObject<T> GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < _width && z < _depth)
            return _gridArray[x, z];
        
        return default;
    }
    public GridObject<T> GetGridObject(Vector2Int gridPosition) => GetGridObject(gridPosition.x, gridPosition.y);
    public GridObject<T> GetGridObject(Vector3 worldPosition)
    {
        Vector2Int gridPosition = GetGridPosition(worldPosition);
        return GetGridObject(gridPosition.x, gridPosition.y);
    }
    
    public void InvokeOnGridObjectChanged(int x, int z) => OnGridObjectChanged?.Invoke(this, new() { x = x, z = z });
}

public class GridObject<T>
{
    private Grid<T> _grid;
    private int _x;
    private int _z;
    
    private T _value;

    public GridObject(Grid<T> grid, int x, int z)
    {
        _grid = grid;
        _x = x;
        _z = z;
    }

    public bool CanBuild() => _value == null;
    
    public void SetValue(T value)
    {
        _value = value;
        _grid.InvokeOnGridObjectChanged(_x, _z);
    }
    public void ClearValue() => SetValue(default);
}