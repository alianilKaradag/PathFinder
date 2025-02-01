using UnityEngine;

public class Node
{
    public Vector2Int GridPosition {get; private set;}
    public bool IsWalkable {get; private set;}
    public int GCost {get; private set;}
    public int HCost {get; private set;}
    public Node Parent {get; private set;}

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

    public void SetWalkable(bool walkable) => IsWalkable = walkable;
    

    public void SetGCost(int gCost) => GCost = gCost;
    

    public void SetHCost(int hCost) => HCost = hCost;
    

    public void SetParent(Node parent) => Parent = parent;
    

} 