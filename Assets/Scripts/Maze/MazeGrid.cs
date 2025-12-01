public class MazeGrid
{
    public int Width;
    public int Height;
    public MazeCell[,] Cells;

    public MazeGrid(int width, int height)
    {
        Width = width;
        Height = height;
        Cells = new MazeCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cells[x, y] = new MazeCell(x, y);
            }
        }
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }

    public MazeCell GetCell(int x, int y)
    {
        return Cells[x, y];
    }
}
