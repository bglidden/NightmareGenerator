using UnityEngine;

namespace AHP
{
    public class LayoutDebugGizmos : MonoBehaviour
    {
        [Header("Target")]
        public GridGenerator Generator;

        [Header("Visualization")]
        public bool DrawCells = true;
        public bool DrawRoomBounds = false;
        public Color RoomColor = new Color(0f, 0.8f, 0f, 0.3f);
        public Color CorridorColor = new Color(0.8f, 0.8f, 0f, 0.3f);
        public Color RoomBoundsColor = new Color(1f, 0f, 1f, 0.8f);

        private void OnDrawGizmos()
        {
            if (!DrawCells || Generator == null || Generator.LastLayout == null || Generator.Config == null)
                return;

            var cfg = Generator.Config;
            var layout = Generator.LastLayout;
            var origin = Generator.transform.position;

            // Draw cells
            for (int x = 0; x < cfg.Width; x++)
            {
                for (int y = 0; y < cfg.Height; y++)
                {
                    var cellType = layout.Cells[y * cfg.Width + x];
                    if (cellType == CellType.Empty) continue;

                    Gizmos.color = cellType == CellType.Room ? RoomColor : CorridorColor;
                    Vector3 pos = origin + cfg.CellToWorld(x, y);
                    Vector3 offset = new Vector3(cfg.CellSize * 0.5f, 0.05f, cfg.CellSize * 0.5f);
                    Gizmos.DrawCube(pos + offset, new Vector3(cfg.CellSize * 0.95f, 0.1f, cfg.CellSize * 0.95f));
                }
            }

            // Draw room bounds
            if (DrawRoomBounds)
            {
                Gizmos.color = RoomBoundsColor;
                foreach (var room in layout.Rooms)
                {
                    Vector3 min = origin + cfg.CellToWorld(room.X, room.Y);
                    Vector3 max = origin + cfg.CellToWorld(room.X + room.W, room.Y + room.H);
                    Vector3 center = (min + max) * 0.5f + Vector3.up * 0.5f;
                    Vector3 size = new Vector3(room.W * cfg.CellSize, 1f, room.H * cfg.CellSize);
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
    }
}