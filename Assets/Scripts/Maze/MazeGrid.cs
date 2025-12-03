using UnityEngine;

public class MazeGrid : MonoBehaviour
{
    public int Width;
    public int Height;
    public MazeCell[,] Cells;

    public void Initialize(int width, int height)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Width = width;
        Height = height;
        Cells = new MazeCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cellObject = new GameObject($"MazeCell_{x}_{y}");
                cellObject.transform.SetParent(transform);

                var cell = cellObject.AddComponent<MazeCell>();
                cell.Initialize(x, y);

                Cells[x, y] = cell;
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
