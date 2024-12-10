using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class FlowField 
{
    public Cell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public Cell destinationCell;

    private Room room;

    private Queue<Cell> cellPool;

    public FlowField(Room room)
    {
        this.room = room;

        // room�� upperBounds, lowerBounds�� ���� gridSize�� ���Ѵ�. 
        gridSize = new Vector2Int(room.upperBounds.x - room.lowerBounds.x,
                                  room.upperBounds.y - room.lowerBounds.y);

        cellPool = new Queue<Cell>();
        PrePopulatePool(gridSize.x * gridSize.y);
    }

    private void PrePopulatePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            cellPool.Enqueue(new Cell(Vector3.zero, Vector2Int.zero));
        }
    }

    public void CreateGrid()
    {
        grid = new Cell[gridSize.x, gridSize.y];
        Vector3 cellMidPoint = room.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 wordlPos = ConvertGridPosToWorldPos(cellMidPoint, x, y);
                Cell cell = GetCellFromPool();
                cell.SetCell(wordlPos, new Vector2Int(x, y));

                grid[x, y] = cell;
            }
        }
    }

    private Cell GetCellFromPool()
    {
        if (cellPool.Count > 0)
            return cellPool.Dequeue();
        else
        {
            Debug.LogWarning("Cell Pool is empty! Creating a new Cell.");
            return new Cell(Vector3.zero, Vector2Int.zero);
        }
    }

    private Vector3 ConvertGridPosToWorldPos(Vector3 cellMidPoint, int x, int y)
    {
        Vector3 worldPos = room.grid.CellToWorld(new Vector3Int(x + room.lowerBounds.x,
                                                                y + room.lowerBounds.y,
                                                                0));

        worldPos += cellMidPoint;
        return worldPos;
    }

    // ��ֹ��� ���� ��쿡�� �ϱ� 
    public void CreateCostField()
    {
        Vector3 cellHalfExtents = room.grid.cellSize * 0.5f;
        int terrainMask = LayerMask.GetMask("Obstacle");

        foreach (var cell in grid)
        {
            Collider2D[] obstacles = Physics2D.OverlapBoxAll(cell.worldPos, cellHalfExtents, 0f, terrainMask);
            foreach (var collider2D in obstacles) 
            {
                if (collider2D.gameObject.layer == 20)
                {
                    cell.IncreaseCost(255);
                    continue;
                }
            }
        }
    }

    public void RestFlowFieldForUpdate()
    {
        foreach (var cell in grid)
        {
            cell.ResetForUpdate();
        }
    }

    // �� Integration : �ϼ� 
    public void CreateIntegrationField(Cell destinationCell)
    {
        this.destinationCell = destinationCell;
        this.destinationCell.cost = 0;
        this.destinationCell.bestCost = 0;

        ConcurrentQueue<Cell> cellsToCheck = new ConcurrentQueue<Cell>();
        cellsToCheck.Enqueue(this.destinationCell);

        int processorCount = System.Environment.ProcessorCount; // ��� ������ CPU �ھ� ��
        var tasks = new List<Task>();

        for (int i = 0; i < processorCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                while (cellsToCheck.TryDequeue(out Cell currentCell))
                {
                    foreach (Vector2Int direction in GridDirection.CardinalDirections)
                    {
                        Cell currentNeighbor = GetCellAtRelativePos(currentCell.gridPos, direction);

                        // ��ȿ�� �̿����� Ȯ��
                        if (currentNeighbor == null || currentNeighbor.cost == byte.MaxValue) continue;

                        // ��� ��� �� ������Ʈ
                        ushort newCost = (ushort)(currentCell.bestCost + currentNeighbor.cost);
                        if (newCost < currentNeighbor.bestCost)
                        {
                            currentNeighbor.bestCost = newCost;
                            cellsToCheck.Enqueue(currentNeighbor);
                        }
                    }
                }

            }));
        }

        Task.WaitAll(tasks.ToArray()); // ��� Task�� �Ϸ�� ������ ���
    }

    private Cell GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = orignPos + relativePos;

        // �̿� Cell�� ���� �ۿ� ������ NULL
        if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
            return null;
        else
            return grid[finalPos.x, finalPos.y];
    }

    public Cell GetCellFromWorldPos(Vector3 worldPos)
    {
        var gridPos = room.grid.WorldToCell(worldPos);
        gridPos -= new Vector3Int(room.lowerBounds.x, room.lowerBounds.y, 0);

        if (gridPos.x < 0 || gridPos.x >= gridSize.x || gridPos.y < 0 || gridPos.y >= gridSize.y)
        {   
            return null;
        }

        return grid[gridPos.x, gridPos.y];
    }

    public void CreateFlowField()
    {
        int processorCount = System.Environment.ProcessorCount; // ��� ������ CPU �ھ� ��
        int totalCells = grid.Length; // ��ü Cell ����
        int batchSize = totalCells / processorCount; // �� �����尡 ó���� Cell ��

        var tasks = new List<Task>();

        for (int i = 0; i < processorCount; i++)
        {
            int start = i * batchSize;
            int end = (i == processorCount - 1) ? totalCells : start + batchSize; // ������ Batch�� ���� ��� Cell ó��

            tasks.Add(Task.Run(() => ProcessCells(start, end)));
        }

        Task.WaitAll(tasks.ToArray()); // ��� Task�� �Ϸ�� ������ ���

        /*foreach (var cell in grid)
        {
            Cell bestNeighbor = null;
            int bestCost = cell.bestCost;

            foreach (Vector2Int direction in GridDirection.AllDirections)
            {
                Cell curNeighbor = GetCellAtRelativePos(cell.gridPos, direction);
                if (curNeighbor != null && curNeighbor.bestCost < bestCost)
                {
                    bestCost = curNeighbor.bestCost;
                    bestNeighbor = curNeighbor;
                }
            }

            if (bestNeighbor != null)
            {
                cell.bestDirection = GridDirection.GetDirectionFromV2I(bestNeighbor.gridPos - cell.gridPos);
            }
        }*/
    }

    private void ProcessCells(int start, int end)
    {
        int gridWidth = grid.GetLength(0); // �׸����� ���� ����
        int gridHeight = grid.GetLength(1); // �׸����� ���� ����

        for (int index = start; index < end; index++)
        {
            int x = index % gridWidth; // ���� Cell�� X ��ǥ
            int y = index / gridWidth; // ���� Cell�� Y ��ǥ

            Cell cell = grid[x, y];
            Cell bestNeighbor = null;
            int bestCost = cell.bestCost;

            foreach (Vector2Int direction in GridDirection.AllDirections)
            {
                Cell curNeighbor = GetCellAtRelativePos(cell.gridPos, direction);
                if (curNeighbor != null && curNeighbor.bestCost < bestCost)
                {
                    bestCost = curNeighbor.bestCost;
                    bestNeighbor = curNeighbor;
                }
            }

            if (bestNeighbor != null)
            {
                cell.bestDirection = GridDirection.GetDirectionFromV2I(bestNeighbor.gridPos - cell.gridPos);
            }
        }
    }

    public void ClearGrid()
    {
        if (grid == null) return;

        foreach (var cell in grid)
        {
            if (cell != null)
                ReturnCellToPool(cell);
        }

        grid = null;
    }

    private void ReturnCellToPool(Cell cell)
    {
        cell.Reset();
        cellPool.Enqueue(cell);
    }
}
