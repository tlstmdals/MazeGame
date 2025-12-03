using UnityEngine;

public enum CellType
{
    Wall,
    Path
}

public class MazeCell : MonoBehaviour
{
    public int X;
    public int Y;
    public CellType Type;
    public bool IsVisited;

    public void Initialize(int x, int y)
    {
        X = x;
        Y = y;
        Type = CellType.Wall;
        IsVisited = false;
    }
}
