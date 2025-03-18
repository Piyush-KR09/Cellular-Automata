using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RandomNoiseGen))]
public class CellularAutomata : MonoBehaviour
{
    [Header("Basic Settings")]
    public int survivalCount = 4; // Cells survive with >= 4 neighbors
    public int birthCount = 5;    // Dead cells become alive with >= 5 neighbors
    public int StateCounts = 3;
    public bool useMoore = true;  // 8-way connectivity

    [Header("Additional Settings")]
    [Range(1,10)]public int generations = 5;   // Number of smoothing iterations
    public bool useStates;
    public Color wallColor;
    public Color baseColor;
    public RawImage displayImg;

    [Header("Update Settings")]
    public float ImgScale;

    RandomNoiseGen noiseGen;
    bool[,] map;
    bool[,] finalMap;
    int[,] stateMap;
    int[,] finalStateMap;

    private void Start()
    {
        noiseGen = GetComponent<RandomNoiseGen>();
        map = noiseGen.MapGen(); // Get initial noise

        // Sets stateMap if use State is true
        if (useStates)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    stateMap[i, j] = map[i, j] ? StateCounts : 0;
                }
            }
        }

        // Generate the map
        finalMap = CellularAutomataFunc(map, generations);

        //Data to ShowVisual()
        Texture2D texture2D = new (noiseGen.width, noiseGen.height);
        ShowVisuals(texture2D);
    }

    private void Update()
    {
        displayImg.transform.localScale = Vector3.one * ImgScale;
    }

    bool[,] CellularAutomataFunc(bool[,] initialMap, int genLeft)
    {
        int width = initialMap.GetLength(0);
        int height = initialMap.GetLength(1);

        // Create a copy of the map
        bool[,] newMap = new bool[width, height];

        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                int aliveNeighbors = CountAliveNeighbors(initialMap, i, j);

                // Apply survival and birth rules
                if (initialMap[i, j])
                    newMap[i, j] = aliveNeighbors >= survivalCount; // Survival
                else
                    newMap[i, j] = aliveNeighbors >= birthCount;    // Birth
            }
        }

        // If more generations are left, recurse
        if (genLeft > 0)
            return CellularAutomataFunc(newMap, genLeft - 1);
        else
            return newMap;
    }

    int CountAliveNeighbors(bool[,] map, int x, int y)
    {
        int count = 0;
        int[,] neighbors = useMoore ? new int[,]
        {
            { x - 1, y + 1 }, 
            { x, y + 1 }, 
            { x + 1, y + 1 },

            { x - 1, y },                 
            { x + 1, y },

            { x - 1, y - 1 }, 
            { x, y - 1 }, 
            { x + 1, y - 1 }

        } : new int[,]
        {
            { x, y + 1 }, 
            { x, y - 1 },

            { x + 1, y }, 
            { x - 1, y }
        };

        for (int i = 0; i < neighbors.GetLength(0); i++)
        {
            int nx = neighbors[i, 0], ny = neighbors[i, 1];
            if (nx >= 0 && ny >= 0 && nx < map.GetLength(0) && ny < map.GetLength(1))
                if (map[nx, ny]) count++;
        }

        return count;
    } 

    private void ShowVisuals(Texture2D texture2D)
    {
        displayImg.rectTransform.sizeDelta = new(noiseGen.width, noiseGen.height);
        displayImg.texture = texture2D;

        if (map != null && finalMap != null)
        {
            int width = map.GetLength(0), height = map.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    texture2D.SetPixel(i, j, finalMap[i, j] ? baseColor : wallColor);
                }
            }
            texture2D.filterMode = FilterMode.Point;
            texture2D.wrapMode = TextureWrapMode.Clamp;
            texture2D.Apply();
        }
    }
}
