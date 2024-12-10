using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
    public Vector3 worldPos;
    public Vector2Int gridPos;
    public byte cost; // 0 ~ 255
    // 해당 값을 비교하여 더 낮은 값이 존재하면 해당 쪽을 bestDirection으로 설정한다. 
    public ushort bestCost;
    public GridDirection bestDirection;

    public Cell(Vector3 worldPos, Vector2Int gridPos)
    {
        this.worldPos = worldPos;
        this.gridPos = gridPos;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void SetCell(Vector3 worldPos, Vector2Int gridPos)
    {
        this.worldPos = worldPos;
        this.gridPos = gridPos;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void Reset()
    {
        worldPos = Vector3.zero;
        gridPos = Vector2Int.zero;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void ResetForUpdate()
    {
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void IncreaseCost(int amount)
    {
        if (cost == byte.MaxValue) return;

        if (amount + cost >= 255)
            cost = byte.MaxValue;
        else
            cost += (byte)amount;
    }
}
