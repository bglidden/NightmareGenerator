using UnityEngine;

namespace AHP
{
    public class PlacementPass
    {
        private readonly GridConfig _cfg;
        private readonly System.Random _rng;
        private readonly LayoutGenerator _layout;
        private readonly PrefabSet _prefabs;
        private readonly Transform _root;

        public PlacementPass(GridConfig cfg, System.Random rng, LayoutGenerator layout, 
                           PrefabSet prefabs, Transform root)
        {
            _cfg = cfg;
            _rng = rng;
            _layout = layout;
            _prefabs = prefabs;
            _root = root;
        }

        private int Idx(int x, int y) => y * _cfg.Width + x;
        private bool In(int x, int y) => _cfg.InBounds(x, y);

        public void Build(bool spawnWalls)
        {
            ClearExisting();
            SpawnFloors();

            if (spawnWalls)
                SpawnWalls();
        }

        private void ClearExisting()
        {
#if UNITY_EDITOR
            // Safe for edit mode
            for (int i = _root.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(_root.GetChild(i).gameObject);
#else
            for (int i = _root.childCount - 1; i >= 0; i--)
                Object.Destroy(_root.GetChild(i).gameObject);
#endif
        }

        private void SpawnFloors()
        {
            for (int x = 0; x < _cfg.Width; x++)
            {
                for (int y = 0; y < _cfg.Height; y++)
                {
                    var cellType = _layout.Cells[Idx(x, y)];
                    if (cellType == CellType.Empty || cellType == CellType.Blocked)
                        continue;

                    var floorPrefab = _prefabs.GetRandom(_rng, PrefabCategory.Floor);
                    if (floorPrefab != null)
                    {
                        Vector3 position = _cfg.CellToWorld(x, y);
                        var instance = Object.Instantiate(floorPrefab, position, Quaternion.identity, _root);
                        instance.name = $"Floor_{x}_{y}";
                    }
                }
            }
        }

        private void SpawnWalls()
        {
            // Track placed walls to avoid duplicates
            var wallsPlaced = new bool[_cfg.Width, _cfg.Height];

            for (int x = 0; x < _cfg.Width; x++)
            {
                for (int y = 0; y < _cfg.Height; y++)
                {
                    // Only check non-empty cells for adjacent empties
                    var currentCell = _layout.Cells[Idx(x, y)];
                    if (currentCell == CellType.Empty || currentCell == CellType.Blocked)
                        continue;

                    // Check all 4 neighbors
                    TryPlaceWall(x + 1, y, wallsPlaced);
                    TryPlaceWall(x - 1, y, wallsPlaced);
                    TryPlaceWall(x, y + 1, wallsPlaced);
                    TryPlaceWall(x, y - 1, wallsPlaced);
                }
            }

            Debug.Log($"[AHP] Placed {CountWalls(wallsPlaced)} walls");
        }

        private void TryPlaceWall(int x, int y, bool[,] wallsPlaced)
        {
            // Check bounds
            if (!In(x, y)) return;

            // Already placed a wall here
            if (wallsPlaced[x, y]) return;

            // Only place walls in empty cells
            if (_layout.Cells[Idx(x, y)] != CellType.Empty) return;

            var wallPrefab = _prefabs.GetRandom(_rng, PrefabCategory.Wall);
            if (wallPrefab == null) return;

            Vector3 position = _cfg.CellToWorld(x, y);
            var instance = Object.Instantiate(wallPrefab, position, Quaternion.identity, _root);
            instance.name = $"Wall_{x}_{y}";

            wallsPlaced[x, y] = true;
        }

        private int CountWalls(bool[,] wallsPlaced)
        {
            int count = 0;
            foreach (bool placed in wallsPlaced)
                if (placed) count++;
            return count;
        }
    }
}