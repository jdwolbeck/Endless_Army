using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    // Static variables
    public static MapGeneration instance;
    // Private variables
    private int[,] spawnAreas;
    private int totalTrees = 0;
    private int totalStones = 0;
    private int totalBushes = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    /*
     List of points for Trees (50 init):

     */
    int avgTotalTrees = 0;
    int avgTotalStones = 0;
    int avgTotalBushes = 0;
    public bool GenerateNewMap(MapGrid map)
    {
        int loopCount = 0;
        const int maxLoops = 15;
        int mapWidth = map.MapScriptable.MapWidth;
        int mapHeight = map.MapScriptable.MapHeight;
        spawnAreas = new int[mapWidth, mapHeight];

        while (loopCount < maxLoops)
        {
            GenerateLayer(map, "Tree");
            GenerateLayer(map, "Stone");
            GenerateLayer(map, "Bush");
            /*Debug.Log("Resources before clear (T, S, B): (" + map.MapScriptable.MinTreeFill + ", " + map.MapScriptable.MinStoneFill +
                ", " + map.MapScriptable.MinBushFill + ") --- (" + (float)(totalTrees * 100) / (mapWidth * mapHeight) + "(" + totalTrees + "), " + 
                (float)(totalStones * 100) / (mapWidth * mapHeight) + "(" + totalStones + "), " + (float)(totalBushes * 100) / (mapWidth * mapHeight) + "(" + totalBushes + "))");*/
            ClearSurroundingResources(map, map.CurrentBushArray);
            ClearSurroundingResources(map, map.CurrentStoneArray);
            ClearSpawnArea(map);

            avgTotalTrees += totalTrees;
            avgTotalStones += totalStones;
            avgTotalBushes += totalBushes;
            float treePercentage = (totalTrees * 100) / ((float)mapWidth * mapHeight);
            float stonePercentage = (totalStones * 100) / ((float)mapWidth * mapHeight);
            float bushPercentage = (totalBushes * 100) / ((float)mapWidth * mapHeight);
            map.UpdateCurrentMap();

            bool sufficientResources = true;
            if (treePercentage < map.MapScriptable.MinTreeFill)
                sufficientResources = false;
            if (stonePercentage < map.MapScriptable.MinStoneFill)
                sufficientResources = false;
            if (bushPercentage < map.MapScriptable.MinBushFill)
                sufficientResources = false;

            //Debug.Log("Generated Resources (T, S, B): (" + map.MapScriptable.MinTreeFill + ", " + map.MapScriptable.MinStoneFill +
                //", " + map.MapScriptable.MinBushFill + ") --- (" + treePercentage + ", " + stonePercentage + ", " + bushPercentage + ")");

            if (sufficientResources)
            {
                totalTrees = 0;
                totalStones = 0;
                totalBushes = 0;
                //break;
            }

            ClearAllPoints(map);
            totalTrees = 0;
            totalStones = 0;
            totalBushes = 0;
            loopCount++;
        }
        Debug.Log("Generated Resources averages over " + loopCount + " counts (T, S, B): (" + map.MapScriptable.MinTreeFill + ", " + map.MapScriptable.MinStoneFill +
                ", " + map.MapScriptable.MinBushFill + ") --- (" + (((avgTotalTrees / (float)loopCount) * 100f) / ((float)mapWidth * mapHeight)) + ", " + 
                (((avgTotalStones / (float)loopCount) * 100f) / ((float)mapWidth * mapHeight)) + ", " + (((avgTotalBushes / (float)loopCount) * 100f) / ((float)mapWidth * mapHeight)) + ")");
        avgTotalTrees = 0;
        avgTotalStones = 0;
        avgTotalBushes = 0;
        if (loopCount == maxLoops)
        {
            //Debug.Log("We looped " + maxLoops + " times and still resulted in no Map, modify generation algorithm/coverage requirements??");
            return false;
        }
        return true;
    }
    void GenerateLayer(MapGrid map, string resourceType)
    {
        int initialPoints;
        int[,] resourceArray;
        switch (resourceType)
        { // Set how many starting resources we want to spawn. From which will be each resource cluster
            case "Tree":
                initialPoints = map.MapScriptable.InitialTreePoints;
                //initialPoints = map.MapScriptable.MapWidth / 2;
                resourceArray = map.CurrentTreeArray;
                break;
            case "Stone":
                initialPoints = map.MapScriptable.InitialStonePoints;
                //initialPoints = (int)(map.MapScriptable.MapWidth / 3.5f);
                resourceArray = map.CurrentStoneArray;
                break;
            case "Bush":
                initialPoints = map.MapScriptable.InitialBushPoints;
                //initialPoints = map.MapScriptable.MapWidth / 4;
                resourceArray = map.CurrentBushArray;
                break;
            default:
                Debug.Log("Invalid string passed into GenerateLayer: " + resourceType==null ? "NULL" : resourceType);
                return;
        }
        for (int i = 0; i < map.MapScriptable.MapWidth; i++)
        {
            for (int j = 0; j < map.MapScriptable.MapHeight; j++)
            {
                resourceArray[i, j] = 0;
                if (Random.Range(0, map.MapScriptable.MapWidth * map.MapScriptable.MapHeight) <= initialPoints)
                {
                    //SpawnResource(i, j, resourceArray, resourceType);
                    SaveResource(map, i, j, resourceArray, resourceType);
                }
            }
        }
        BeginClusterCreep(map, resourceArray, resourceType);
    }
    void BeginClusterCreep(MapGrid map, int[,] resourceArray, string resourceType)
    {
        //int[,] startingPoints = new int[map.MapScriptable.MapWidth, map.MapScriptable.MapHeight];
        List<Vector2> startingPoints = new List<Vector2>();
        for (int i = 0; i < map.MapScriptable.MapWidth; i++)
        {
            for (int j = 0; j < map.MapScriptable.MapHeight; j++)
            {
                //startingPoints[i, j] = resourceArray[i, j];
                if (resourceArray[i, j] == 1)
                {
                    startingPoints.Add(new Vector2(i, j));
                }
            }
        }
        /*
        for (int i = 0; i < map.MapScriptable.MapWidth; i++)
        {
            for (int j = 0; j < map.MapScriptable.MapHeight; j++)
            {
                if (startingPoints[i, j] == 1)
                {
                    CreepPoint(new Vector2(i, j), 0, map, resourceArray, resourceType);
                }
            }
        } 
        */
        foreach (Vector2 point in startingPoints)
        {
            CreepPoint(point, 0, map, resourceArray, resourceType);
        }
    }

    private float calculateM(float percentCoverage, string resourceType)
    {
        switch (resourceType)
        {
            case "Tree":
                return 10 - 8.22839f * Mathf.Pow(percentCoverage, 0.0425597f);
            case "Stone":
                return 8 - 4.69002f * Mathf.Pow(percentCoverage, 0.116229f);
            case "Bush":
                return 10 - 1.51237f * Mathf.Pow(percentCoverage, 0.410389f);
        }

        return 10;
    }
    void CreepPoint(Vector2 parentPoint, int generation, MapGrid map, int[,] resourceArray, string resourceType)
    {
        int randChance = 0;
        float dampeningFactor = 0;
        switch (resourceType)
        {
            case "Tree":
                dampeningFactor = generation * map.MapScriptable.TreeDampeningFactor;
                //dampeningFactor = generation * (map.MapScriptable.MinTreeFill / 60);
                //dampeningFactor = generation * calculateM(map.MapScriptable.MinTreeFill, resourceType);
                break;
            case "Stone":
                dampeningFactor = generation * map.MapScriptable.StoneDampeningFactor;
                //dampeningFactor = generation * (map.MapScriptable.MinStoneFill * 2);
                //dampeningFactor = generation * calculateM(map.MapScriptable.MinStoneFill, resourceType);
                break;
            case "Bush":
                dampeningFactor = generation * map.MapScriptable.BushDampeningFactor;
                //dampeningFactor = generation * (map.MapScriptable.MinBushFill * 3);
                //dampeningFactor = generation * calculateM(map.MapScriptable.MinBushFill, resourceType);
                break;
            default:
                dampeningFactor = 10f;
                break;
        }
        if (parentPoint.x - 1 >= 0 && resourceArray[(int)parentPoint.x - 1, (int)parentPoint.y] == 0)
        { // Check Left
            if (Random.Range(0f, 10f) >= randChance + dampeningFactor)
            {
                //SpawnResource((int)parentPoint.x - 1, (int)parentPoint.y, resourceArray, resourceType);
                SaveResource(map, (int)parentPoint.x - 1, (int)parentPoint.y, resourceArray, resourceType);
                CreepPoint(new Vector2(parentPoint.x - 1, parentPoint.y), generation + 1, map, resourceArray, resourceType);
            }
        }
        if (parentPoint.x + 1 < map.MapScriptable.MapWidth && resourceArray[(int)parentPoint.x + 1, (int)parentPoint.y] == 0)
        { // Check Right
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                //SpawnResource((int)parentPoint.x + 1, (int)parentPoint.y, resourceArray, resourceType);
                SaveResource(map, (int)parentPoint.x + 1, (int)parentPoint.y, resourceArray, resourceType);
                CreepPoint(new Vector2(parentPoint.x + 1, parentPoint.y), generation + 1, map, resourceArray, resourceType);
            }
        }
        if (parentPoint.y + 1 < map.MapScriptable.MapHeight && resourceArray[(int)parentPoint.x, (int)parentPoint.y + 1] == 0)
        { // Check Up
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                //SpawnResource((int)parentPoint.x, (int)parentPoint.y + 1, resourceArray, resourceType);
                SaveResource(map, (int)parentPoint.x, (int)parentPoint.y + 1, resourceArray, resourceType);
                CreepPoint(new Vector2(parentPoint.x, parentPoint.y + 1), generation + 1, map, resourceArray, resourceType);
            }
        }
        if (parentPoint.y - 1 >= 0 && resourceArray[(int)parentPoint.x, (int)parentPoint.y - 1] == 0)
        { // Check Down
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                //SpawnResource((int)parentPoint.x, (int)parentPoint.y - 1, resourceArray, resourceType);
                SaveResource(map, (int)parentPoint.x, (int)parentPoint.y - 1, resourceArray, resourceType);
                CreepPoint(new Vector2(parentPoint.x, parentPoint.y - 1), generation + 1, map, resourceArray, resourceType);
            }
        }
    }
    void ClearAllPoints(MapGrid map)
    {
        for (int i = 0; i < map.MapScriptable.MapWidth; i++)
        {
            for (int j = 0; j < map.MapScriptable.MapHeight; j++)
            {
                ClearPoint(map, i, j);
                if (map.CurrentGameObjectArray[i, j] != null)
                    Destroy(map.CurrentGameObjectArray[i, j]);
            }
        }
    }
    void ClearPoint(MapGrid map, int x, int y)
    {
        if (map.CurrentTreeArray[x, y] == 1)
            totalTrees--;
        if (map.CurrentStoneArray[x, y] == 1)
            totalStones--;
        if (map.CurrentBushArray[x, y] == 1)
            totalBushes--;

        map.CurrentTreeArray[x, y] = 0;
        map.CurrentStoneArray[x, y] = 0;
        map.CurrentBushArray[x, y] = 0;
        map.CurrentBasicObjectArray[x, y] = "";
    }
    void ClearSpawnArea(MapGrid map)
    {
        for (int i = 0; i < map.MapScriptable.MapWidth; i++)
        {
            for (int j = 0; j < map.MapScriptable.MapHeight; j++)
            {
                spawnAreas[i, j] = 0;
                // Check to see if each point is within the spawn area's radius
                for (int k = 0; k < map.MapScriptable.NumberOfTeams; k++)
                {
                    if ((Mathf.Pow(i - map.TeamSpawns[k].x, 2) + Mathf.Pow(j - map.TeamSpawns[k].y, 2) <= Mathf.Pow(map.MapScriptable.SpawnRadius, 2))) //(x-i)^2 + (y-j)^2 <= r^2
                    {
                        spawnAreas[i, j] = 1;
                        ClearPoint(map, i, j);
                    }
                }
            }
        }
    }
    void ClearSurroundingResources(MapGrid map, int[,] resourceArray)
    {
        for (int i = 0; i < map.MapScriptable.MapWidth; i++)
        {
            for (int j = 0; j < map.MapScriptable.MapHeight; j++)
            {
                if (resourceArray[i, j] == 1)
                {
                    if (i - 1 >= 0 && resourceArray[i - 1, j] == 0)
                    {
                        ClearPoint(map, i - 1, j);
                    }
                    if (i + 1 < map.MapScriptable.MapWidth && resourceArray[i + 1, j] == 0)
                    {
                        ClearPoint(map, i + 1, j);
                    }
                    if (j - 1 >= 0 && resourceArray[i, j - 1] == 0)
                    {
                        ClearPoint(map, i, j - 1);
                    }
                    if (j + 1 < map.MapScriptable.MapHeight && resourceArray[i, j + 1] == 0)
                    {
                        ClearPoint(map, i, j + 1);
                    }
                }
            }
        }
    }
    void SaveResource(MapGrid map, int x, int y, int[,] resourceArray, string resourceType)
    {
        switch (resourceType)
        {
            case "Tree":
                totalTrees++;
                break;
            case "Stone":
                totalStones++;
                break;
            case "Bush":
                totalBushes++;
                break;
        }

        if (map.CurrentTreeArray[x, y] == 1 ||
            map.CurrentStoneArray[x, y] == 1 ||
            map.CurrentBushArray[x, y] == 1)
        {
            ClearPoint(map, x, y);
        }
        resourceArray[x, y] = 1;
    }
}