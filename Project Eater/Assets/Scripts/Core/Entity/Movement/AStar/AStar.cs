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
        // → CellPosition을 World Space 기준으로 adjust한다. 
        startGridPosition -= (Vector3Int)room.lowerBounds;
        endGridPosition -= (Vector3Int)room.lowerBounds;

        // Create open queue and closed hashset
        PriorityQueue<Node> openNodeList = new PriorityQueue<Node>();
        // HashSet :  https://wlsdn629.tistory.com/entry/%EC%9C%A0%EB%8B%88%ED%8B%B0-Dictionary-HashTable-HastSet-%EA%B0%84%EB%8B%A8-%EC%84%A4%EB%AA%85
        HashSet<Node> closedNodeList = new HashSet<Node>();

        // create gridNodes for path finding 
        GridNodes gridNodes = new GridNodes(room.upperBounds.x - room.lowerBounds.x + 1,  // 가로
                                            room.upperBounds.y - room.lowerBounds.y + 1); // 세로

        // Set startNode and targetNode
        Node startNode = gridNodes.GetGridNode(startGridPosition.x , startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x , endGridPosition.y);

        // Path Find
        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeList, room);

        if (endPathNode != null)
            // Grid 좌표를 World 좌표로 변환해서 반환
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

        // Node 추적 시작
        Node nextNode = endPathNode;

        // Get mid point of cell
        // → gridPosition이 좌측 하단을 기준으로 잡혀 있기 때문에 중간(MidPosition)이 되려면 cellSize * 0.5를 더해주면 된다. 
        // ※ cellSize : The size of each cell in the Grid
        Vector3 cellMidPoint = room.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            // Convert grid position to world position 
            Vector3 worldPosition = room.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.lowerBounds.x,
                                                                         nextNode.gridPosition.y + room.lowerBounds.y,
                                                                         0));

            // 현재 worldPosition은 기준이 좌측 하단으로 잡혀 있음
            // 여기에 cellMidPoint을 더해줌으로 가운데로 맞춤
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            // startNode는 parentNode가 Null이라 while문에서 나간다. 
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
        // → 현재 노드를 둘러싼 모든 노드를 검사 
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // 현재 노드는 건너뛰기 
                if (i == 0 && j == 0)
                    continue;

                // 주변 노드 가져오기 
                validNeighborNode = GetValidNodeNeighbor(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j,
                                                         gridNodes, closedNodeList, room);

                if (validNeighborNode != null)
                {
                    // Calculate new gCost for neighbor
                    int newCostToNeighbor;

                    // newCostToNeighbor = 현재 노드의 gCost(from start) + 현재 노드와 이웃 노드 간의 거리 
                    newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, validNeighborNode);

                    // 이웃 노드가 openNodeList에 있는지 체크 
                    bool isValidNeighborNodeInOpenList = openNodeList.Contains(validNeighborNode);

                    // ※ !isValidNeighborNodeInOpenList : 새로운 neighborNode
                    // ※ newCostToNeighbor < validNeighborNode.gCost : recalculated gCost is lower
                    // → currnetNode가 바뀜으로 gCost가 변경되는 경우, gCost를 최신화 한다. 
                    if (newCostToNeighbor < validNeighborNode.gCost || !isValidNeighborNodeInOpenList)
                    {
                        validNeighborNode.gCost = newCostToNeighbor;
                        validNeighborNode.hCost = GetDistance(validNeighborNode, targetNode);
                        validNeighborNode.parentNode = currentNode;

                        // 새로운 neighborNode에 경우, openNodeList에 삽입
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
    // ※ valid : 유효한
    private static Node GetValidNodeNeighbor(int neighborNodeXPosition, int neighborNodeYPosition, GridNodes gridNodes, 
                                             HashSet<Node> closedNodeList, Room room)
    {
        // neighborNodeXPosition, neighborNodeYPosition이 0보다 작거나 room의 Bound를 넘어가면 잘못된 위치 정보이므로 return null
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
        // Mahf.Abs : 절댓값
        int distanceX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int distanceY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        // 계산 방식은 디스코드 pdf 확인 : 메시지 고정 해둠
        if (distanceX > distanceY)  
            return 14 * distanceY + 10 * (distanceX - distanceY);
        else
            return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
