using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public enum MapObjectEnum
{
    None,
    Tree,
    Stone,
    Bush,
    EnemyTownCenter,
    EnemyWorker,
    EnemyFighter,
    PlayerTownCenter,
    PlayerWorker,
    PlayerFighter
}

public class MapGrid
{
    // Public variables
    public ScriptableMap MapScriptable;
    public int[,] CurrentTreeArray;
    public int[,] CurrentStoneArray;
    public int[,] CurrentBushArray;
    public string[,] CurrentBasicObjectArray;
    public GameObject[,] CurrentGameObjectArray;
    public MapObjectEnum[,] CurrentGameMap;
    public List<Vector2> TeamSpawns;
    public MapTypeEnum MapType;

    public MapGrid()
    {
        MapScriptable = (ScriptableMap)ResourceDictionary.instance.GetPreset("MapTest");
        CurrentTreeArray = new int[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentStoneArray = new int[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentBushArray = new int[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentBasicObjectArray = new string[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentGameObjectArray = new GameObject[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentGameMap = new MapObjectEnum[MapScriptable.MapWidth, MapScriptable.MapHeight];

        // Used for determining spawn points
        float spawnDegrees;
        float degreesPerPlayer = 360.0f / MapScriptable.NumberOfTeams;
        float spawnRadius = (MapScriptable.MapWidth / 2f) * 0.8f;
        float x;
        float y;
        TeamSpawns = new List<Vector2>();
        for (int i = 0; i < MapScriptable.NumberOfTeams; i++)
        {
            spawnDegrees = degreesPerPlayer * i; // number of degrees around the map in which this team will spawn
            // Use triginometry to figure out the x, y coordinates for this player spawn.
            if (spawnDegrees <= 90)
            {
                x = spawnRadius * Mathf.Cos(Mathf.Deg2Rad * spawnDegrees);
                y = spawnRadius * Mathf.Sin(Mathf.Deg2Rad * spawnDegrees);
            }
            else if (spawnDegrees <= 180)
            {
                spawnDegrees = 180 - spawnDegrees;
                x = spawnRadius * Mathf.Cos(Mathf.Deg2Rad * spawnDegrees);
                x *= -1;
                y = spawnRadius * Mathf.Sin(Mathf.Deg2Rad * spawnDegrees);
            }
            else if (spawnDegrees <= 270)
            {
                spawnDegrees = spawnDegrees - 180; 
                x = spawnRadius * Mathf.Cos(Mathf.Deg2Rad * spawnDegrees);
                x *= -1;
                y = spawnRadius * Mathf.Sin(Mathf.Deg2Rad * spawnDegrees);
                y *= -1;
            }
            else if (spawnDegrees <= 360)
            {
                spawnDegrees = 360 - spawnDegrees;
                x = spawnRadius * Mathf.Cos(Mathf.Deg2Rad * spawnDegrees);
                y = spawnRadius * Mathf.Sin(Mathf.Deg2Rad * spawnDegrees);
                y *= -1;
            }
            else
            {
                spawnDegrees = spawnDegrees - 360;
                x = spawnRadius * Mathf.Cos(Mathf.Deg2Rad * spawnDegrees);
                y = spawnRadius * Mathf.Sin(Mathf.Deg2Rad * spawnDegrees);
            }
            // Since our coordinate system used for instatiation is set to the bottom left and the circle equations are set for the center of the map offset the calculated x,y
            x += (MapScriptable.MapWidth / 2);
            y += MapScriptable.MapHeight / 2;
            TeamSpawns.Add(new Vector2(x, y));
        }
    }
    public void UpdateCurrentMap()
    {
        for (int i = 0; i < MapScriptable.MapWidth; i++)
        {
            for (int j = 0; j < MapScriptable.MapHeight; j++)
            {
                if (CurrentTreeArray[i, j] == 1)
                    CurrentGameMap[i, j] = MapObjectEnum.Tree;
                if (CurrentStoneArray[i, j] == 1)
                    CurrentGameMap[i, j] = MapObjectEnum.Stone;
                if (CurrentBushArray[i, j] == 1)
                    CurrentGameMap[i, j] = MapObjectEnum.Bush;
                if (CurrentBasicObjectArray[i, j] != "")
                {
                    switch (CurrentBasicObjectArray[i, j])
                    {
                        case "EnemyTownCenterAIO":
                            CurrentGameMap[i, j] = MapObjectEnum.EnemyTownCenter;
                            break;
                        case "EnemyWorker":
                            CurrentGameMap[i, j] = MapObjectEnum.EnemyWorker;
                            break;
                        case "EnemyFighter":
                            CurrentGameMap[i, j] = MapObjectEnum.EnemyFighter;
                            break;
                        case "PlayerTownCenterAIO":
                            CurrentGameMap[i, j] = MapObjectEnum.PlayerTownCenter;
                            break;
                        case "PlayerWorker":
                            CurrentGameMap[i, j] = MapObjectEnum.PlayerWorker;
                            break;
                        case "PlayerFighter":
                            CurrentGameMap[i, j] = MapObjectEnum.PlayerFighter;
                            break;
                    }

                }
            }
        }
    }
    public void InstantiateMap()
    {
        Vector3 newScale = Vector3.zero;
        newScale.x = MapScriptable.MapWidth / 10; // Planes are 10x10 Primative Cubes by default.
        newScale.x *= MapScriptable.ResourceSpreadFactor; // Expand to the largest tile spread size of the resources.
        newScale.y = 1; // Important to set this to 1 so we dont lose the color.
        newScale.z = MapScriptable.MapHeight / 10; // Planes are 10x10 Primative Cubes by default.
        newScale.z *= MapScriptable.ResourceSpreadFactor; // Expand to the largest tile spread size of the resources.
        MapManager.instance.Ground.transform.localScale = newScale;//new Vector3(2 * MAP_WIDTH / 10, 1, 2 * MAP_HEIGHT / 10);
        NavMeshSurface navSurface = MapManager.instance.Ground.GetComponent<NavMeshSurface>();
        navSurface.layerMask = LayerMask.GetMask("GroundLayer");
        navSurface.BuildNavMesh();

        for (int i = 0; i < MapScriptable.MapWidth; i++)
        {
            for (int j = 0; j < MapScriptable.MapHeight; j++)
            {
                SpawnResource(i, j);
            }
        }
    }
    private void SpawnResource(int x, int y)
    {
        float randSize = 1f;
        float randomRotation = Random.Range(0, 360);
        string resourceType = "";
        if (CurrentTreeArray[x, y] == 1)
        {
            randSize = Random.Range(0.75f, 1.25f);
            resourceType = "Tree";
        }
        if (CurrentStoneArray[x, y] == 1)
        {
            randSize = Random.Range(1.25f, 1.75f);
            resourceType = "Stone";
        }
        if (CurrentBushArray[x, y] == 1)
        {
            randSize = Random.Range(2f, 2.5f);
            resourceType = "Bush";
        }

        if (resourceType == "")
            return;

        GameObject go = GameObject.Instantiate(ResourceDictionary.instance.GetPrefab(resourceType + "AIO"));
        go.transform.rotation = Quaternion.Euler(0, randomRotation, 0);
        go.transform.localScale = new Vector3(randSize, randSize, randSize);
        Vector2 newPosition = TranslateCoordinatesToGameWorld(MapScriptable, new Vector2(x, y));
        go.transform.position = new Vector3(newPosition.x, 0, newPosition.y);

        CurrentGameObjectArray[x, y] = go;
        GameHandler.instance.resourceObjects.Add(go);
    }
    private void InstantiatePlayerSpawns()
    {
        Vector2 gamePosition = new Vector2();
        for (int i = 0; i < TeamSpawns.Count; i++)
        {
            gamePosition = TranslateCoordinatesToGameWorld(MapScriptable, TeamSpawns[i]);
            if (i == 0)
            {
                GameObject go = GameObject.Instantiate(ResourceDictionary.instance.GetPrefab("TownCenterEGO"), new Vector3(gamePosition.x, 0, gamePosition.y), Quaternion.identity);
                go.layer = LayerMask.NameToLayer("PlayerBuildingLayer");
                GameHandler.instance.playerBuildings.Add(go);
                go.GetComponent<BasicBuilding>().FinishBuilding();
            }
            else
            {
                GameObject go = GameObject.Instantiate(ResourceDictionary.instance.GetPrefab("TownCenterEGO"), new Vector3(gamePosition.x, 0, gamePosition.y), Quaternion.identity);
                go.layer = LayerMask.NameToLayer("EnemyBuildingLayer");
                GameHandler.instance.enemyBuildings.Add(go);
                go.GetComponent<BasicBuilding>().FinishBuilding();
            }
        }
    }
    public Vector2 TranslateCoordinatesToGameWorld(ScriptableMap map, Vector2 inputCoords)
    {
        Vector2 outputCoords = new Vector2();
        outputCoords.x = inputCoords.x;
        outputCoords.x -= map.MapWidth / 2f; // The Ground is anchored at the center. If we didnt do this, we would see GameObjects only in the right half of the map.
        outputCoords.x *= map.ResourceSpreadFactor; // Accomodate for spreading out the largest resource
        outputCoords.x += Random.Range(0.25f, 0.75f); // We must shift by around 0.5f to align with the "tiles" of the map. If we didnt do this we would overhang the edge GameObjects by 0.5f.
        outputCoords.y = inputCoords.y;
        outputCoords.y -= map.MapHeight / 2f; // The Ground is anchored at the center. If we didnt do this, we would see GameObjects only in the top half of the map.
        outputCoords.y *= map.ResourceSpreadFactor; // Accomodate for spreading out the largest resource
        outputCoords.y += Random.Range(0.25f, 0.75f); // We must shift by around 0.5f to align with the "tiles" of the map. If we didnt do this we would overhang the edge GameObjects by 0.5f.

        return outputCoords;
    }
}
