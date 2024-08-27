using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0; // distance from starting node
    public int hCost = 0; // distance from finishing node
    public Node parentNode; // current Node → 모든 Node에 parentNode가 있기 때문에 TargetNode에서 시작 노드까지 추적할 수 있다. 

    // 생성자
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
