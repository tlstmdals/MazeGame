public class MazeConfig
{
    public int Width;
    public int Height;
    public int Threshold;
    public bool UseRandomSeed;
    public int Seed;

    public MazeConfig(int width, int height, int threshold)
    {
        Width = width;
        Height = height;
        Threshold = threshold;
        UseRandomSeed = true;
        Seed = 0;
    }
}
