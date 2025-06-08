using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonGen
{
    [CreateAssetMenu(fileName = "GenPreset", menuName = "Room Generation/GenPreset")]
    public class GenPreset : ScriptableObject
    {
        [Header("Visual Settings")]
        public TileBase floorTile;
        public TileBase wallTile;
        public TileBase lavaTile;
        public TileBase lavaDetailTile;
        public TileBase holeTile;
        public TileBase pillarTile;

        [Space(20)]

        [Header("Prefab Settings")]
        public GameObject SpikePrefab;
        public GameObject chestPrefab;
        public GameObject enemyPrefab;
        public GameObject[] playerPrefabs;

        [Space(20)]

        [Header("Room Settings")]
        public Vector2Int size = new Vector2Int(20, 10);
        public int numTiles = 100;

        [Space(20)]
        [Header("Lava River Settings")]
        public int numRivers = 2;
        public float riverSeparationChance = 0.2f;

        [Space(20)]
        [Header("Hole Settings")]
        public int numHoles = 2;
        public Vector2Int minMaxHoleSize = new Vector2Int(1, 3);

        [Space(20)]
        [Header("Chest Settings")]
        public Vector2Int minMaxChests = new Vector2Int(2, 4);
        public int minChestDistance = 2;

        [Space(20)]
        [Header("Pillar Settings")]
        public Vector2Int minMaxPillars = new Vector2Int(5, 10);
        public int minPillarDistance = 0;

        [Space(20)]
        [Header("Enemy Settings")]
        public Vector2Int minMaxEnemies = new Vector2Int(1, 3);
        public int minEnemyDistance = 0;

        [Space(20)]
        [Header("Player Settings")]
        public int minPlayerDistance = 5;
    }

}
