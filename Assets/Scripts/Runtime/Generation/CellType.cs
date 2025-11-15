namespace AHP
{
    public enum CellType : byte
    {
        Empty = 0,
        Room = 1,
        Corridor = 2,
        Blocked = 3  // Reserved for future use (e.g., locked areas)
    }
}