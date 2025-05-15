using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;



public enum TileType
{
    Empty,
    Wall,
    Floor,
    EnemySpawn,
    ChestSpawn,
    Player1Spawn,
    Player2Spawn,
    Lava,
    Water,
    Pillar,
    Spikes,
}



public class RoomGeneration : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase chestTile;
    public TileBase lavaTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the room generation process
        GenerateRoom(20, 20);


    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateRoom(int width, int height)
    {
        TileType[,] roomLayout = new TileType[width, height];


        // Initialize the room layout with empty tiles
        Vector2Int currentPos = new Vector2Int(width / 2, height / 2);
        for (int i = 0; i < 100; i++)
        {

            roomLayout[currentPos.x, currentPos.y] = TileType.Floor;

            // Move randomly
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            currentPos += directions[Random.Range(0, directions.Length)];

            // Clamp to bounds
            currentPos.x = Mathf.Clamp(currentPos.x, 1, width - 2);
            currentPos.y = Mathf.Clamp(currentPos.y, 1, height - 2);
        }

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
                            if (roomLayout[x + dx, y + dy] == TileType.Floor)
                                nearFloor = true;
                        }
                    }
                    if (nearFloor)
                        roomLayout[x, y] = TileType.Wall;
                }
            }
        }

        //GenerateRiver(roomLayout, width, height, TileType.Lava);



        DrawGrid(roomLayout);
    }

    void GenerateRiver(TileType[,] grid, int width, int height, TileType type)
    {

        //generate lava rivers
        bool isHorizontal = Random.value > 0.5f;
        int riverLength = Random.Range(height / 2, height);

        Vector2Int startingPos = new Vector2Int(Random.Range(1, width - 1), Random.Range(1, height - 1));
        if (isHorizontal)
        {
            for (int i = 0; i < riverLength; i++)
            {
                if (grid[startingPos.x + i, startingPos.y] == TileType.Floor)
                    grid[startingPos.x + i, startingPos.y] = type;
            }


        }
    }

    public void DrawGrid(TileType[,] grid)
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                TileType type = grid[x, y];
                TileBase tile = null;

                switch (type)
                {
                    case TileType.Floor: tile = floorTile; break;
                    case TileType.Wall: tile = wallTile; break;
                    case TileType.Lava: tile = lavaTile; break;
                }

                if (tile != null)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }
}
