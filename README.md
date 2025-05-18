Unity Procedural Room Generator
===============================

A modular, tilemap-driven procedural generation system for top-down 2D games in Unity. This tool uses ScriptableObject presets to generate rooms composed of walls, floors, hazards, and interactive elements like chests and player spawns. Each room guarantees player connectivity and customizable layout features.

-------------------------------
Features
-------------------------------

- Scriptable Presets (`GenPreset`) for easy tuning
- Player spawn connectivity via flood fill pathfinding
- Randomized generation of:
  - Floor/wall layout
  - Lava rivers
  - Holes (rectangular voids)
  - Pillars and chests
  - Player spawn points with distance constraints
- Tilemap layer support:
  - Floor/Walls
  - Obstacles
  - Details
  - Entities
- Camera auto-centering on generated content
- ASCII debug output of generated rooms

-------------------------------
System Overview
-------------------------------

Main Components:

- RoomGeneration.cs: handles all generation and placement
- GenPreset.asset: ScriptableObject containing tile assets and generation settings
- TileType enum: describes the contents of each tile
- Tilemaps: four layers used to separate tile categories

Tilemap Layers:
- floorWalls
- Obstacles
- Details
- Entities

-------------------------------
Setup Instructions
-------------------------------

1. Assign Tilemaps
   - Attach `RoomGeneration` to a GameObject in your scene
   - Assign the 4 tilemap layers in the inspector

2. Create and Assign a GenPreset
   - Right-click in the Project window → Create → Room Generation → GenPreset
   - Fill in all required tile references and settings (room size, number of chests, etc.)
   - Assign this GenPreset to the `genPreset` field in the `RoomGeneration` script

3. Assign Player Tiles
   - Set each player’s tile in the `players[]` array
   - Each tile will be placed at a valid, walkable location

-------------------------------
Usage
-------------------------------

- Enter Play mode in the Unity Editor
- A room is generated automatically on start
- Press SPACE to regenerate the room

The system ensures:
- Players are always on walkable floor
- Players can reach one another
- Entities like chests and hazards are spaced appropriately
- The camera centers on the room bounds

-------------------------------
Generation Logic
-------------------------------

1. Random walker carves floor tiles from the center of the map
2. Walls are placed around floor/lava tiles
3. Lava rivers and holes are added using randomized trail and patch methods
4. Scatterable items like chests and pillars are placed with minimum spacing
5. Players are placed one at a time with:
   - A required minimum distance from each other
   - Walkable surroundings
   - Connectivity confirmed via flood-fill algorithm

-------------------------------
Customization (via GenPreset)
-------------------------------

size:               Width and height of the room grid  
numTiles:           Number of tiles carved during random walk  
numRivers:          Number of lava river paths  
riverSeparationChance: Chance to split or gap lava tiles  
numHoles:           Number of random holes (voids)  
minMaxHoleSize:     Min and max size of rectangular holes  
minMaxChests:       Random range for chest count  
minChestDistance:   Minimum distance between chests  
minMaxPillars:      Random range for pillar count  
minPillarDistance:  Minimum distance between pillars  

Player-specific configuration is done in the `RoomGeneration` script:
- players[]: TileBase array of player tiles
- minPlayerDistance: Minimum distance between player spawns

-------------------------------
Debugging Tools
-------------------------------

- Console printout of generated room layout via `PrintRoom()`
  - Characters:
    
    "=" floor  
    "#" wall  
    "~" lava  
    "O" hole  
    "C" chest  
    "8" pillar  
    

-------------------------------
Potential Extensions
-------------------------------

- Multi-room dungeon support
- Exit generation
- Enemy spawn logic

-------------------------------
Dependencies
-------------------------------

- Unity 2021.3+  
- Uses Unity’s built-in Tilemap system  
- No third-party packages required  

-------------------------------
License
-------------------------------

This project is free to use and adapt. Attribution is appreciated if redistributed or published.
