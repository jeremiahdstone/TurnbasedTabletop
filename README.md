# Procedural Room Generation System (Unity 2D Tilemap)

This Unity project implements a flexible and extensible system for procedurally generating 2D tile-based rooms using Unity's Tilemap system. It supports multiple tile layers, configurable generation parameters, and clear separation between visual and gameplay data.

---

## Features

### Core Room Generation
- Customizable room dimensions
- Floor generation using random walk algorithms
- Automatic wall placement with full corner coverage
- Tilemap compression and camera centering support

### Hazard and Obstacle Placement
- Lava rivers with directional control and spacing logic
- Randomized lava pools with variable size
- Hole placement using RuleTiles to allow organic shapes
- Optional spacing validation to preserve gameplay paths

### Entity Placement
- Chest and player spawn placement with configurable minimum distances
- Flexible scatter algorithm with fallback for critical spawns
- Support for multiple entity layers (players, enemies, interactables)

### Tilemap Layering
- Clean separation of visual elements:
  - Floor and wall tiles
  - Lava/water overlays
  - Decorative/RuleTile details
  - Entities (player, enemies, chests)

### Camera and Visualization
- Automatic centering of camera based on tilemap bounds
- Optional zoom adjustments based on room size
- Debugging tools (console-based grid output, warnings on placement failure)

---

## Usage Instructions

### Setup

1. Create a 2D Unity project.
2. Add a Grid GameObject and create the following child Tilemaps:
   - `FloorWalls`
   - `Obstacles`
   - `Details`
   - `Entities`
3. Assign corresponding `TileBase` assets in the `RoomGeneration` script.
4. Attach the script to an empty GameObject.
5. Press Play to auto-generate a room.
6. Press **Space** at runtime to regenerate the layout.

### Configuration

All generation settings are exposed via `[SerializeField]` fields for tuning in the Unity Inspector:

| Setting | Description |
|--------|-------------|
| Room Size | Width and height of the grid |
| Tile Count | Number of steps for the random walk |
| Lava Rivers | Number of rivers and separation chance |
| Holes | Number and size range |
| Chests | Minimum and maximum chest count and spacing |
| Player Spawns | Number of player start positions and minimum distance |

---

## Code Structure

- `RoomGeneration.cs` – Central script for managing room layout, tile placement, hazard logic, and entity scattering.
- Utility methods for:
  - Random walk carving
  - Controlled scatter placement
  - River and pool generation
  - Wall and border enforcement
  - Camera centering based on tilemap bounds

---

## Development Considerations

This system is intended as a base for:
- Roguelike or dungeon-style level generation
- Tactical grid-based gameplay
- Testing and visualizing procedural mechanics

Future improvements may include:
- Integration with navmesh/pathfinding systems
- ScriptableObject configuration presets
- Level saving/loading support

---
