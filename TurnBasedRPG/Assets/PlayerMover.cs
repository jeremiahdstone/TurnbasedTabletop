using UnityEngine;

public class GridDragMover : MonoBehaviour
{
    public float gridSize = 1f;  // Size of each grid cell
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorld = GetMouseWorldPosition();
        offset = transform.position - mouseWorld;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mouseWorld = GetMouseWorldPosition();
        transform.position = mouseWorld + offset;
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Snap to nearest grid position
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            transform.position.z
        );

        transform.position = snappedPosition;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = 10f; // Distance from camera
        return mainCam.ScreenToWorldPoint(screenMousePos);
    }
}
