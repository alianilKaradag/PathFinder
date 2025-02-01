using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GridVisualizer _gridVisualizer;
    [SerializeField] private float _padding = 1f; // Extra space around the grid
    [SerializeField] private float _angle = 90f;  // Top-down angle
    [SerializeField] private float _heightOffset = 1f; // Fine-tune camera height

    private Camera _camera;
    private Vector3 _targetPosition;
    private int _currentGridSizeX;
    private int _currentGridSizeY;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("Camera component not found!");
            return;
        }
        
        // Ensure we're using orthographic projection for top-down view
        _camera.orthographic = true;
    }

    private void Start()
    {
        UpdateCameraPosition();
    }

    private void LateUpdate()
    {
        // Check if grid size has changed
        if (_currentGridSizeX != _gridVisualizer.GridSizeX || 
            _currentGridSizeY != _gridVisualizer.GridSizeY)
        {
            UpdateCameraPosition();
        }
    }

    public void UpdateCameraPosition()
    {
        if (_gridVisualizer == null) return;

        _currentGridSizeX = _gridVisualizer.GridSizeX;
        _currentGridSizeY = _gridVisualizer.GridSizeY;

        // Calculate grid center
        float centerX = (_currentGridSizeX - 1) / 2f;
        float centerY = (_currentGridSizeY - 1) / 2f;

        // Position camera
        transform.position = new Vector3(centerX, _heightOffset, centerY);
        transform.rotation = Quaternion.Euler(_angle, 0, 0);

        // Calculate required orthographic size
        float gridWidth = _currentGridSizeX + _padding * 2;
        float gridHeight = _currentGridSizeY + _padding * 2;

        // Set orthographic size based on the larger dimension
        float aspectRatio = Screen.width / (float)Screen.height;
        float orthoSize = Mathf.Max(gridHeight / 2f, gridWidth / (2f * aspectRatio));
        
        _camera.orthographicSize = orthoSize;
    }
} 