using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    // Builds a path for the room, from the startGridPosition to the endGridPosition, and add movement steps to the returned Stack
    // Returns null if no path is found
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // Adjust position by lower bounds
        // �� CellPosition�� World Space �������� adjust�Ѵ�. 
        startGridPosition -= (Vector3Int)room.lowerBounds;
        endGridPosition -= (Vector3Int)room.lowerBounds;

        // Create open queue and closed hashset
        PriorityQueue<Node> openNodeList = new PriorityQueue<Node>();
        // HashSet :  https://wlsdn629.tistory.com/entry/%EC%9C%A0%EB%8B%88%ED%8B%B0-Dictionary-HashTable-HastSet-%EA%B0%84%EB%8B%A8-%EC%84%A4%EB%AA%85
        HashSet<Node> closedNodeList = new HashSet<Node>();

        // create gridNodes for path finding 
        GridNodes gridNodes = new GridNodes(room.upperBounds.x - room.lowerBounds.x + 1,  // ����
                                            room.upperBounds.y - room.lowerBounds.y + 1); // ����

        // Set startNode and targetNode
        Node startNode = gridNodes.GetGridNode(startGridPosition.x , startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x , endGridPosition.y);

        // Path Find
        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeList, room);

        if (endPathNode != null)
            // Grid ��ǥ�� World ��ǥ�� ��ȯ�ؼ� ��ȯ
            return CreatePathStack(endPathNode, room);

        return null;
    }

    // Find the shortest path - returns the end Node if a path has been found, else returns null
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, PriorityQueue<Node> openNodeList, 
                                         HashSet<Node> closedNodeList, Room room)
    {
        // Add start node to open list
        openNodeList.Push(startNode);

        // Loop through open node list until empty
        while (openNodeList.Count() > 0)
        {
            // current node : the node in the open list with the lowest FCost
            Node currentNode = openNodeList.Pop();

            // if the current node is target node then finish
            if (currentNode == targetNode)
                return currentNode;

            // add current node to the closed list 
            closedNodeList.Add(currentNode);

            // evaluate fCost for each neighbor of the current node
            EvaluateCurrentNodeNeighbors(currentNode, targetNode, gridNodes, openNodeList, closedNodeList, room);
        }

        return null;
    }

    // Create a Stack<Vector3> containing the movement path
    private static Stack<Vector3> CreatePathStack(Node endPathNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        // Node ���� ����
        Node nextNode = endPathNode;

        // Get mid point of cell
        // �� gridPosition�� ���� �ϴ��� �������� ���� �ֱ� ������ �߰�(MidPosition)�� �Ƿ��� cellSize * 0.5�� �����ָ� �ȴ�. 
        // �� cellSize : The size of each cell in the Grid
        Vector3 cellMidPoint = room.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            // Convert grid position to world position 
            Vector3 worldPosition = room.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.lowerBounds.x,
                                                                         nextNode.gridPosition.y + room.lowerBounds.y,
                                                                         0));

            // ���� worldPosition�� ������ ���� �ϴ����� ���� ����
            // ���⿡ cellMidPoint�� ���������� ����� ����
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            // startNode�� parentNode�� Null�̶� while������ ������. 
            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    // Evaluate neighbor nodes
    private static void EvaluateCurrentNodeNeighbors(Node currentNode, Node targetNode, GridNodes gridNodes, PriorityQueue<Node> openNodeList, 
                                                     HashSet<Node> closedNodeList, Room room)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;
        Node validNeighborNode;

        // Loop through all directions
        // �� ���� ��带 �ѷ��� ��� ��带 �˻� 
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // ���� ���� �ǳʶٱ� 
                if (i == 0 && j == 0)
                    continue;

                // �ֺ� ��� �������� 
                validNeighborNode = GetValidNodeNeighbor(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j,
                                                         gridNodes, closedNodeList, room);

                if (validNeighborNode != null)
                {
                    // Calculate new gCost for neighbor
                    int newCostToNeighbor;

                    // newCostToNeighbor = ���� ����� gCost(from start) + ���� ���� �̿� ��� ���� �Ÿ� 
                    newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, validNeighborNode);

                    // �̿� ��尡 openNodeList�� �ִ��� üũ 
                    bool isValidNeighborNodeInOpenList = openNodeList.Contains(validNeighborNode);

                    // �� !isValidNeighborNodeInOpenList : ���ο� neighborNode
                    // �� newCostToNeighbor < validNeighborNode.gCost : recalculated gCost is lower
                    // �� currnetNode�� �ٲ����� gCost�� ����Ǵ� ���, gCost�� �ֽ�ȭ �Ѵ�. 
                    if (newCostToNeighbor < validNeighborNode.gCost || !isValidNeighborNodeInOpenList)
                    {
                        validNeighborNode.gCost = newCostToNeighbor;
                        validNeighborNode.hCost = GetDistance(validNeighborNode, targetNode);
                        validNeighborNode.parentNode = currentNode;

                        // ���ο� neighborNode�� ���, openNodeList�� ����
                        if (!isValidNeighborNodeInOpenList)
                        {
                            openNodeList.Push(validNeighborNode);
                        }
                    }
                }
            }
        }
    }

    // Evaluate a neighbor node at neighborNodeXPosition, neighborNodeYPosition, using the specified gridNode, 
    // closedNodeList, and room. Return null if the node isn't valid
    // �� valid : ��ȿ��
    private static Node GetValidNodeNeighbor(int neighborNodeXPosition, int neighborNodeYPosition, GridNodes gridNodes, 
                                             HashSet<Node> closedNodeList, Room room)
    {
        // neighborNodeXPosition, neighborNodeYPosition�� 0���� �۰ų� room�� Bound�� �Ѿ�� �߸��� ��ġ �����̹Ƿ� return null
        if (neighborNodeXPosition >= room.upperBounds.x - room.lowerBounds.x || neighborNodeXPosition < 0 ||
            neighborNodeYPosition >= room.upperBounds.y - room.lowerBounds.y || neighborNodeYPosition < 0)
        {
            return null;
        }

        // Get neighbor node
        Node neighborNode = gridNodes.GetGridNode(neighborNodeXPosition, neighborNodeYPosition);

        // if neighbor is in the closed list then skip
        if (closedNodeList.Contains(neighborNode))
            return null;
        else
            return neighborNode;
    }

    // Return the distance int between nodeA and nodeB
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        // Mahf.Abs : ����
        int distanceX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int distanceY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        // ��� ����� ���ڵ� pdf Ȯ�� : �޽��� ���� �ص�
        if (distanceX > distanceY)  
            return 14 * distanceY + 10 * (distanceX - distanceY);
        else
            return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
