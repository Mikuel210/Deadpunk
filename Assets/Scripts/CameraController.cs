using DayNightCycle;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float zoomSpeedMultiplier;
    [SerializeField] private float movementSmoothing;
    
    [Header("Zoom")]
    [Space, SerializeField] private float startingZoom = 10;
    [SerializeField] private float minimumZoom = 0;
    [SerializeField] private float maximumZoom = 50;
    [SerializeField] private float zoomInMultiplier = 1.1f;
    [Space, SerializeField] private float scrollMultiplier = 50;
    [SerializeField] private float scrollSmoothing = 0.25f;
    
    private float _zoom;
    private float _zoomSpeed;
    private float _smoothedZoom;
    
    private Vector3 _movementPosition;
    private Vector3 _smoothedMovementPosition;
    private Vector3 _zoomPosition;
    
    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _zoom = startingZoom;
        
        UpdateZoom();
        _smoothedZoom = _zoom;
    }

    void Update()
    {
        UpdateZoom();
        UpdateMovement();
        UpdatePosition();
    }
    
    private void UpdateZoom() {
        // Get the user input
        bool canZoom = !EventSystem.current.IsPointerOverGameObject();
        float scroll = canZoom ? Input.GetAxis("Mouse ScrollWheel") : 0;
        if (scroll > 0) scroll *= zoomInMultiplier; 
        
        // Apply the zoom
        _zoom -= scroll * scrollMultiplier;
        _zoom = Mathf.Clamp(_zoom, minimumZoom, maximumZoom);
            
        _smoothedZoom = Mathf.SmoothDamp(_smoothedZoom, _zoom, ref _zoomSpeed, scrollSmoothing);
        _zoomPosition = new(0, _smoothedZoom, -_smoothedZoom);
    }

    private void UpdateMovement()
    {
        Vector3 movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        _movementPosition += movementSpeed * Time.deltaTime * (1 + _zoom * zoomSpeedMultiplier) * movementVector;
        _smoothedMovementPosition = Vector3.Lerp(_smoothedMovementPosition, _movementPosition, movementSmoothing * Time.deltaTime);
    }
    
    private void UpdatePosition() => transform.position = initialPosition + _smoothedMovementPosition + _zoomPosition;
}
