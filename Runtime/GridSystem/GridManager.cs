using UnityEngine;
using System;
using System.Collections.Generic;

public enum GridCoordinateSpace
{
    TWO_DIMENSIONAL,
    THREE_DIMENSIONAL
}

public class GridManager : MonoBehaviour
{
    public static Action OnGridInitialized_;
    [SerializeField] GridCoordinateSpace gridCoordinateSpace;
    [SerializeField] GameObject CellPrefab2D;
    [SerializeField] GameObject CellPrefab3D;
    [SerializeField] int width, height;

    [SerializeField] private Transform GridParent;
    private Cell[,] grid;

    public Cell[,] Grid { get => grid; }
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }

    private CellFactory CellFactory;
    public void Init()
    {
        CellFactory = new CellFactory();
        grid = InitGrid();
        UpdateGridCellNeighbours();
        OnGridInitialized_?.Invoke();
    }

    Cell[,] InitGrid()
    {
        Cell[,] tempGrid = new Cell[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Cell cell = SpawnCell(new Vector2(x, y));
                tempGrid[x, y] = cell;
                tempGrid[x, y].SetIndices(x, y);
            }
        }
        return tempGrid;
    }

    void UpdateGridCellNeighbours()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                Cell tempCell = null;
                // Top cell = y+1
                if (y < height - 1)
                {
                    tempCell = grid[x, y + 1];
                    cell.AddToNeigbhourCells(tempCell);
                }
                // Left cell = x-1
                if (x > 0)
                {
                    tempCell = grid[x - 1, y];
                    cell.AddToNeigbhourCells(tempCell);
                }
                // Right cell = x+1
                if (x < width - 1)
                {
                    tempCell = grid[x + 1, y];
                    cell.AddToNeigbhourCells(tempCell);
                }
                // Bottom cell = y-1
                if (y > 0)
                {
                    tempCell = grid[x, y - 1];
                    cell.AddToNeigbhourCells(tempCell);
                }
            }
        }
    }

    Cell SpawnCell(Vector2 pos)
    {
        GameObject prefab = gridCoordinateSpace == GridCoordinateSpace.TWO_DIMENSIONAL ? CellPrefab2D : CellPrefab3D;
        return CellFactory.SpawnCell(prefab, GetGridParent(), pos, gridCoordinateSpace);
    }

    Transform GetGridParent()
    {
        if (GridParent == null)
        {
            GameObject gridParentObj = new GameObject("GridRoot");
            GridParent = gridParentObj.transform;
        }
        return GridParent;
    }

    public Cell GetCell(int x, int y)
    {
        return grid[x, y];
    }

    public List<Cell> GetOpenCells()
    {
        List<Cell> OpenCells = new List<Cell>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if(cell.IsOpen)
                {
                    OpenCells.Add(cell);
                }
            }
        }
        return OpenCells;
    }

    public List<Cell> GetClosedCells()
    {
        List<Cell> ClosedCells = new List<Cell>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (!cell.IsOpen && !cell.IsCenter)
                {
                    ClosedCells.Add(cell);
                }
            }
        }
        return ClosedCells;
    }

    public Cell[] GetRowCells(int rowIndex)
    {
        Cell[] row = new Cell[width];
        for (int i = 0; i < width; i++)
        {
            row[i] = grid[i, rowIndex];
        }
        return row;
    }

    public Cell[] GetColomnCells(int colomnIndex)
    {
        Cell[] colomn = new Cell[height];
        for (int i = 0; i < height; i++)
        {
            colomn[i] = grid[colomnIndex, i];
        }
        return colomn;
    }
    public void ShowOrHideGrid(bool shouldShow)
    {
        GetGridParent().gameObject.SetActive(shouldShow);
    }
}