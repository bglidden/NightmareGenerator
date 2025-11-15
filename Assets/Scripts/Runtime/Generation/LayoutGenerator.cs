using System.Collections.Generic;
using UnityEngine;

namespace AHP
{
    public class LayoutGenerator
    {
        private readonly GridConfig _cfg;
        private readonly System.Random _rng;

        public CellType[] Cells { get; private set; }
        public int W => _cfg.Width;
        public int H => _cfg.Height;
        public List<Room> Rooms { get; private set; } = new List<Room>();

        public LayoutGenerator(GridConfig cfg, System.Random rng)
        {
            _cfg = cfg;
            _rng = rng;
            Cells = new CellType[W * H];
        }

        private int Idx(int x, int y) => y * W + x;
        private bool In(int x, int y) => x >= 0 && x < W && y >= 0 && y < H;

        public void Generate(int roomAttempts = 80, int minW = 5, int maxW = 12, int minH = 5, int maxH = 10)
        {
            Rooms.Clear();
            System.Array.Fill(Cells, CellType.Empty);

            // 1) Place rooms
            for (int i = 0; i < roomAttempts; i++)
            {
                int rw = _rng.Next(minW, maxW + 1);
                int rh = _rng.Next(minH, maxH + 1);
                int rx = _rng.Next(2, W - rw - 2);  // Leave 2-cell border
                int ry = _rng.Next(2, H - rh - 2);

                var room = new Room(rx, ry, rw, rh);

                bool overlaps = false;
                foreach (var r in Rooms)
                {
                    if (r.Intersects(room, 2))  // 2-cell padding for horror spacing
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps) continue;

                Rooms.Add(room);
                FillRoom(room);
            }

            // Safety check
            if (Rooms.Count < 2)
            {
                Debug.LogWarning($"[AHP] Only {Rooms.Count} room(s) generated. Increase roomAttempts or grid size.");
            }

            // 2) Connect rooms using improved algorithm
            ConnectRooms();
        }

        private void FillRoom(Room r)
        {
            for (int x = r.X; x < r.X + r.W; x++)
            {
                for (int y = r.Y; y < r.Y + r.H; y++)
                {
                    if (In(x, y))
                        Cells[Idx(x, y)] = CellType.Room;
                }
            }
        }

        private void ConnectRooms()
        {
            if (Rooms.Count == 0) return;

            // Use nearest-neighbor connection to avoid isolated rooms
            var connected = new HashSet<int> { 0 };
            var unconnected = new HashSet<int>();
            for (int i = 1; i < Rooms.Count; i++)
                unconnected.Add(i);

            while (unconnected.Count > 0)
            {
                int bestA = -1, bestB = -1;
                float minDist = float.MaxValue;

                // Find closest pair between connected and unconnected
                foreach (int a in connected)
                {
                    foreach (int b in unconnected)
                    {
                        float dist = Vector2Int.Distance(Rooms[a].Center, Rooms[b].Center);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            bestA = a;
                            bestB = b;
                        }
                    }
                }

                if (bestA >= 0 && bestB >= 0)
                {
                    CarveCorridor(Rooms[bestA].Center, Rooms[bestB].Center);
                    connected.Add(bestB);
                    unconnected.Remove(bestB);
                }
                else break;  // Safety exit
            }

            // Add some extra connections for loops (horror: multiple paths)
            int extraConnections = Mathf.Max(1, Rooms.Count / 5);
            for (int i = 0; i < extraConnections; i++)
            {
                int a = _rng.Next(Rooms.Count);
                int b = _rng.Next(Rooms.Count);
                if (a != b)
                    CarveCorridor(Rooms[a].Center, Rooms[b].Center);
            }
        }

        private void CarveCorridor(Vector2Int a, Vector2Int b)
        {
            // L-shaped corridor (randomize direction)
            bool horizontalFirst = _rng.NextDouble() < 0.5;

            if (horizontalFirst)
            {
                CarveLineX(a.x, b.x, a.y);
                CarveLineY(a.y, b.y, b.x);
            }
            else
            {
                CarveLineY(a.y, b.y, a.x);
                CarveLineX(a.x, b.x, b.y);
            }
        }

        private void CarveLineX(int x0, int x1, int y)
        {
            int step = x0 <= x1 ? 1 : -1;
            for (int x = x0; x != x1 + step; x += step)
            {
                if (In(x, y) && Cells[Idx(x, y)] == CellType.Empty)
                    Cells[Idx(x, y)] = CellType.Corridor;
            }
        }

        private void CarveLineY(int y0, int y1, int x)
        {
            int step = y0 <= y1 ? 1 : -1;
            for (int y = y0; y != y1 + step; y += step)
            {
                if (In(x, y) && Cells[Idx(x, y)] == CellType.Empty)
                    Cells[Idx(x, y)] = CellType.Corridor;
            }
        }
    }
}