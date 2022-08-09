using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    // Constant variables
    public const int MAP_WIDTH = 100;
    public const int MAP_HEIGHT = 100;
    private const float TREE_DAMPENING_FACTOR = 0.5f;
    private const float STONE_DAMPENING_FACTOR = 4f;
    private const float BUSH_DAMPENING_FACTOR = 8f;
    private const int SPAWN_RADIUS = 7;
    private const float TREE_SPREAD_FACTOR = 2.5f;
    private const float STONE_SPREAD_FACTOR = 2.5f;
    private const float BUSH_SPREAD_FACTOR = 2.5f;
    // Static variables
    public static MapGeneration instance;
    // Public variables
    public GameObject Ground;
    // Private variables
    private int[,] generatedTreeArray = new int[MAP_WIDTH, MAP_HEIGHT];
    private int[,] generatedStoneArray = new int[MAP_WIDTH, MAP_HEIGHT];
    private int[,] generatedBushArray = new int[MAP_WIDTH, MAP_HEIGHT];
    private int[,] spawnAreas = new int[MAP_WIDTH, MAP_HEIGHT];
    private GameObject[,] primArray = new GameObject[MAP_WIDTH, MAP_HEIGHT];
    private const int InitialTreePoints = 50;
    private const int InitialStonePoints = 30;
    private const int InitialBushPoints = 25;
    private int totalTrees = 0;
    private int totalStones = 0;
    private int totalBushes = 0;
    private float minTreeFill = 30f;
    private float minStoneFill = 2.5f;
    private float minBushFill = 2f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        StartCoroutine(GenerateMap());
    }
    IEnumerator GenerateMap()
    {
        Debug.Log("Generating map...");
        Debug.Log("Resizing ground for a " + MAP_WIDTH + " by " + MAP_HEIGHT + " map:");
        //Ground.transform.localScale = new Vector3(2 * MAP_WIDTH / 10 + (2 * 2.5f), 1, 2 * MAP_HEIGHT / 10 + (2 * 2.5f));
        Vector3 newScale = Vector3.zero;
        newScale.x = MAP_WIDTH / 10; // Planes are 10x10 Primative Cubes by default.
        newScale.x *= TREE_SPREAD_FACTOR; // Expand to the largest tile spread size of the resources.
        newScale.y = 1; // Important to set this to 1 so we dont lose the color.
        newScale.z = MAP_HEIGHT / 10; // Planes are 10x10 Primative Cubes by default.
        newScale.z *= TREE_SPREAD_FACTOR; // Expand to the largest tile spread size of the resources.
        Ground.transform.localScale = newScale;//new Vector3(2 * MAP_WIDTH / 10, 1, 2 * MAP_HEIGHT / 10);
        while (true)
        {
            yield return new WaitForSeconds(2f);
            yield return GenerateLayer(generatedTreeArray, "Tree");
            yield return new WaitForSeconds(0.5f);
            yield return GenerateLayer(generatedStoneArray, "Stone");
            yield return new WaitForSeconds(0.5f);
            yield return GenerateLayer(generatedBushArray, "Bush");
            yield return new WaitForSeconds(0.5f);
            yield return ClearSurroundingResources(generatedBushArray, "Bush");
            yield return ClearSurroundingResources(generatedStoneArray, "Stone");
            ClearSpawnArea();

            float treePercentage = (totalTrees * 100f) / ((float)MAP_WIDTH * MAP_HEIGHT);
            float stonePercentage = (totalStones * 100f) / ((float)MAP_WIDTH * MAP_HEIGHT);
            float bushPercentage = (totalBushes * 100f) / ((float)MAP_WIDTH * MAP_HEIGHT);
            Debug.Log("Generation complete");

            bool sufficientResources = true;
            if (treePercentage < minTreeFill)
            {
                sufficientResources = false;
                Debug.Log("Tree " + treePercentage + "% below " + minTreeFill + "%, removing and regenerating...");
            }
            else
                Debug.Log("Tree " + treePercentage + "% above " + minTreeFill + "%, continuing...");
            if (stonePercentage < minStoneFill)
            {
                sufficientResources = false;
                Debug.Log("Stone " + stonePercentage + "% below " + minStoneFill + "%, removing and regenerating...");
            }
            else
                Debug.Log("Stone " + stonePercentage + "% above " + minStoneFill + "%, continuing...");
            if (bushPercentage < minBushFill)
            {
                sufficientResources = false;
                Debug.Log("Bush " + bushPercentage + "% below " + minBushFill + "%, removing and regenerating...");
            }
            else
                Debug.Log("Bush " + bushPercentage + "% above " + minBushFill + "%, continuing...");

            if (sufficientResources)
                break;

            yield return new WaitForSeconds(3f);
            RemoveClusters();
        }
    }
    IEnumerator GenerateLayer(int[,] resourceArray, string resourceType)
    {
        int initialPoints;
        switch (resourceType)
        { // Set how many starting resources we want to spawn. From which will be each resource cluster
            case "Tree":
                initialPoints = InitialTreePoints;
                break;
            case "Stone":
                initialPoints = InitialStonePoints;
                break;
            case "Bush":
                initialPoints = InitialBushPoints;
                break;
            default:
                Debug.Log("Invalid string passed into GenerateLayer: " + resourceType==null ? "NULL" : resourceType);
                //return;
                yield break;
        }
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            for (int j = 0; j < MAP_HEIGHT; j++)
            {
                resourceArray[i, j] = 0;
                if (Random.Range(0, MAP_WIDTH * MAP_HEIGHT) <= initialPoints)
                {
                    yield return SpawnResource(i, j, resourceArray, resourceType);
                }
            }
        }
        yield return BeginClusterCreep(resourceArray, resourceType);
    }
    IEnumerator BeginClusterCreep(int[,] resourceArray, string resourceType)
    {
        int[,] startingPoints = new int[MAP_WIDTH, MAP_HEIGHT];
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            for (int j = 0; j < MAP_HEIGHT; j++)
            {
                startingPoints[i, j] = resourceArray[i, j];
            }
        }
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            for (int j = 0; j < MAP_HEIGHT; j++)
            {
                if (startingPoints[i, j] == 1)
                {
                    yield return CreepPoint(new Vector2(i, j), 0, resourceArray, resourceType);
                }
            }
        }
    }
    IEnumerator CreepPoint(Vector2 parentPoint, int generation, int[,] resourceArray, string resourceType)
    {
        int randChance = 0;
        float dampeningFactor = 0;
        switch (resourceType)
        {
            case "Tree":
                dampeningFactor = generation * TREE_DAMPENING_FACTOR;
                break;
            case "Stone":
                dampeningFactor = generation * STONE_DAMPENING_FACTOR;
                break;
            case "Bush":
                dampeningFactor = generation * BUSH_DAMPENING_FACTOR;
                break;
            default:
                dampeningFactor = 10f;
                break;
        }
        if (parentPoint.x - 1 >= 0 && resourceArray[(int)parentPoint.x - 1, (int)parentPoint.y] == 0)
        { // Check Left
            if (Random.Range(0f, 10f) >= randChance + dampeningFactor)
            {
                yield return SpawnResource((int)parentPoint.x - 1, (int)parentPoint.y, resourceArray, resourceType);
                yield return CreepPoint(new Vector2(parentPoint.x - 1, parentPoint.y), generation + 1, resourceArray, resourceType);
            }
        }
        if (parentPoint.x + 1 < MAP_WIDTH && resourceArray[(int)parentPoint.x + 1, (int)parentPoint.y] == 0)
        { // Check Right
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                yield return SpawnResource((int)parentPoint.x + 1, (int)parentPoint.y, resourceArray, resourceType);
                yield return CreepPoint(new Vector2(parentPoint.x + 1, parentPoint.y), generation + 1, resourceArray, resourceType);
            }
        }
        if (parentPoint.y + 1 < MAP_HEIGHT && resourceArray[(int)parentPoint.x, (int)parentPoint.y + 1] == 0)
        { // Check Up
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                yield return SpawnResource((int)parentPoint.x, (int)parentPoint.y + 1, resourceArray, resourceType);
                yield return CreepPoint(new Vector2(parentPoint.x, parentPoint.y + 1), generation + 1, resourceArray, resourceType);
            }
        }
        if (parentPoint.y - 1 >= 0 && resourceArray[(int)parentPoint.x, (int)parentPoint.y - 1] == 0)
        { // Check Down
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                yield return SpawnResource((int)parentPoint.x, (int)parentPoint.y - 1, resourceArray, resourceType);
                yield return CreepPoint(new Vector2(parentPoint.x, parentPoint.y - 1), generation + 1, resourceArray, resourceType);
            }
        }
    }
    void RemoveClusters()
    {
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            for (int j = 0; j < MAP_HEIGHT; j++)
            {
                ClearPoint(i, j);
            }
        }
    }
    void ClearPoint(int x, int y)
    {
        if (generatedTreeArray[x, y] == 1 ||
            generatedStoneArray[x, y] == 1 ||
            generatedBushArray[x, y] == 1)
        {
            Destroy(primArray[x, y]);
            if (generatedTreeArray[x, y] == 1)
                totalTrees--;
            if (generatedStoneArray[x, y] == 1)
                totalStones--;
            if (generatedBushArray[x, y] == 1)
                totalBushes--;
            generatedTreeArray[x, y] = 0;
            generatedStoneArray[x, y] = 0;
            generatedBushArray[x, y] = 0;
        }
    }
    void ClearSpawnArea()
    {
        Vector2 p1Spawn = new Vector2((int)(MAP_WIDTH * 0.2f), (int)(MAP_HEIGHT * 0.2f));
        Vector2 p2Spawn = new Vector2((int)(MAP_WIDTH * 0.8f), (int)(MAP_HEIGHT * 0.8f));
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            for (int j = 0; j < MAP_HEIGHT; j++)
            {
                // Check to see if each point is within the spawn area's radius
                if ((Mathf.Pow(i - p1Spawn.x, 2) + Mathf.Pow(j - p1Spawn.y, 2) <= Mathf.Pow(SPAWN_RADIUS, 2)) || //(x-i)^2 + (y-j)^2 <= r^2
                    (Mathf.Pow(i - p2Spawn.x, 2) + Mathf.Pow(j - p2Spawn.y, 2) <= Mathf.Pow(SPAWN_RADIUS, 2)))   //(x-i)^2 + (y-j)^2 <= r^2
                {
                    spawnAreas[i, j] = 1;
                    ClearPoint(i, j);
                }
                else
                {
                    spawnAreas[i, j] = 0;
                }
            }
        }
    }
    IEnumerator ClearSurroundingResources(int[,] resourceArray, string resourceType)
    {
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            for (int j = 0; j < MAP_HEIGHT; j++)
            {
                if (resourceArray[i, j] == 1)
                {
                    if (i - 1 >= 0 && resourceArray[i - 1, j] == 0)
                    {
                        ClearPoint(i - 1, j);
                        yield return new WaitForSeconds(0.01f);
                    }
                    if (i + 1 < MAP_WIDTH && resourceArray[i + 1, j] == 0)
                    {
                        ClearPoint(i + 1, j);
                        yield return new WaitForSeconds(0.01f);
                    }
                    if (j - 1 >= 0 && resourceArray[i, j - 1] == 0)
                    {
                        ClearPoint(i, j - 1);
                        yield return new WaitForSeconds(0.01f);
                    }
                    if (j + 1 < MAP_HEIGHT && resourceArray[i, j + 1] == 0)
                    {
                        ClearPoint(i, j + 1);
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
        }
        yield break;
    }
    IEnumerator SpawnResource(int x, int y, int[,] resourceArray, string resourceType)
    {
        float spreadAmount = 1f;
        float randSize = 1f;
        switch (resourceType)
        {
            case "Tree":
                totalTrees++;
                spreadAmount = TREE_SPREAD_FACTOR;
                randSize = Random.Range(0.75f, 1.25f);
                break;
            case "Stone":
                totalStones++;
                spreadAmount = STONE_SPREAD_FACTOR;
                randSize = Random.Range(1.25f, 1.75f);
                break;
            case "Bush":
                totalBushes++;
                spreadAmount = BUSH_SPREAD_FACTOR;
                randSize = Random.Range(2f, 2.5f);
                break;
        }
        GameObject go = Instantiate(ResourceDictionary.instance.GetPrefab(resourceType + "AIO"));
        float randomRotation = Random.Range(0, 360);
        go.transform.rotation = Quaternion.Euler(0, randomRotation, 0);
        go.transform.localScale = new Vector3(randSize, randSize, randSize);
        //go.transform.position = new Vector3((2.5f * x) + .5f - (2.5f * MAP_WIDTH / 2), 0, (2.5f * y) + .5f - (2.5f * MAP_HEIGHT / 2));
        Vector3 newPosition = Vector3.zero;
        newPosition.x = x;
        newPosition.x -= MAP_WIDTH / 2f; // The Ground is anchored at the center. If we didnt do this, we would see GameObjects only in the right half of the map.
        newPosition.x *= spreadAmount; // Accomodate for spreading out the largest resource
        newPosition.x += Random.Range(0.25f, 0.75f); // We must shift by around 0.5f to align with the "tiles" of the map. If we didnt do this we would overhang the edge GameObjects by 0.5f.
        newPosition.y = 0;
        newPosition.z = y;
        newPosition.z -= MAP_HEIGHT / 2f; // The Ground is anchored at the center. If we didnt do this, we would see GameObjects only in the top half of the map.
        newPosition.z *= spreadAmount; // Accomodate for spreading out the largest resource
        newPosition.z += Random.Range(0.25f, 0.75f); // We must shift by around 0.5f to align with the "tiles" of the map. If we didnt do this we would overhang the edge GameObjects by 0.5f.
        go.transform.position = newPosition;

        if (primArray[x, y] != null)
        {
            ClearPoint(x, y);
        }
        resourceArray[x, y] = 1;
        primArray[x, y] = go;
        yield break;
        //yield return new WaitForSeconds(0.001f);
    }
    /*IEnumerator GenerateRandomPoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            //string debugStr = "";
            //Debug.Log("Pixel Array:");
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    generatedTreeArray[i, j] = 0;
                    if (Random.Range(0, Width * Height) <= 50)
                    {
                        SpawnPrim(i, j);
                    }
                    //debugStr += pixelArray[i,j].ToString();
                    //debugStr += " ";
                }
                //Debug.Log(debugStr);
                //debugStr = "";
            }
            BeginClusterCreep();
            ClearSpawnArea();

            float hitPercentage = (totalHits * 100f) / ((float)Width * Height);
            Debug.Log("Generation complete, total Hits/size: " + totalHits + " / " + Width * Height + " = " + hitPercentage + "%");
            if (hitPercentage >= minFillPercentage)
            {
                Debug.Log("Tree " + hitPercentage + "% above " + minFillPercentage + "%, continuing...");
                break;
            }
            Debug.Log("Tree " + hitPercentage + "% below " + minFillPercentage + "%, removing and regenerating...");
            yield return new WaitForSeconds(3f);
            RemoveClusters();
        }
    }*/
    /*public void GenerateRandomPoints()
    {
        //string debugStr = "";
        //Debug.Log("Pixel Array:");
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixelArray[i, j] = 0;
                if (Random.Range(0, width * height / 10) == 0)
                {
                    SpawnPrim(i, j);
                }
                //debugStr += pixelArray[i,j].ToString();
                //debugStr += " ";
            }
            //Debug.Log(debugStr);
            //debugStr = "";
        }
        BeginClusterCreep();

        Debug.Log("Generation complete, total Hits/size: " + totalHits + " / " + width * height + " = " + (float)(totalHits*100)/(width * height) + "%");
        RemoveClusters();
    }*/
    /*void SpawnPrim(int x, int y)
{
    float compressionFactor = 10f / ((width + height) / 2f);
    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //GameObject go = Instantiate()
    go.transform.position = new Vector3(compressionFactor * (x + .5f - width / 2), compressionFactor * 0.5f, compressionFactor * (y + .5f - height / 2));
    go.transform.localScale = new Vector3(compressionFactor, compressionFactor, compressionFactor);
    go.GetComponent<MeshRenderer>().material.color = Color.black;

    // Increment pixelArray and keep track of how many we have
    pixelArray[x, y] = 1;
    totalHits++;
    primArray[x, y] = go;
}*/
    /*void SpawnPrim(int x, int y)
{
    GameObject go = Instantiate(ResourceDictionary.instance.GetPrefab("TreeAIO"));
    //GameObject go = Instantiate()
    go.transform.position = new Vector3((2.5f*x) + .5f - (2.5f * MAP_WIDTH / 2), 0.5f, (2.5f*y) + .5f - (2.5f * MAP_HEIGHT / 2));
    float randSize = Random.Range(0.75f, 1.25f);
    go.transform.localScale = new Vector3(randSize, randSize, randSize);
    //go.GetComponent<MeshRenderer>().material.color = Color.black;

    // Increment pixelArray and keep track of how many we have
    generatedTreeArray[x, y] = 1;
    totalTrees++;
    primArray[x, y] = go;
}*/
}
