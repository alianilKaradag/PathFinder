using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridVisualizer : MonoBehaviour
{
    public int GridSizeX => _gridSizeX;
    public int GridSizeY => _gridSizeY;

    [SerializeField, OnValueChanged("OnGridSizeChanged")] private int _gridSizeX = 3;
    [SerializeField, OnValueChanged("OnGridSizeChanged")] private int _gridSizeY = 3;
    [SerializeField, Foldout("References")] private GameObject _nodePrefab;
    [SerializeField, Foldout("References")] private Transform _gridParent;
    
    [Header("Path Finding Settings")]
    [SerializeField] private Vector2Int _startPos;
    [SerializeField] private Vector2Int _targetPos;
    [SerializeField, ReorderableList] private List<Vector2Int> _obstacles = new List<Vector2Int>();
    
    private Grid _grid;
    private GameObject[,] _nodeObjects;
    private PathFinder _pathFinder;

    private void Start()
    {
        Debug.Log("Grid is being created...");
        _grid = new Grid(_gridSizeX, _gridSizeY);
        _pathFinder = new PathFinder(_grid);
        CreateVisualGrid();
        UpdateAllGrid(_obstacles);
    }
    
    private void OnGridSizeChanged()
    {
        if (!Application.isPlaying) return;
        
        // Destroy old grid objects
        if (_nodeObjects != null)
        {
            for (int x = 0; x < _nodeObjects.GetLength(0); x++)
            {
                for (int y = 0; y < _nodeObjects.GetLength(1); y++)
                {
                    if (_nodeObjects[x, y] != null)
                    {
                        Destroy(_nodeObjects[x, y]);
                    }
                }
            }
        }

        // Create new grid
        _grid = new Grid(_gridSizeX, _gridSizeY);
        _pathFinder = new PathFinder(_grid);
        CreateVisualGrid();
        UpdateAllGrid(_obstacles);
    }

    private void CreateVisualGrid()
    {
        // Destroy any existing child objects under gridParent
        while (_gridParent.childCount > 0)
        {
            DestroyImmediate(_gridParent.GetChild(0).gameObject);
        }

        _nodeObjects = new GameObject[_gridSizeX, _gridSizeY];
        
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPos = new Vector3(x, 0, y);
                GameObject nodeObj = Instantiate(_nodePrefab, worldPos, Quaternion.identity, _gridParent);
                nodeObj.name = $"Node_{x}_{y}";
                
                var renderer = nodeObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material newMaterial = new Material(renderer.sharedMaterial);
                    renderer.material = newMaterial;
                }
                
                _nodeObjects[x, y] = nodeObj;
            }
        }
    }

    private void VisualizePath(List<Node> path)
    {
        if (path == null)
        {
            return;
        }

        Debug.Log("The path is being visualized...");
        foreach (Node node in path)
        {
            Debug.Log($"Node is being marked: {node.GridPosition}");
            UpdateNodeVisual(node.GridPosition.x, node.GridPosition.y, Color.green);
        }

        if (path.Count > 0)
        {
            Node startNode = path[0];
            Node endNode = path[path.Count - 1];
            
            UpdateNodeVisual(startNode.GridPosition.x, startNode.GridPosition.y, Color.blue);
            UpdateNodeVisual(endNode.GridPosition.x, endNode.GridPosition.y, Color.yellow);
        }
    }

    private void UpdateNodeVisual(int x, int y, Color color)
    {
        if (_nodeObjects[x, y] != null)
        {
            var renderer = _nodeObjects[x, y].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
            else
            {
                Debug.LogError($"Renderer Not Found: Node_{x}_{y}");
            }
        }
        else
        {
            Debug.LogError($"Node Object Not Found: {x}, {y}");
        }
    }

    private void UpdateAllGrid(List<Vector2Int> obstacles)
    {
        _grid.UpdateNodeStates(obstacles);
        
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Node node = _grid.GetNode(x, y);
                UpdateNodeVisual(x, y, node.IsWalkable ? Color.white : Color.red);
            }
        }
    }

    [Button]
    public void FindPath()
    {
        UpdateAllGrid(_obstacles);
        
        List<Node> path = _pathFinder.FindPath(_startPos, _targetPos);
        if (path != null)
        {
            VisualizePath(path);
        }
        else
        {
            Debug.Log("The Path Not Found!");
        }
    }
} 