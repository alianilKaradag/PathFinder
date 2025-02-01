using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private Node[,] _nodes;
    private int _width;
    private int _height;

    public Grid(int width, int height)
    {
        _width = width;
        _height = height;
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        _nodes = new Node[_width, _height];
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _nodes[x, y] = new Node(new Vector2Int(x, y), true);
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _nodes[x, y];
        }
        
        return null;
    }

    public void SetWalkable(int x, int y, bool walkable)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            _nodes[x, y].IsWalkable = walkable;
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        
        // Sadece 4 yön için komşuları kontrol et (sağ, sol, yukarı, aşağı)
        Vector2Int[] directions = 
        {
            new Vector2Int(0, 1),  // yukarı
            new Vector2Int(0, -1), // aşağı
            new Vector2Int(1, 0),  // sağ
            new Vector2Int(-1, 0)  // sol
        };

        foreach (Vector2Int dir in directions)
        {
            int checkX = node.GridPosition.x + dir.x;
            int checkY = node.GridPosition.y + dir.y;

            Node neighbor = GetNode(checkX, checkY);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
} 