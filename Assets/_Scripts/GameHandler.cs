using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
    public ScriptableMap Map;
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
    }
    private void Start()
    {
        BuildObjectLists();
        LoadMap(Map);
    }
    void LoadMap(ScriptableMap map)
    {
        UnloadCurrentMap();
        PlayerResourceManger.instance.playerCurrentFood = map.StartingFoodAmount;
        PlayerResourceManger.instance.playerCurrentWood = map.StartingWoodAmount;
        PlayerResourceManger.instance.playerCurrentStone = map.StartingStoneAmount;
        ResourceDictionary.instance.GetMaterial("GroundMat").color = map.groundColor;
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
