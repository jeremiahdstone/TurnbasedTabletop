using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;



public enum TileType
{
    Empty,
    Wall,
    Floor,
    EnemySpawn,
    Chest,
    PlayerSpawn,
    Lava,
    Water,
    Pillar,
    Spikes,
    Hole,
}



public class RoomGeneration : MonoBehaviour
{
    public Tilemap floorWalls, Obstacles, Details, Entities;
    public TileBase floorTile, wallTile, chestTile, lavaTile, lavaDetailTile, enemyTile, playerTile, holeTile, spikeTile, pillarTile;

    [Header("Room Settings")]
    [SerializeField] private Vector2Int size = new Vector2Int(20, 20);
    [SerializeField] private int numTiles = 100;

    [Space(20)]
    [Header("Lava River Settings")]
    [SerializeField] private int numRivers = 2;
    [SerializeField] private float riverSeparationChance = 0.2f;

    [Space(20)]
    [Header("Hole Settings")]
    [SerializeField] private int numHoles = 2;
    [SerializeField] private Vector2Int minMaxHoleSize = new Vector2Int(1, 3);

    [Space(20)]
    [Header("Chest Settings")]
    [SerializeField] private Vector2Int minMaxChests = new Vector2Int(2, 4);
    [SerializeField] private int minChestDistance = 2;

    [Space(20)]
    [Header("Pillar Settings")]
    [SerializeField] private Vector2Int minMaxPillars = new Vector2Int(5, 10);
    [SerializeField] private int minPillarDistance = 0;

