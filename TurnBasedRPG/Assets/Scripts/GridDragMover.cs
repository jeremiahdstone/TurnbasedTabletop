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

        List<Vector2Int> path = AStarPathfinder.FindPath(
            player.currentPosition,
            new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y)
            ),
            player.maxDistance
        );
        if (path != null && path.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Vector3 worldPos = GameManager.Instance.GridToWorld(path[i]);
                if (i + 1 >= path.Count) break; // Prevent out of bounds
                Debug.DrawLine(worldPos, GameManager.Instance.GridToWorld(path[i + 1]), Color.green, 0.1f);
            }
        }

    }

    void OnMouseUp()
    {
        isDragging = false;

        Vector2 snappedPosition = new Vector2(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize
        );

        Vector2Int targetGrid = new Vector2Int(
            Mathf.RoundToInt(snappedPosition.x),
            Mathf.RoundToInt(snappedPosition.y)
        );

        List<Vector2Int> path = AStarPathfinder.FindPath(
            player.currentPosition,
            targetGrid,
            player.maxDistance
        );

        if (path.Count == 0)
        {
            Debug.LogWarning("No valid path found.");
            return;
        }

        // Visualize the path
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 worldPosA = GameManager.Instance.GridToWorld(path[i]);
            Vector3 worldPosB = GameManager.Instance.GridToWorld(path[i + 1]);
            Debug.DrawLine(worldPosA, worldPosB, Color.green, 1f);
        }

        Vector2Int finalStep = path[path.Count - 1];

        Debug.DrawLine(
            GameManager.Instance.GridToWorld(player.currentPosition),
            GameManager.Instance.GridToWorld(finalStep),
            Color.red, 2f
        );

        player.targetPosition = finalStep;
        GameManager.Instance.setGridPosition(player.currentPosition, "floor");
        GameManager.Instance.setGridPosition(finalStep, "P" + player.PlayerNumber);

        transform.position = GameManager.Instance.GridToWorld(finalStep);
        player.currentPosition = finalStep;
    }


    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = 10f; // Distance from camera
        return mainCam.ScreenToWorldPoint(screenMousePos);
    }
}
