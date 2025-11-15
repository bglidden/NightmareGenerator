using UnityEngine;

namespace AHP
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "AHP/Configs/GridConfig")]
    public class GridConfig : ScriptableObject
    {
        [Header("Grid Dimensions")]
        [Min(10)] public int Width = 48;
        [Min(10)] public int Height = 32;
        [Min(0.1f)] public float CellSize = 1f;

        [Header("Determinism")]
        public bool UseSeed = true;
        public int Seed = 1337;

        public Vector3 CellToWorld(int x, int y)
        {
            return new Vector3(x * CellSize, 0f, y * CellSize);
        }

        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        // Validation
        private void OnValidate()
        {
            Width = Mathf.Clamp(Width, 10, 256);
            Height = Mathf.Clamp(Height, 10, 256);
        }
    }
}