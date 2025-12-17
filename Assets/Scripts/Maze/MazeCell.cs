public class MazeCell
{
    public int X;
    public int Y;
    public bool IsVisited;

    public MazeCell(int x, int y)
    {
        X = x;
        Y = y;
        IsVisited = false;
    }
}
