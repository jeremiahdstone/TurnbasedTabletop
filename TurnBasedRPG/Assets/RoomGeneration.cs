using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;



public enum TileType
{
    Empty,
    Wall,
    Floor,
    EnemySpawn,
    Chest,
    Player1Spawn,
    Player2Spawn,
    Lava,
    Water,
    Pillar,
    Spikes,
}



public class RoomGeneration : MonoBehaviour
{
    [Header("Tiles")]
    public Tilemap floorWalls, LavaWater, Details, Entities;
    public TileBase floorTile, wallTile, chestTile, lavaTile, lavaDetailTile, enemyTile, playerTile;

    [Header("Settings")]
    public Vector2Int size = new Vector2Int(20, 20);
    public int numTiles = 100;
    public int numRivers = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the room generation process
        GenerateRoom(size.x, size.y);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Regenerate the room when the space key is pressed
            GenerateRoom(size.x, size.y);
        }
    }

    void GenerateRoom(int width, int height)
    {
        TileType[,] roomLayout = new TileType[width, height];


        // Initialize the room layout with empty tiles
        Vector2Int currentPos = new Vector2Int(width / 2, height / 2);
        for (int i = 0; i < numTiles; i++)
        {

            roomLayout[currentPos.x, currentPos.y] = TileType.Floor;

            // Move randomly
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            currentPos += directions[Random.Range(0, directions.Length)];

            // Clamp to bounds
            currentPos.x = Mathf.Clamp(currentPos.x, 2, width - 3);
            currentPos.y = Mathf.Clamp(currentPos.y, 2, height - 3);
        }


        for(int i = 0; i < numRivers; i++)
            GenerateRiver(roomLayout, width, height, TileType.Lava);


        // Add walls around the edges
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (roomLayout[x, y] == TileType.Empty)
                {
                    bool nearFloor = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            TileType neighbor = roomLayout[x + dx, y + dy];
                            if (neighbor == TileType.Floor || neighbor == TileType.Lava)
                                nearFloor = true;
                        }
                    }
                    if (nearFloor)
                        roomLayout[x, y] = TileType.Wall;
                }
            }
        }


        DrawGrid(roomLayout);
        PrintRoom(roomLayout);
    }

    void PrintRoom(TileType[,] grid)
    {
        string output = "";
        for (int y = grid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                output += grid[x, y] switch
                {
                    TileType.Floor => "=",
                    TileType.Wall => "#",
                    TileType.Lava => "~",
                    TileType.Chest => "C",
                    _ => "_"
                };
            }
            output += "\n";
        }
        Debug.Log(output);
    }

    void GenerateRiver(TileType[,] grid, int width, int height, TileType type)
    {
        bool isHorizontal = Random.value > 0.5f;

        // How long the river should be
        int riverLength = Random.Range(height / 2, height); // You can tweak this

        if (isHorizontal)
        {
            int y = Random.Range(1, height - 1);
            int maxStartX = Mathf.Max(1, width - riverLength - 1);
            int startX = Random.Range(1, maxStartX);

            for (int i = 0; i < riverLength && (startX + i) < width - 1; i++)
            {
                int x = startX + i;
                if (grid[x, y] == TileType.Floor)
                    grid[x, y] = type;
            }
        }
        else
        {
            int x = Random.Range(1, width - 1);
            int maxStartY = Mathf.Max(1, height - riverLength - 1);
            int startY = Random.Range(1, maxStartY);

            for (int i = 0; i < riverLength && (startY + i) < height - 1; i++)
            {
                int y = startY + i;
                if (grid[x, y] == TileType.Floor)
                    grid[x, y] = type;
            }
        }
    }


    public void DrawGrid(TileType[,] grid)
    {
        floorWalls.ClearAllTiles();
        LavaWater.ClearAllTiles();
        Details.ClearAllTiles();
        Entities.ClearAllTiles();



        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                TileType type = grid[x, y];

                switch (type)
                {
                    case TileType.Floor:
                        floorWalls.SetTile(new Vector3Int(x, y, 0), floorTile);
                        break;
                    case TileType.Wall:
                        floorWalls.SetTile(new Vector3Int(x, y, 0), wallTile);
                        break;
                    case TileType.Lava:
                        floorWalls.SetTile(new Vector3Int(x, y, 0), floorTile);
                        LavaWater.SetTile(new Vector3Int(x, y, 0), lavaTile);
                        Details.SetTile(new Vector3Int(x, y, 0), lavaDetailTile);
                        break;
                    case TileType.Chest:
                        floorWalls.SetTile(new Vector3Int(x, y, 0), floorTile);
                        Entities.SetTile(new Vector3Int(x, y, 0), chestTile);
                        break;
                }
            }
        }
    }
}