    [Space(20)]
    [Header("Player Settings")]
    [SerializeField] private int numPlayers = 2;
    [SerializeField] private int minPlayerDistance = 5;

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
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int fx = Mathf.Clamp(currentPos.x + dx, 2, width - 3);
                    int fy = Mathf.Clamp(currentPos.y + dy, 2, height - 3);
                    roomLayout[fx, fy] = TileType.Floor;
                }
            }

            // Move randomly
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            currentPos += directions[Random.Range(0, directions.Length)];
            currentPos.x = Mathf.Clamp(currentPos.x, 2, width - 3);
            currentPos.y = Mathf.Clamp(currentPos.y, 2, height - 3);
        }

        AddWalls(roomLayout, width, height);

        for (int i = 0; i < numHoles; i++)
            GeneratePatch(roomLayout, width, height, TileType.Hole);

        for (int i = 0; i < numRivers; i++)
            GenerateTrail(roomLayout, width, height, TileType.Lava);



        GenerateScatter(roomLayout, width, height, TileType.Chest, Random.Range(minMaxChests.x, minMaxChests.y), minChestDistance);

        GenerateScatter(roomLayout, width, height, TileType.Pillar, Random.Range(minMaxPillars.x, minMaxPillars.y), minPillarDistance);

        GenerateScatter(roomLayout, width, height, TileType.PlayerSpawn, 2, minPlayerDistance, true);




        DrawGrid(roomLayout);
        PrintRoom(roomLayout);

        //impermanent solution to center the camera
        floorWalls.CompressBounds();
        var cellCenter = floorWalls.cellBounds.center;
        var worldCenter = floorWalls.CellToWorld(Vector3Int.RoundToInt(cellCenter));

        Camera.main.transform.position = new Vector3(worldCenter.x, worldCenter.y, -10f);
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
                    TileType.Hole => "O",
                    TileType.PlayerSpawn => "P",
                    TileType.Pillar => "I",
                    _ => "_"
                };
            }
            output += "\n";
        }
        Debug.Log(output);
    }

    void AddWalls(TileType[,] grid, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == TileType.Floor || grid[x, y] == TileType.Lava)
                {
                    // Check all 8 neighbors
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                if (grid[nx, ny] == TileType.Empty)
                                {
                                    grid[nx, ny] = TileType.Wall;
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    void GenerateTrail(TileType[,] grid, int width, int height, TileType type)
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
                if (grid[x, y] == TileType.Floor && Random.value > riverSeparationChance)
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
                if (grid[x, y] == TileType.Floor && Random.value > riverSeparationChance)
                    grid[x, y] = type;
            }
        }
    }

    void GeneratePatch(TileType[,] grid, int width, int height, TileType type)
    {
        Vector2Int holeSize = new Vector2Int(Random.Range(minMaxHoleSize.x, minMaxHoleSize.y), Random.Range(minMaxHoleSize.x, minMaxHoleSize.y));
        Vector2Int holePos = new Vector2Int(Random.Range(1, width - holeSize.x - 1), Random.Range(1, height - holeSize.y - 1));
        int floorCount = 0;
        for (int x = holePos.x; x < holePos.x + holeSize.x; x++)
        {
            for (int y = holePos.y; y < holePos.y + holeSize.y; y++)
            {
                if (grid[x, y] == TileType.Floor)
                    floorCount++;
            }
        }

        // Require at least 75% of the hole to be over Floor
        int totalTiles = holeSize.x * holeSize.y;
        if (floorCount < totalTiles * 0.75f)
            return; // Skip this hole

        // Place the hole
        for (int x = holePos.x; x < holePos.x + holeSize.x; x++)
        {
            for (int y = holePos.y; y < holePos.y + holeSize.y; y++)
            {
                if (grid[x, y] == TileType.Floor)
                {
                    grid[x, y] = type;
                }
            }
        }
    }

    void GenerateScatter(TileType[,] grid, int width, int height, TileType type, int amount, int minDistance, bool requireExactAmount = false)
    {
        List<Vector2Int> placedPositions = new List<Vector2Int>();
        int attempts = 0;
        int maxAttempts = 5000;

        while (placedPositions.Count < amount && attempts < maxAttempts)
        {
            attempts++;
            int x = Random.Range(3, width - 4);
            int y = Random.Range(3, height - 4);
            Vector2Int candidate = new Vector2Int(x, y);

            if (grid[x, y] != TileType.Floor)
                continue;

            // Check spacing
            bool tooClose = false;
            foreach (var pos in placedPositions)
            {
                if (Vector2Int.Distance(candidate, pos) < minDistance + 1)
                {
                    tooClose = true;
                    break;
                }
            }

            // If too close, skip unless we *must* place and we've exhausted attempts
            if (tooClose && requireExactAmount && attempts > maxAttempts / 2)
            {
                // Relax distance for critical items
                tooClose = false;
            }

            if (!tooClose)
            {
                grid[x, y] = type;
                placedPositions.Add(candidate);
            }
        }

        // Optional warning
        if (placedPositions.Count < amount)
        {
            Debug.LogWarning($"{type}: Only placed {placedPositions.Count}/{amount} with minDistance {minDistance}");
        }
    }




    public void DrawGrid(TileType[,] grid)
    {
        floorWalls.ClearAllTiles();
        Obstacles.ClearAllTiles();
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
                        Obstacles.SetTile(new Vector3Int(x, y, 0), lavaTile);
                        Details.SetTile(new Vector3Int(x, y, 0), lavaDetailTile);
                        break;
                    case TileType.Chest:
                        floorWalls.SetTile(new Vector3Int(x, y, 0), floorTile);
                        Entities.SetTile(new Vector3Int(x, y, 0), chestTile);
                        break;
                    case TileType.Hole:
                        floorWalls.SetTile(new Vector3Int(x, y, 0), floorTile);
                        Obstacles.SetTile(new Vector3Int(x, y, 0), holeTile);
                        break;
                    case TileType.PlayerSpawn:
                        floorWalls.SetTile(new Vector3Int(x, y, 0), floorTile);
                        Entities.SetTile(new Vector3Int(x, y, 0), playerTile);
                        break;
                    case TileType.Pillar:
                        floorWalls.SetTile(new Vector3Int(x, y, 0), floorTile);
                        Obstacles.SetTile(new Vector3Int(x, y, 0), pillarTile);
                        break;
                }
            }
        }
    }
}
