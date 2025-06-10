using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GridDragMover : MonoBehaviour
{
    public Tilemap pathTilemap;
    public RuleTile pathTile;

    private float gridSize = 1f; //Size of each grid cell
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCam;
    Player player;

    void Awake()
    {
        player = GetComponent<Player>();
        mainCam = Camera.main;
        pathTilemap = GameObject.Find("PathTilemap").GetComponent<Tilemap>();
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.currentTurnPlayer != gameObject)
            return;
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
            player.distanceLeft
        );

        drawPath(path);

    }

    void drawPath(List<Vector2Int> path)
    {
        pathTilemap.ClearAllTiles(); // Clear previous path tiles

        if (path == null || path.Count == 0) return;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 worldPosA = GameManager.Instance.GridToWorld(path[i]);
            Vector3 worldPosB = GameManager.Instance.GridToWorld(path[i + 1]);
            Debug.DrawLine(worldPosA, worldPosB, Color.green, 0.1f);

            pathTilemap.SetTile(
                new Vector3Int(GameManager.Instance.WorldToGrid(worldPosA).x, GameManager.Instance.WorldToGrid(worldPosA).y, 0),
                pathTile
            );
        }

        pathTilemap.SetTile(
                new Vector3Int(GameManager.Instance.WorldToGrid(path[path.Count - 1]).x, GameManager.Instance.WorldToGrid(path[path.Count - 1]).y, 0),
                pathTile
            );


    }

    void clearPath()
    {
        pathTilemap.ClearAllTiles();
    }

    void OnMouseUp()
    {
        if (GameManager.Instance.currentTurnPlayer != gameObject)
            return;

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
            player.distanceLeft
        );

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No valid path found.");
            transform.position = GameManager.Instance.GridToWorld(player.currentPosition);
            clearPath();
            return;
        }



        Vector2Int finalStep = path[path.Count - 1];

        player.targetPosition = finalStep;
        GameManager.Instance.setGridPosition(player.currentPosition, "floor");
        GameManager.Instance.setGridPosition(finalStep, "P" + player.PlayerNumber);

        transform.position = GameManager.Instance.GridToWorld(finalStep);
        player.currentPosition = finalStep;
        player.useDistance(path.Count - 1);
        clearPath();

        // TEMPORARY: change turn after moving
        if( player.distanceLeft <= 0)
        {
            GameManager.Instance.changeTurn();
        }
        
    }


    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = 10f; // Distance from camera
        return mainCam.ScreenToWorldPoint(screenMousePos);
    }
}
