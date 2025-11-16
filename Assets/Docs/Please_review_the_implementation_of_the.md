### Overview
This project provides a lightweight, grid‑based procedural level generator for Unity with two major phases:
- Layout generation (rooms and corridors on an abstract grid)
- Prefab placement (instantiate floors and optional walls into the scene)

It also includes editor tooling for one‑click generation/reseeding and gizmo visualization for debugging.

Key entry points:
- Runtime component: `GridGenerator` (partial)
- Layout algorithm: `LayoutGenerator`
- Placement system: `PlacementPass`
- Data assets: `GridConfig`, `PrefabSet`
- Editor: `GridGeneratorEditor` (inspector buttons)
- Debug visualization: `LayoutDebugGizmos`

Unity version context: 6000.0.47f1 (Unity 6). The scripts target .NET 4.x (`net471`).

---

### Architecture
#### 1) Configuration and determinism
- `GridConfig` (ScriptableObject)
  - Dimensions: `Width`, `Height`, and `CellSize`
  - Determinism: `UseSeed`, `Seed`
  - Helpers: `CellToWorld(x, y)` converts grid to world position; `InBounds(x, y)` clamps, and `OnValidate()` constrains dimensions.
- `GridGenerator` (MonoBehaviour, partial)
  - Holds a reference to `GridConfig` via `public GridConfig Config;`
  - Initializes a `System.Random` RNG via `InitRng()` using `Config.UseSeed` and `Config.Seed`.
  - Draws the base grid lines using gizmos if `DrawGrid` is enabled.

#### 2) Layout generation
- `LayoutGenerator`
  - Input: `GridConfig` and `System.Random`
  - Output: `Cells` array of `CellType` (`Empty`, `Room`, `Corridor`, …) and a `Rooms` list.
  - Default `Generate(...)` process:
    1. Room placement attempts (non‑overlapping with padding) with randomized room sizes/positions.
    2. Connectivity: connects rooms using a nearest‑neighbor strategy to ensure a single connected layout, then adds a few extra connections to create loops.
    3. Carving corridors with simple L‑shaped paths (horizontal‑first vs vertical‑first randomized).
  - Safety checks/logging assist tuning (`Rooms.Count`, etc.).
- `GridGenerator.GenerateLayout()`
  - Validates `Config`
  - (Re)initializes RNG
  - Creates `LayoutGenerator`, calls `Generate()`, and compiles a `GenerationLog` (stored in `GridGenerator.LastLog`).
  - Stores results in `GridGenerator.LastLayout`.

#### 3) Prefab placement
- `PrefabSet` (ScriptableObject)
  - `Items` list of `{ PrefabCategory Category; GameObject Prefab; float Weight; }`
  - `GetRandom(rng, category)` does a weighted random pick among valid items.
- `PrefabCategory` enum
  - Currently: `Floor`, `Wall`, `Prop`, `Light`, `Decal` (placement pass uses `Floor` and `Wall` out of the box).
- `PlacementPass`
  - Inputs: `GridConfig`, `System.Random`, `LayoutGenerator`, `PrefabSet`, `Transform root`
  - `Build(spawnWalls)`
    - Clears previous children under `root` (edit‑safe via `DestroyImmediate`)
    - `SpawnFloors()`: instantiate floor prefabs for all non‑empty and non‑blocked cells
    - `SpawnWalls()`: optional boundary walls on empty cells adjacent to non‑empty cells, deduplicated with a 2D `bool` mask
- `GridGenerator.BuildPlacement()`
  - Validates that `Config`, `LastLayout` (generated), and `Prefabs` are set
  - Creates a `~Generated` child object as `PlacementRoot` if missing
  - Runs `PlacementPass.Build(SpawnWalls)`

#### 4) Editor tooling and debug
- `GridGeneratorEditor` (Custom inspector)
  - Generation:
    - Button: “Generate Layout” → `GridGenerator.GenerateLayout()`
    - Button: “Reseed + Generate” → randomizes `Config.Seed` then generates
  - Placement:
    - Button: “Build Placement” (enabled only if there’s a `LastLayout`)
  - Displays `LastLog` as a help box
