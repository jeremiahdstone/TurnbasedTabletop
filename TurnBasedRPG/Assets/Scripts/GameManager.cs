using UnityEngine;
using System.Collections.Generic;
using dungeonGen;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string[,] grid { get; private set; }

    public int turn = 1;
    public List<GameObject> players = new List<GameObject>();
    
    public GameObject currentTurnPlayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        

    }

    public void changeTurn()
    {
        GameObject PlayerUI = GameObject.Find("PlayerUI " + currentTurnPlayer.GetComponent<Player>().PlayerNumber.ToString());
        PlayerUI.SetActive(false);
        turn++;
        int currentIndex = players.IndexOf(currentTurnPlayer);
        currentIndex = (currentIndex + 1) % players.Count;
        currentTurnPlayer = players[currentIndex];
        currentTurnPlayer.GetComponent<Player>().startTurn();
        PlayerUI = GameObject.Find("PlayerUI " + currentTurnPlayer.GetComponent<Player>().PlayerNumber.ToString());
        PlayerUI.SetActive(true);
    }

    public void useAction()
    {
        if (currentTurnPlayer != null)
        {
            currentTurnPlayer.GetComponent<Player>().useAction();
        }
        else
        {
            Debug.LogWarning("No player is currently taking their turn.");
        }
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

        PrintRoom(grid);
    }

    public static void PrintRoom(string[,] grid)
    {
        if (grid == null)
        {
            Debug.LogError("Grid is null.");
            return;
        }

        string output = "";
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                string tile = grid[x, y]?.Trim().ToLower();

                output += tile switch
                {
                    "floor" => "=",
                    "wall" => "#",
                    "lava" => "~",
                    "chest" => "C",
                    "hole" => "O",
                    "pillar" => "8",
                    "enemyspawn" => "E",
                    _ when tile != null && tile.StartsWith("p") => "P",
                    _ => "_"
                };
            }
            output += "\n";
        }

        Debug.Log(output);
    }

    

    // Update is called once per frame
    void Update()
    {

    }
}
