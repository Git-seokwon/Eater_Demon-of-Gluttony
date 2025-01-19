using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes 
{
    private int width; // Room width
    private int height; // Room height

    // Node 2차원 배열 
    private Node[,] gridNode; // Room Grid : 2차원 배열

    // 생성자 : Node들로 이루어진 Grid(맵)을 생성 (2차원 배열)
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        // Node[가로, 세로] 만큼의 2차원 배열 생성 
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
        // xPosition과 yPosition은 0부터 시작하기 때문에 < 표시를 사용
        if (xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("좌표가 범위를 넘음");
#endif
            return null;
        }
    }
}
