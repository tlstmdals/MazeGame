using System;
using System.Collections.Generic;

public class MazeGenerator
{
    public MazeGrid Generate(MazeConfig config)
    {
        var grid = new MazeGrid(config.Width, config.Height);
        var rng = config.UseRandomSeed ? new Random() : new Random(config.Seed);

        grid.Cells[0, 0].Type = CellType.Path;
        DFS(grid, 0, 0, rng);

        return grid;
    }

    private void DFS(MazeGrid grid, int x, int y, Random rng)
    {
        grid.GetCell(x, y).IsVisited = true;

        List<(int dx, int dy)> dirs = new List<(int, int)>
        {
            (1, 0),
            (-1, 0),
            (0, 1),
            (0, -1)
        };

        Shuffle(dirs, rng);

        foreach ((int dx, int dy) in dirs)
        {
            int nx = x + dx * 2;
            int ny = y + dy * 2;
            if (!grid.InBounds(nx, ny))
            {
                continue;
            }

            var nextCell = grid.GetCell(nx, ny);
            if (!nextCell.IsVisited)
            {
                // 길을 뚫는다
                grid.GetCell(x + dx, y + dy).Type = CellType.Path;
                nextCell.Type = CellType.Path;

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