- `LayoutDebugGizmos` (MonoBehaviour)
  - References a `GridGenerator` and draws:
    - Filled cubes for cells (`RoomColor` vs `CorridorColor`)
    - Optional wire bounds per room (`DrawRoomBounds`)

---

### Files at a glance
- `Assets/Scripts/Runtime/Generation/GridGenerator.cs` — component storage, RNG init, grid gizmo
- `Assets/Scripts/Runtime/Generation/GridGenerator.Generate.cs` — generation method, logging, state
- `Assets/Scripts/Runtime/Placement/GridGenerator.Placement.cs` — placement entry point and bindings
- `Assets/Scripts/Runtime/Generation/LayoutGenerator.cs` — grid algorithm (rooms + corridors)
- `Assets/Scripts/Runtime/Generation/CellType.cs` — cell types enum
- `Assets/Scripts/Runtime/Placement/PlacementPass.cs` — prefab instantiation logic
- `Assets/Scripts/Runtime/Placement/PrefabSet.cs` — weighted prefab registry
- `Assets/Scripts/Runtime/Placement/PrefabCategory.cs` — prefab categories
- `Assets/Scripts/Runtime/Generation/GridConfig.cs` — grid and seed config
- `Assets/Scripts/Runtime/Generation/GenerationLog.cs` — structured log for UI/editor
- `Assets/Editor/Scripts/GridGeneratorEditor.cs` — editor buttons and last log display
- `Assets/Scripts/Runtime/Generation/LayoutDebugGizmos.cs` — runtime gizmo drawer

---

### How to set it up in your Unity project
1) Create the data assets
- Grid config
  - Project window → Create → `AHP/Configs/GridConfig`
  - Tune values:
    - `Width`, `Height`: initial size (start around 48×32)
    - `CellSize`: world size of each grid cell (e.g., 1.0)
    - `UseSeed`: enable deterministic generation
    - `Seed`: integer for reproducibility
- Prefab set
  - Project window → Create → `AHP/Configs/PrefabSet`
  - Add `Items`:
    - For Floors: assign 1+ floor prefabs with non‑zero weights
    - For Walls: assign 1+ wall prefabs with non‑zero weights
    - Optional: pre‑fill categories you plan to support (Props/Lights/etc.) even if the default pass doesn’t consume them yet

2) Prepare prefabs
- Floor prefabs should logically represent “walkable” tiles and be aligned so that placing at `CellToWorld(x, y)` fits a grid of `CellSize`.
- Wall prefabs should visually work on grid boundaries; rotation/orientation is currently axis‑aligned without rotation logic in `PlacementPass` (they’re dropped centered into the empty neighbor cell). For more precise wall meshes (true edges), consider an edge‑based pass later (see Extensibility).

3) Add the generator to a scene
- Create an empty GameObject (e.g., `LevelGenerator`)
- Add component `GridGenerator`
- Assign fields in the inspector:
  - `Config`: your `GridConfig` asset
  - `DrawGrid`: optional; helpful to see base grid lines
  - Placement section:
    - `Prefabs`: your `PrefabSet` asset
    - `PlacementRoot`: optional; leave empty to auto‑create `~Generated` child
    - `SpawnWalls`: toggle wall placement pass

4) Optional: Add debug gizmos
- Add `LayoutDebugGizmos` to any GameObject (often the same as `GridGenerator`)
- Assign `Generator`
- Enable `DrawCells` and/or `DrawRoomBounds` for live scene‐view visualization

5) Generate and build from the editor
- Select the `GridGenerator` GameObject
- In the inspector under “Generation Controls”:
  - Click `Generate Layout` to run the layout algorithm
  - Or click `Reseed + Generate` to change the seed and then generate
  - Review the “Last Generation Log” box for summary and warnings
- Under “Placement Controls”:
  - Click `Build Placement` to instantiate floor (and optional wall) prefabs under `PlacementRoot`

---

### Using it at runtime (code)
You can drive generation/placement from your own scripts (e.g., for a menu or runtime reseed):
```csharp
using UnityEngine;
using AHP;

public class RuntimeGenDriver : MonoBehaviour
{
    public GridGenerator generator;

    public void NewRun(bool randomizeSeed)
    {
        if (randomizeSeed && generator.Config != null)
        {
            generator.Config.Seed = Random.Range(int.MinValue, int.MaxValue);
            generator.InitRng();
        }

        generator.GenerateLayout();
        generator.BuildPlacement();
    }
}
```
Notes:
- `GenerateLayout()` logs into `generator.LastLog` and writes the `LastLayout` result.
- `BuildPlacement()` requires a valid `LastLayout` and `Prefabs`.

