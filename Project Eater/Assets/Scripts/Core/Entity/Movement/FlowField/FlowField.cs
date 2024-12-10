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

        // room의 upperBounds, lowerBounds를 통해 gridSize를 구한다. 
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

    // 장애물이 있을 경우에만 하기 
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

    // ※ Integration : 완성 
    public void CreateIntegrationField(Cell destinationCell)
    {
        this.destinationCell = destinationCell;
        this.destinationCell.cost = 0;
        this.destinationCell.bestCost = 0;

        ConcurrentQueue<Cell> cellsToCheck = new ConcurrentQueue<Cell>();
        cellsToCheck.Enqueue(this.destinationCell);

        int processorCount = System.Environment.ProcessorCount; // 사용 가능한 CPU 코어 수
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

                        // 유효한 이웃인지 확인
                        if (currentNeighbor == null || currentNeighbor.cost == byte.MaxValue) continue;

                        // 비용 계산 및 업데이트
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

        Task.WaitAll(tasks.ToArray()); // 모든 Task가 완료될 때까지 대기
    }

    private Cell GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = orignPos + relativePos;

        // 이웃 Cell이 범위 밖에 있으면 NULL
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
        int processorCount = System.Environment.ProcessorCount; // 사용 가능한 CPU 코어 수
        int totalCells = grid.Length; // 전체 Cell 개수
        int batchSize = totalCells / processorCount; // 각 스레드가 처리할 Cell 수

        var tasks = new List<Task>();

        for (int i = 0; i < processorCount; i++)
        {
            int start = i * batchSize;
            int end = (i == processorCount - 1) ? totalCells : start + batchSize; // 마지막 Batch는 남은 모든 Cell 처리

            tasks.Add(Task.Run(() => ProcessCells(start, end)));
        }

        Task.WaitAll(tasks.ToArray()); // 모든 Task가 완료될 때까지 대기

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
        int gridWidth = grid.GetLength(0); // 그리드의 가로 길이
        int gridHeight = grid.GetLength(1); // 그리드의 세로 길이

        for (int index = start; index < end; index++)
        {
            int x = index % gridWidth; // 현재 Cell의 X 좌표
            int y = index / gridWidth; // 현재 Cell의 Y 좌표

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
