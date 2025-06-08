using UnityEngine;
using dungeonGen;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string[,] grid { get;  private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    public void SetGrid(string[,] newGrid)
    {
        if (newGrid == null || newGrid.GetLength(0) == 0 || newGrid.GetLength(1) == 0)
        {
            Debug.LogError("Invalid grid provided.");
            return;
        }
        grid = newGrid;
    }

    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        return new Vector2(gridPos.x, gridPos.y);
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
    }

    public void setGridPosition(Vector2Int gridPos, string value)
    {
        if (grid == null || gridPos.x < 0 || gridPos.x >= grid.GetLength(0) || gridPos.y < 0 || gridPos.y >= grid.GetLength(1))
        {
            Debug.LogError("Invalid grid position or grid not initialized.");
            return;
        }
        grid[gridPos.x, gridPos.y] = value;

        Debug.Log(gridPos);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
