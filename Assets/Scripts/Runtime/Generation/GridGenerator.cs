using UnityEngine;

namespace AHP
{
    public class GridGenerator : MonoBehaviour
    {
        [Header("Configuration")]
        public GridConfig Config;

        [Header("Debug Visualization")]
        public bool DrawGrid = true;
        public Color GridColor = new Color(0f, 1f, 1f, 0.2f);

        public System.Random Rng { get; private set; }

        private void Awake()
        {
            InitRng();
        }

        public void InitRng()
        {
            if (Config == null)
            {
                Debug.LogWarning("[AHP] GridConfig is null. Assign a config asset.");
                return;
            }

            Rng = Config.UseSeed 
                ? new System.Random(Config.Seed) 
                : new System.Random();
        }

        private void OnDrawGizmos()
        {
            if (!DrawGrid || Config == null) return;

            Gizmos.color = GridColor;
            var origin = transform.position;

            // Vertical lines
            for (int x = 0; x <= Config.Width; x++)
            {
                Vector3 a = origin + new Vector3(x * Config.CellSize, 0f, 0f);
                Vector3 b = origin + new Vector3(x * Config.CellSize, 0f, Config.Height * Config.CellSize);
                Gizmos.DrawLine(a, b);
            }

            // Horizontal lines
            for (int y = 0; y <= Config.Height; y++)
            {
                Vector3 a = origin + new Vector3(0f, 0f, y * Config.CellSize);
                Vector3 b = origin + new Vector3(Config.Width * Config.CellSize, 0f, y * Config.CellSize);
                Gizmos.DrawLine(a, b);
            }
        }
    }
}