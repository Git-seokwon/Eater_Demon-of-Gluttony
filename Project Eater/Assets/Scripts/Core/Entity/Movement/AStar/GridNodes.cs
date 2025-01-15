using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes 
{
    private int width; // Room width
    private int height; // Room height

    // Node 2���� �迭 
    private Node[,] gridNode; // Room Grid : 2���� �迭

    // ������ : Node��� �̷���� Grid(��)�� ���� (2���� �迭)
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        // Node[����, ����] ��ŭ�� 2���� �迭 ���� 
        gridNode = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPosition)
    {
        // xPosition�� yPosition�� 0���� �����ϱ� ������ < ǥ�ø� ���
        if (xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("��ǥ�� ������ ����");
#endif
            return null;
        }
    }
}
