using UnityEngine;

namespace AHP
{
    [System.Serializable]
    public struct Room
    {
        public int X, Y, W, H;

        public Room(int x, int y, int w, int h)
        {
            X = x; Y = y; W = w; H = h;
        }

        public Vector2Int Center => new Vector2Int(X + W / 2, Y + H / 2);

        public bool Intersects(Room other, int padding = 1)
        {
            return !(X + W + padding <= other.X || 
                     other.X + other.W + padding <= X ||
                     Y + H + padding <= other.Y || 
                     other.Y + other.H + padding <= Y);
        }

        public bool Contains(int x, int y)
        {
            return x >= X && x < X + W && y >= Y && y < Y + H;
        }
    }
}