using UnityEngine;

public class MazeTest : MonoBehaviour
{
    public new MazeRenderer renderer;
    public int width = 21;   // must be odd
    public int height = 21;  // must be odd

    void Start()
    {
        MazeConfig config = new MazeConfig(width, height, 0);
        MazeGenerator generator = new MazeGenerator();
        MazeGrid grid = generator.Generate(config);

        renderer.Render(grid);
    }
}
