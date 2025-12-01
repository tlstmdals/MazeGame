public enum CellType
{
    Wall,
    Path
}

public class MazeCell
{
    public int X;
    public int Y;
    public CellType Type;
    public bool IsVisited;

    public MazeCell(int x, int y)
    {
        X = x;
        Y = y;
        Type = CellType.Wall;
        IsVisited = false;
    }
}
