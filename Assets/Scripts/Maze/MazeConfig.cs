using UnityEngine;

public class MazeConfig : MonoBehaviour
{
    public int Width;
    public int Height;
    public int Threshold;
    public bool UseRandomSeed = true;
    public int Seed;

    public void Initialize(int width, int height, int threshold, bool useRandomSeed = true, int seed = 0)
    {
        Width = width;
        Height = height;
        Threshold = threshold;
        UseRandomSeed = useRandomSeed;
        Seed = seed;
    }
}
