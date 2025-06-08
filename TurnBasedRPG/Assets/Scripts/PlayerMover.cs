using UnityEngine;
using System.Collections.Generic;

public class GridDragMover : MonoBehaviour
{
    public float gridSize = 1f;  // Size of each grid cell
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCam;
    Player player;

    void Start()
    {
        player = GetComponent<Player>();
        mainCam = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorld = GetMouseWorldPosition();
        offset = transform.position - mouseWorld;
        player.currentPosition = GameManager.Instance.WorldToGrid(transform.position);
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
        Vector2 snappedPosition = new Vector2(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize
        );

        transform.position = snappedPosition;
        player.targetPosition = GameManager.Instance.WorldToGrid(snappedPosition);


        List<Vector2Int> path = AStarPathfinder.FindPath(
            player.currentPosition,
            new Vector2Int(
                Mathf.RoundToInt(snappedPosition.x),
                Mathf.RoundToInt(snappedPosition.y)
            ),
            player.maxDistance
        );
        foreach (Vector2Int pos in path)
        {
            Debug.Log("Path position: " + pos);
        }
        
        Debug.DrawLine(GameManager.Instance.GridToWorld(player.currentPosition), GameManager.Instance.GridToWorld(new Vector2Int(
                Mathf.RoundToInt(snappedPosition.x),
                Mathf.RoundToInt(snappedPosition.y)
            )), Color.red, 1f);
        //GameManager.Instance.setGridPosition(GameManager.Instance.WorldToGrid(snappedPosition), "P" + player.PlayerNumber);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = 10f; // Distance from camera
        return mainCam.ScreenToWorldPoint(screenMousePos);
    }
}
