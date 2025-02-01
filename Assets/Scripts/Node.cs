using UnityEngine;

public class Node
{
    public Vector2Int GridPosition;
    public bool IsWalkable;
    public int GCost;
    public int HCost;
    public Node Parent;

    public int FCost => GCost + HCost;

    public Node(Vector2Int pos, bool walkable)
    {
        GridPosition = pos;
        IsWalkable = walkable;
    }

    public override string ToString()
    {
        return $"Node[{GridPosition}] W:{IsWalkable} G:{GCost} H:{HCost} F:{FCost}";
    }
} 