using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "GenPreset", menuName = "Room Generation/GenPreset")]
public class GenPreset : ScriptableObject
{
    [Header("Visual Settings")]
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase chestTile;
    public TileBase lavaTile;
    public TileBase lavaDetailTile;
    public TileBase enemyTile;
    public TileBase playerTile;
    public TileBase holeTile;
    public TileBase spikeTile;
    public TileBase pillarTile;

    [Space(20)]

    [Header("Room Settings")]
    public Vector2Int size = new Vector2Int(20, 20);
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
}
