using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
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
    }
    private void Start()
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
}
