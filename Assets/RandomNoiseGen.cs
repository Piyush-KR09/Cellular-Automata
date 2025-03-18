using UnityEngine;

public class RandomNoiseGen : MonoBehaviour
{
    public int width;
    public int height;
    [Range(1,100)] public float valueThreshold;

    public bool[,] MapGen()
    {
        bool[,] map = new bool[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //Generate random values and assign it according to the Threshold
                float RandNo = Random.value;
                map[i, j] = RandNo >= valueThreshold / 100;

                //Make the edges a walls
                if (i == width-1 || j == height-1 || j == 0 || i == 0) map[i, j] = false;
            }
        }
        return map;
    }

    public Vector2 GetDimensions() { return new(width, height); }
}
