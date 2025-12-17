public class MazeGrid
{
    public int Width;
    public int Height;
    public MazeCell[,] Cells;

    // 벽이 있으면 true, 없으면 false
    public bool[,] VerticalWalls;    // (Width+1) x Height
    public bool[,] HorizontalWalls;  // Width x (Height+1)

    public MazeGrid(int width, int height)
    {
        Width = width;
        Height = height;

        Cells = new MazeCell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Cells[x, y] = new MazeCell(x, y);

        VerticalWalls = new bool[width + 1, height];
        HorizontalWalls = new bool[width, height + 1];

        // 초기에는 모든 경계에 벽을 세움
        for (int x = 0; x < width + 1; x++)
            for (int y = 0; y < height; y++)
                VerticalWalls[x, y] = true;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height + 1; y++)
                HorizontalWalls[x, y] = true;
    }

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public MazeCell GetCell(int x, int y) => Cells[x, y];

    // 두 셀 사이의 벽 제거
    public void RemoveWallBetween(int x, int y, int nx, int ny)
    {
        int dx = nx - x;
        int dy = ny - y;

        // 오른쪽으로 이동: x+1 경계의 세로벽 제거
        if (dx == 1 && dy == 0) VerticalWalls[x + 1, y] = false;
        // 왼쪽
        else if (dx == -1 && dy == 0) VerticalWalls[x, y] = false;
        // 위쪽
        else if (dx == 0 && dy == 1) HorizontalWalls[x, y + 1] = false;
        // 아래쪽
        else if (dx == 0 && dy == -1) HorizontalWalls[x, y] = false;
    }
}
