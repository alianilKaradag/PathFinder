using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class PathFinder
{
    private readonly Grid _grid;

    public PathFinder(Grid grid)
    {
        _grid = grid;
    }

    public List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        Node startNode = _grid.GetNode(startPos.x, startPos.y);
        Node targetNode = _grid.GetNode(targetPos.x, targetPos.y);

        if (startNode == null || targetNode == null) return null;
        
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // En düşük F Cost'a sahip node'u bul
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || 
                    (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Hedef node'a ulaştık mı?
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // Komşuları değerlendir
            foreach (Node neighbor in _grid.GetNeighbors(currentNode))
            {
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newGCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                if (newGCost < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = GetDistance(neighbor, targetNode);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // Yol bulunamadı
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
        int distY = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);
        return distX + distY;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Add(startNode);
        path.Reverse();
        return path;
    }
}
