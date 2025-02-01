using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private int _gridSizeX = 3;
    [SerializeField] private int _gridSizeY = 3;
    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private Vector2Int _startPos;
    [SerializeField] private Vector2Int _targetPos;
    
    private Grid _grid;
    private GameObject[,] _nodeObjects;
    private PathFinder _pathFinder;

    private void Start()
    {
        Debug.Log("Grid oluşturuluyor...");
        _grid = new Grid(_gridSizeX, _gridSizeY);
        _pathFinder = new PathFinder(_grid);
        CreateVisualGrid();
        SetupTestScenario();
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

    private void SetupTestScenario()
    {
        Debug.Log("Test senaryosu başlatılıyor...");
        
        // Tüm grid'i temizle
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                UpdateNodeVisual(x, y, Color.white);
            }
        }

        // Engel oluştur
        MakeTestObstacles();

        // Test için yol bulma
        Vector2Int startPos = _startPos;
        Vector2Int targetPos = _targetPos;
        
        Debug.Log($"Yol aranıyor: {startPos} -> {targetPos}");
        List<Node> path = _pathFinder.FindPath(startPos, targetPos);
        
        if (path == null)
        {
            Debug.LogWarning("Yol bulunamadı!");
        }
        else
        {
            Debug.Log($"Yol bulundu! Uzunluk: {path.Count}");
            VisualizePath(path);
        }
    }

    private void MakeTestObstacles()
    {
        Debug.Log("Engel oluşturuluyor...");
        _grid.SetWalkable(1, 1, false);
        _grid.SetWalkable(3, 3, false);
        UpdateNodeVisual(1, 1, Color.red);
        UpdateNodeVisual(3, 3, Color.red);
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
        List<Node> path = _pathFinder.FindPath(_startPos, _targetPos);
        if (path != null)
        {
            VisualizePath(path);
        }
    }
} 