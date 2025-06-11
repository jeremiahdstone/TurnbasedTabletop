using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Numerics;





namespace dungeonGen
{
    public enum TileType
    {
        Empty,
        Wall,
        Floor,
        EnemySpawn,
        Chest,
        PlayerPosition,
        Lava,
        Water,
        Pillar,
        Spikes,
        Hole,
    }
    public class RoomGeneration : MonoBehaviour
    {
        [SerializeField] Tilemap floorWalls, Obstacles, Details;
        [SerializeField] GenPreset genPreset;


        [Space(20)]
        [Header("Player Settings")]

        private Vector2Int[] playerSpawnPositions;
        private GameObject runtimeEntitiesParent;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Initialize the room generation process
            createRoom(genPreset.size.x, genPreset.size.y);

            
            // IMPERMANENT SOLUTION: Disable PlayerUI for all players at the start
            //GameObject.Find("PlayerUI 1").SetActive(false);
            GameObject.Find("PlayerUI 2").SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Regenerate the room when the space key is pressed
                createRoom(genPreset.size.x, genPreset.size.y);
            }
        }

        void createRoom(int width, int height)
        {


            bool canReach = false;
            TileType[,] grid = new TileType[width, height];
            playerSpawnPositions = new Vector2Int[genPreset.playerPrefabs.Length];

            int safety = 0;

            while (!canReach)
            {
                if (++safety > 100)
                {
                    Debug.LogError("Room generation failed after 100 attempts.");
                    return;
                }

                grid = GenerateRoom(width, height);
                playerSpawnPositions = GeneratePlayerSpawns(grid, width, height, genPreset.playerPrefabs.Length, genPreset.minPlayerDistance).ToArray();

                canReach = true;
                foreach (Vector2Int pos in playerSpawnPositions)
                {
                    if (!CanReach(grid, pos, playerSpawnPositions[0]))
                    {
                        canReach = false;
                        break;
                    }
                }
            }



            /*-------------------------------------------------------------*/
            
            if (GameManager.Instance == null) //(GAME SPECIFIC CODE) Update the GameManager with the generated grid
            {
                GameObject gm = new GameObject("GameManager");
                gm.AddComponent<GameManager>();
            }
            GameManager.Instance.SetGrid(ToStringGrid(grid));
            /*-------------------------------------------------------------*/
            
            PrintRoom(grid);
            DrawGrid(grid);
            SpawnEntities(grid);

            GameManager.Instance.currentTurnPlayer = GameManager.Instance.players[0]; // (GAME SPECIFIC CODE)
            GameManager.Instance.currentTurnPlayer.GetComponent<Player>().startTurn(); // (GAME SPECIFIC CODE)
            
            //impermanent solution to center the camera
            floorWalls.CompressBounds();
            var cellCenter = floorWalls.cellBounds.center;
            var worldCenter = floorWalls.CellToWorld(Vector3Int.RoundToInt(cellCenter));

            Camera.main.transform.position = new UnityEngine.Vector3(worldCenter.x, worldCenter.y, -10f);
        }

        TileType[,] GenerateRoom(int width, int height)
        {
            TileType[,] roomLayout = new TileType[width, height];


            // Initialize the room layout with empty tiles
            Vector2Int currentPos = new Vector2Int(width / 2, height / 2);
            for (int i = 0; i < genPreset.numTiles; i++)
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

            for (int i = 0; i < genPreset.numHoles; i++)
                GeneratePatch(roomLayout, width, height, TileType.Hole);

            for (int i = 0; i < genPreset.numRivers; i++)
                GenerateTrail(roomLayout, width, height, TileType.Lava);



            GenerateScatter(roomLayout, width, height, TileType.Chest, Random.Range(genPreset.minMaxChests.x, genPreset.minMaxChests.y), genPreset.minChestDistance, true);

            GenerateScatter(roomLayout, width, height, TileType.Pillar, Random.Range(genPreset.minMaxPillars.x, genPreset.minMaxPillars.y), genPreset.minPillarDistance);

            GenerateScatter(roomLayout, width, height, TileType.EnemySpawn, Random.Range(genPreset.minMaxEnemies.x, genPreset.minMaxEnemies.y), genPreset.minEnemyDistance);




            //PrintRoom(roomLayout);

            return roomLayout;

        }

        public static string[,] ToStringGrid(TileType[,] tileGrid) //(GAME SPECIFIC CODE) easier use for fluctuating number of players
    {
        int width = tileGrid.GetLength(0);
        int height = tileGrid.GetLength(1);
        int playerNumber = 1;
        string[,] stringGrid = new string[width, height];

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            TileType type = tileGrid[x, y];
            switch (type)
            {
                case TileType.PlayerPosition:
                    stringGrid[x, y] = "P" + playerNumber;
                    playerNumber++;
                    break;
                default:
                    stringGrid[x, y] = type.ToString().ToLower(); // e.g. "wall", "floor"
                    break;
            }
        }

        return stringGrid;
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
                        TileType.PlayerPosition => "P",
                        TileType.Pillar => "8",
                        TileType.EnemySpawn => "E",
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
                    if (grid[x, y] == TileType.Floor && Random.value > genPreset.riverSeparationChance)
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
                    if (grid[x, y] == TileType.Floor && Random.value > genPreset.riverSeparationChance)
                        grid[x, y] = type;
                }
            }
        }

        void GeneratePatch(TileType[,] grid, int width, int height, TileType type)
        {
            Vector2Int holeSize = new Vector2Int(Random.Range(genPreset.minMaxHoleSize.x, genPreset.minMaxHoleSize.y), Random.Range(genPreset.minMaxHoleSize.x, genPreset.minMaxHoleSize.y));
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


        List<Vector2Int> GeneratePlayerSpawns(TileType[,] grid, int width, int height, int playerCount, int minDistance)
        {
            const int maxAttemptsPerPlayer = 1000;
            List<Vector2Int> playerPositions = new List<Vector2Int>();

            for (int i = 0; i < playerCount; i++)
            {
                bool placed = false;

                for (int attempt = 0; attempt < maxAttemptsPerPlayer; attempt++)
                {
                    int x = Random.Range(2, width - 2);
                    int y = Random.Range(2, height - 2);

                    // Must be on a floor tile
                    if (grid[x, y] != TileType.Floor)
                        continue;

                    // Check for at least one walkable neighbor (inline logic)
                    bool hasNeighbor = false;
                    Vector2Int[] directions = {
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 0), new Vector2Int(-1, 0)
            };

                    foreach (var dir in directions)
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;
                        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                        {
                            TileType neighborType = grid[nx, ny];
                            if (IsWalkable(neighborType))
                            {
                                hasNeighbor = true;
                                break;
                            }
                        }
                    }

                    if (!hasNeighbor)
                        continue;

                    // Enforce distance from other players
                    bool tooClose = false;
                    foreach (var other in playerPositions)
                    {
                        float dx = x - other.x;
                        float dy = y - other.y;
                        float distance = Mathf.Sqrt(dx * dx + dy * dy);
                        if (distance < minDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    // Allow placement if valid or after enough failed attempts
                    if (!tooClose || attempt > maxAttemptsPerPlayer * 0.75f)
                    {
                        grid[x, y] = TileType.PlayerPosition;
                        playerPositions.Add(new Vector2Int(x, y));
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    Debug.LogWarning($"Player {i + 1} could not be placed with full constraints. Skipped or relaxed.");
                }
            }

            return playerPositions;
        }




        bool CanReach(TileType[,] grid, Vector2Int start, Vector2Int target)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            bool[,] visited = new bool[width, height];

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            visited[start.x, start.y] = true;

            Vector2Int[] directions = new Vector2Int[]
            {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (current == target)
                    return true; // Found a path!

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = current + dir;

                    if (neighbor.x >= 0 && neighbor.x < width &&
                        neighbor.y >= 0 && neighbor.y < height &&
                        !visited[neighbor.x, neighbor.y] &&
                        IsWalkable(grid[neighbor.x, neighbor.y]))
                    {
                        visited[neighbor.x, neighbor.y] = true;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return false; // No path found
        }

        bool IsWalkable(TileType type)
        {
            return type == TileType.Floor ||
                   type == TileType.PlayerPosition ||
                   type == TileType.Chest;
        }

        public void DrawGrid(TileType[,] grid)
        {
            floorWalls.ClearAllTiles();
            Obstacles.ClearAllTiles();
            Details.ClearAllTiles();



            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    TileType type = grid[x, y];

                    switch (type)
                    {
                        case TileType.Floor:
                            floorWalls.SetTile(new Vector3Int(x, y, 0), genPreset.floorTile);
                            break;
                        case TileType.Wall:
                            floorWalls.SetTile(new Vector3Int(x, y, 0), genPreset.wallTile);
                            break;
                        case TileType.Lava:
                            floorWalls.SetTile(new Vector3Int(x, y, 0), genPreset.floorTile);
                            Obstacles.SetTile(new Vector3Int(x, y, 0), genPreset.lavaTile);
                            Details.SetTile(new Vector3Int(x, y, 0), genPreset.lavaDetailTile);
                            break;
                        case TileType.Hole:
                            floorWalls.SetTile(new Vector3Int(x, y, 0), genPreset.floorTile);
                            Obstacles.SetTile(new Vector3Int(x, y, 0), genPreset.holeTile);
                            break;
                        case TileType.Pillar:
                            floorWalls.SetTile(new Vector3Int(x, y, 0), genPreset.floorTile);
                            Obstacles.SetTile(new Vector3Int(x, y, 0), genPreset.pillarTile);
                            break;
                        case TileType.Chest:
                        case TileType.EnemySpawn:
                        case TileType.PlayerPosition:
                            floorWalls.SetTile(new Vector3Int(x, y, 0), genPreset.floorTile);
                            break;

                    }
                }
            }
        }

        void SpawnEntities(TileType[,] grid)
        {
            ClearOldEntities(); // before spawning

            int playerNumber = 0;
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    UnityEngine.Vector3 worldPos = floorWalls.CellToWorld(cell) + floorWalls.cellSize / 2;

                    switch (grid[x, y])
                    {
                        case TileType.PlayerPosition:
                            GameObject player = Instantiate(genPreset.playerPrefabs[playerNumber], worldPos, UnityEngine.Quaternion.identity, runtimeEntitiesParent.transform);
                            GameManager.Instance.players.Add(player);
                            player.GetComponent<Player>().PlayerNumber = playerNumber + 1; // Set player number
                            playerNumber++;
                            break;
                        case TileType.EnemySpawn:
                            Instantiate(genPreset.enemyPrefab, worldPos, UnityEngine.Quaternion.identity, runtimeEntitiesParent.transform);
                            break;
                        case TileType.Chest:
                            Instantiate(genPreset.chestPrefab, worldPos, UnityEngine.Quaternion.identity, runtimeEntitiesParent.transform);
                            break;
                    }
                }
            }
        }


        void ClearOldEntities()
        {
            if (runtimeEntitiesParent != null)
                Destroy(runtimeEntitiesParent);

            runtimeEntitiesParent = new GameObject("Entities");
        }

    }

}

