using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0; // distance from starting node
    public int hCost = 0; // distance from finishing node
    public Node parentNode; // current Node �� ��� Node�� parentNode�� �ֱ� ������ TargetNode���� ���� ������ ������ �� �ִ�. 

    // ������
    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;

        parentNode = null;
    }

    // Fcost Property
    public int FCost => gCost + hCost;

    public int CompareTo(Node nodeToCompare)
    {
        // compare(variant) will be < 0 if this instance Fcost is less than nodeToCompare.Fcost
        // compare(variant) will be > 0 if this instance Fcost is greater than nodeToCompare.Fcost
        // compare(variant) will be == 0 if the value are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);

        return compare;
    }
}
