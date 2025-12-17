using System;
using System.Collections.Generic;

public class MazeGenerator
{
    public MazeGrid Generate(MazeConfig config)
    {
        var grid = new MazeGrid(config.Width, config.Height);
        var rng = config.UseRandomSeed ? new Random() : new Random(config.Seed);

        DFS(grid, 0, 0, rng);
        return grid;
    }

    private void DFS(MazeGrid grid, int x, int y, Random rng)
    {
        grid.GetCell(x, y).IsVisited = true;

        var dirs = new List<(int dx, int dy)>
        {
            (1, 0), (-1, 0), (0, 1), (0, -1)
        };

        Shuffle(dirs, rng);

        foreach (var (dx, dy) in dirs)
        {
            int nx = x + dx;
            int ny = y + dy;
            if (!grid.InBounds(nx, ny)) continue;

            var next = grid.GetCell(nx, ny);
            if (!next.IsVisited)
            {
                // ✅ "중간칸 Path" 같은 거 없이, 두 셀 사이 벽을 제거
                grid.RemoveWallBetween(x, y, nx, ny);
                DFS(grid, nx, ny, rng);
            }
        }
    }

    private void Shuffle<T>(IList<T> list, Random rng)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
