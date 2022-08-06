using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
    public ScriptableMap Map;
    public GameObject Ground;
    private Material groundMat;
    public List<GameObject> playerUnits;
    public List<GameObject> playerBuildings;
    public List<GameObject> enemyUnits;
    public List<GameObject> enemyBuildings;
    public List<GameObject> neutralUnits;
    public List<GameObject> resourceObjects;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        playerUnits = new List<GameObject>();
        playerBuildings = new List<GameObject>();
        enemyUnits = new List<GameObject>();
        enemyBuildings = new List<GameObject>();
        neutralUnits = new List<GameObject>();
        resourceObjects = new List<GameObject>();
        Map = Resources.Load<ScriptableMap>("Presets/MapPresets/MapTest");
        groundMat = Resources.Load<Material>("Materials/GroundMat");
    }
    private void Start()
    {
        BuildObjectLists();
        LoadMap(Map);
        GenerateRandomPoints();
    }

    const int width = 100;
    const int height = 100;
    int[,] pixelArray = new int[width, height];
    void GenerateRandomPoints()
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
                    pixelArray[i, j] = 1;
                }
                //debugStr += pixelArray[i,j].ToString();
                //debugStr += " ";

                if (pixelArray[i,j] == 1)
                {
                    SpawnPrim(i, j);
                }
            }
            //Debug.Log(debugStr);
            //debugStr = "";
        }
        BeginClusterCreep();
    }
    void BeginClusterCreep()
    {
        int[,] startingPoints = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                startingPoints[i, j] = pixelArray[i, j];
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (startingPoints[i,j] == 1)
                {
                    Debug.Log("Starting cluster creep on point: (" + i + ", " + j + ")");
                    CreepPoint(new Vector2(i, j), 0);
                }
            }
        }
    }
    void CreepPoint(Vector2 parentPoint, int generation)
    {
        int randChance = 0;
        // Check Left
        if (parentPoint.x - 1 >= 0 && pixelArray[(int)parentPoint.x - 1, (int)parentPoint.y] == 0)
        {
            if (Random.Range(0, 10) >= randChance + (generation / 2))
            {
                pixelArray[(int)parentPoint.x - 1, (int)parentPoint.y] = 1;
                SpawnPrim((int)parentPoint.x - 1, (int)parentPoint.y);
                CreepPoint(new Vector2(parentPoint.x - 1, parentPoint.y), generation+1);
            }
        }
        // Check Right
        if (parentPoint.x + 1 < width && pixelArray[(int)parentPoint.x + 1, (int)parentPoint.y] == 0)
        {
            if (Random.Range(0, 10) >= randChance + (generation / 2))
            {
                pixelArray[(int)parentPoint.x + 1, (int)parentPoint.y] = 1;
                SpawnPrim((int)parentPoint.x + 1, (int)parentPoint.y);
                CreepPoint(new Vector2(parentPoint.x + 1, parentPoint.y), generation + 1);
            }
        }
        // Check Up
        if (parentPoint.y + 1 < height && pixelArray[(int)parentPoint.x, (int)parentPoint.y + 1] == 0)
        {
            if (Random.Range(0, 10) >= randChance + (generation / 2))
            {
                pixelArray[(int)parentPoint.x, (int)parentPoint.y + 1] = 1;
                SpawnPrim((int)parentPoint.x, (int)parentPoint.y + 1);
                CreepPoint(new Vector2(parentPoint.x, parentPoint.y + 1), generation + 1);
            }
        }
        // Check Down
        if (parentPoint.y - 1 >= 0 && pixelArray[(int)parentPoint.x, (int)parentPoint.y - 1] == 0)
        {
            if (Random.Range(0, 10) >= randChance + (generation / 2))
            {
                pixelArray[(int)parentPoint.x, (int)parentPoint.y - 1] = 1;
                SpawnPrim((int)parentPoint.x, (int)parentPoint.y - 1);
                CreepPoint(new Vector2(parentPoint.x, parentPoint.y - 1), generation + 1);
            }
        }
        if (generation > 6)
        Debug.Log("Generation " + generation);
    }
    void SpawnPrim(int x, int y)
    {
        float compressionFactor = 10f / ((width + height) / 2f);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(compressionFactor * (x + .5f - width / 2), compressionFactor * 0.5f, compressionFactor * (y + .5f - height / 2));
        go.transform.localScale = new Vector3(compressionFactor, compressionFactor, compressionFactor);
        go.GetComponent<MeshRenderer>().material.color = Color.black;
    }
    void LoadMap(ScriptableMap map)
    {
        UnloadCurrentMap();
        PlayerResourceManger.instance.playerCurrentFood = map.StartingFoodAmount;
        PlayerResourceManger.instance.playerCurrentWood = map.StartingWoodAmount;
        PlayerResourceManger.instance.playerCurrentStone = map.StartingStoneAmount;
        groundMat.color = map.groundColor;
        Ground.transform.localScale = Vector3.one;
        Debug.Log("New map has been loaded");
    }
    void UnloadCurrentMap()
    {
        RemoveAllObjects();
    }
    void BuildObjectLists()
    {
        // Go through and populate our lists with the existing GameObjects in the scene
        GameObject[] goList = FindObjectsOfType<GameObject>();
        foreach (GameObject go in goList)
        {
            if (go.gameObject.transform.parent == null)
            {
                if (go.layer == LayerMask.NameToLayer("PlayerUnitLayer"))
                {
                    playerUnits.Add(go);
                }
                else if (go.layer == LayerMask.NameToLayer("PlayerBuildingLayer"))
                {
                    playerBuildings.Add(go);
                }
                else if (go.layer == LayerMask.NameToLayer("EnemyUnitLayer"))
                {
                    enemyUnits.Add(go);
                }
                else if (go.layer == LayerMask.NameToLayer("EnemyBuildingLayer"))
                {
                    enemyBuildings.Add(go);
                }
                else if (go.layer == LayerMask.NameToLayer("NeutralLayer"))
                {
                    neutralUnits.Add(go);
                }
                else if (go.layer == LayerMask.NameToLayer("ResourceLayer"))
                {
                    resourceObjects.Add(go);
                }
            }
        }
    }
    void RemoveAllObjects()
    {
        // Go through all lists and delete the objects from the scene.
        GameObject go;
        for (int i = playerUnits.Count - 1; i >= 0; i--)
        {
            go = playerUnits[i];
            playerUnits.RemoveAt(i);
            Destroy(go);
        }
        for (int i = playerBuildings.Count - 1; i >= 0; i--)
        {
            go = playerBuildings[i];
            playerBuildings.RemoveAt(i);
            Destroy(go);
        }
        for (int i = enemyUnits.Count - 1; i >= 0; i--)
        {
            go = enemyUnits[i];
            enemyUnits.RemoveAt(i);
            Destroy(go);
        }
        for (int i = enemyBuildings.Count - 1; i >= 0; i--)
        {
            go = enemyBuildings[i];
            enemyBuildings.RemoveAt(i);
            Destroy(go);
        }
        for (int i = neutralUnits.Count - 1; i >= 0; i--)
        {
            go = neutralUnits[i];
            neutralUnits.RemoveAt(i);
            Destroy(go);
        }
        for (int i = resourceObjects.Count - 1; i >= 0; i--)
        {
            go = resourceObjects[i];
            resourceObjects.RemoveAt(i);
            Destroy(go);
        }
    }
}
