using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public MapGrid()
    {
        MapScriptable = (ScriptableMap)ResourceDictionary.instance.GetPreset("MapTest");
        CurrentTreeArray = new int[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentStoneArray = new int[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentBushArray = new int[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentBasicObjectArray = new string[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentGameObjectArray = new GameObject[MapScriptable.MapWidth, MapScriptable.MapHeight];
        CurrentGameMap = new MapObjectEnum[MapScriptable.MapWidth, MapScriptable.MapHeight];
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
        Vector3 newPosition = Vector3.zero;
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
        newPosition.x = x;
        newPosition.x -= MapScriptable.MapWidth / 2f; // The Ground is anchored at the center. If we didnt do this, we would see GameObjects only in the right half of the map.
        newPosition.x *= MapScriptable.ResourceSpreadFactor; // Accomodate for spreading out the largest resource
        newPosition.x += Random.Range(0.25f, 0.75f); // We must shift by around 0.5f to align with the "tiles" of the map. If we didnt do this we would overhang the edge GameObjects by 0.5f.
        newPosition.y = 0;
        newPosition.z = y;
        newPosition.z -= MapScriptable.MapHeight / 2f; // The Ground is anchored at the center. If we didnt do this, we would see GameObjects only in the top half of the map.
        newPosition.z *= MapScriptable.ResourceSpreadFactor; // Accomodate for spreading out the largest resource
        newPosition.z += Random.Range(0.25f, 0.75f); // We must shift by around 0.5f to align with the "tiles" of the map. If we didnt do this we would overhang the edge GameObjects by 0.5f.
        go.transform.position = newPosition;

        CurrentGameObjectArray[x, y] = go;
        GameHandler.instance.resourceObjects.Add(go);
    }
}
