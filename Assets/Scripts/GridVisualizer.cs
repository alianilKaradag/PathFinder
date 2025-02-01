using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private int _gridSizeX = 3;
    [SerializeField] private int _gridSizeY = 3;
    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private Transform _gridParent;
    
    [Header("Path Finding Settings")]
    [SerializeField] private Vector2Int _startPos;
    [SerializeField] private Vector2Int _targetPos;
    [SerializeField, ReorderableList] 
    private List<Vector2Int> _obstacles = new List<Vector2Int>();
    
    private Grid _grid;
    private GameObject[,] _nodeObjects;
    private PathFinder _pathFinder;

    private void Start()
    {
        Debug.Log("Grid oluşturuluyor...");
        _grid = new Grid(_gridSizeX, _gridSizeY);
        _pathFinder = new PathFinder(_grid);
        CreateVisualGrid();
        UpdateAllGrid(_obstacles);
    }

    [Button("Find Path")]
    private void FindPathButton()
    {
        FindNewPath();
    }

    private void CreateVisualGrid()
    {
        _nodeObjects = new GameObject[_gridSizeX, _gridSizeY];
        
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPos = new Vector3(x, 0, y);
                GameObject nodeObj = Instantiate(_nodePrefab, worldPos, Quaternion.identity, _gridParent);
                nodeObj.name = $"Node_{x}_{y}";
                
                // Her node için yeni bir material instance oluştur
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
            Debug.LogError("VisualizePath'e null path geldi!");
            return;
        }

        Debug.Log("Yol görselleştiriliyor...");
        foreach (Node node in path)
        {
            Debug.Log($"Node işaretleniyor: {node.GridPosition}");
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
                Debug.LogError($"Renderer bulunamadı: Node_{x}_{y}");
            }
        }
        else
        {
            Debug.LogError($"Node objesi bulunamadı: {x}, {y}");
        }
    }

    public void UpdateAllGrid(List<Vector2Int> obstacles)
    {
        // Grid'i güncelle
        _grid.UpdateNodeStates(obstacles);
        
        // Görsel güncelleme
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Node node = _grid.GetNode(x, y);
                UpdateNodeVisual(x, y, node.IsWalkable ? Color.white : Color.red);
            }
        }
    }

    public void FindNewPath()
    {
        // Önce engelleri güncelle
        UpdateAllGrid(_obstacles);
        
        // Sonra yol bul
        List<Node> path = _pathFinder.FindPath(_startPos, _targetPos);
        if (path != null)
        {
            VisualizePath(path);
        }
        else
        {
            Debug.LogWarning("Yol bulunamadı!");
            // Yol bulunamadığında zaten engeller güncel olacak
        }
    }
} 