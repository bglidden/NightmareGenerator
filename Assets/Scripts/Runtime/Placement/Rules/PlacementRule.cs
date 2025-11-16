using UnityEngine;

namespace AHP.Rules
{
    public abstract class PlacementRule : ScriptableObject
    {
        [Header("Rule Settings")]
        public bool Enabled = true;
        [Range(0f, 1f)] public float Priority = 0.5f;

        /// <summary>
        /// Evaluate if placement is valid at given position
        /// </summary>
        /// <returns>True if placement is allowed</returns>
        public abstract bool Evaluate(PlacementContext context);

        /// <summary>
        /// Optional: Modify spawn probability (0-1 multiplier)
        /// </summary>
        public virtual float ModifyProbability(PlacementContext context, float baseProbability)
        {
            return baseProbability;
        }
    }

    /// <summary>
    /// Context passed to rules for evaluation
    /// </summary>
    public struct PlacementContext
    {
        public int X, Y;
        public CellType CellType;
        public PrefabCategory Category;
        public GameObject Prefab;
        public LayoutGenerator Layout;
        public GridConfig Config;
        public System.Random Rng;

        // Helper: Get surrounding cells
        public CellType GetCell(int offsetX, int offsetY)
        {
            int tx = X + offsetX;
            int ty = Y + offsetY;
            if (!Config.InBounds(tx, ty)) return CellType.Blocked;
            return Layout.Cells[ty * Config.Width + tx];
        }

        // Helper: Distance to room center
        public float DistanceToNearestRoomCenter()
        {
            float minDist = float.MaxValue;
            foreach (var room in Layout.Rooms)
            {
                float dist = Vector2Int.Distance(new Vector2Int(X, Y), room.Center);
                if (dist < minDist) minDist = dist;
            }
            return minDist;
        }
    }
}