---

### Tuning and expected outcomes
- Room density: controlled primarily by `LayoutGenerator.Generate(roomAttempts, minW, maxW, minH, maxH)` (defaults are inside `LayoutGenerator`). If you need different defaults, expose them on `GridConfig` or `GridGenerator` and pass through.
- Connectivity: the nearest‑neighbor step ensures the graph is connected; additional randomized connections produce loops that suit horror/maze navigation.
- Determinism: enable `UseSeed` on `GridConfig` for exact reproducibility. The editor’s “Reseed + Generate” writes a new seed back into the asset for future runs.

---

### Extensibility ideas
- More placement categories
  - Extend `PlacementPass` to also place `Prop`, `Light`, `Decal` based on rules (e.g., in rooms only, corridor corners, along walls).
- Edge‑based walls
  - Instead of dropping wall prefabs into empty neighbor cells, derive true cell edges between `Empty` and `Room/Corridor` and place oriented wall segments on those edges.
- Rotation & variation
  - Add heuristics to rotate/flip prefabs based on neighbor topology. For floors, you might add weighted tilesets for corners/edges/center.
- Post‑processing passes
  - E.g., doorways at room/corridor boundaries, decals in corridors, spawn points, navmesh baking hooks.
- Performance & pooling
  - For frequent rebuilds, add pooling or use `PrefabUtility.InstantiatePrefab` in editor, and pooled `GameObject` reuse at runtime.

---

### Troubleshooting
- Button disabled: “Build Placement” is greyed out
  - Cause: `LastLayout` is null. Run `Generate Layout` first.
- Error: “[AHP] GridConfig is missing. Cannot generate layout.”
  - Assign a `GridConfig` asset to the `GridGenerator`.
- Error: “[AHP] PrefabSet is missing. Assign a prefab set.”
  - Assign a `PrefabSet` asset before building placement.
- Warning: low room count
  - Increase `GridConfig.Width/Height`, and/or increase room attempts (update `LayoutGenerator.Generate(...)` params or expose them), or reduce padding/room sizes.
- Misaligned tiles
  - Ensure your floor/wall prefabs are modeled to match `CellSize` units and are centered appropriately.
- Nothing appears after placement
  - Check that `PrefabSet.Items` includes at least one `Floor` item with `Weight > 0` and a valid prefab.
- Non‑deterministic results with same seed
  - Ensure you call `InitRng()` when you modify `Seed` at runtime; the editor’s “Reseed + Generate” already does this for you.

---

### Reference: key APIs
- `GridGenerator`
  - `public GridConfig Config`
  - `public bool DrawGrid`
  - `public System.Random Rng { get; private set; }`
  - `public LayoutGenerator LastLayout { get; private set; }`
  - `public string LastLog { get; private set; }`
  - `public void InitRng()`
  - `public void GenerateLayout()`
  - `public void BuildPlacement()`
- `LayoutGenerator`
  - `public CellType[] Cells`
  - `public List<Room> Rooms` (rooms with `X, Y, W, H` and `Center` used for corridors)
  - `public void Generate(int roomAttempts = 80, int minW = 5, int maxW = 12, int minH = 5, int maxH = 10)`
- `PrefabSet`
  - `public List<Entry> Items`
  - `public GameObject GetRandom(System.Random rng, PrefabCategory category)`
- `PlacementPass`
  - `public void Build(bool spawnWalls)`

---

### Quick start checklist
- Create `GridConfig` and `PrefabSet` assets
- Add `GridGenerator` to a GameObject
- Assign `Config` and `Prefabs`
- (Optional) Add `LayoutDebugGizmos` for visual debug
- Click `Generate Layout` then `Build Placement`
- Adjust size/seed/prefab weights to taste

If you want, I can also provide a minimal sample scene setup with placeholder floor/wall prefabs and a tuned `GridConfig` preset for immediate testing.