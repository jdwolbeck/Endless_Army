using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
    public List<GameObject> playerUnits;
    public List<GameObject> enemyUnits = new List<GameObject>();
    public List<GameObject> neutralUnits = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        playerUnits = new List<GameObject>();
    }
    private void Start()
    {
        // Go through and populate our lists with the existing GameObjects in the scene
        GameObject[] goList = FindObjectsOfType<GameObject>();
        foreach (GameObject go in goList)
        {
            if (go.layer == LayerMask.NameToLayer("PlayerLayer"))
            {
                playerUnits.Add(go);
            }
            else if (go.layer == LayerMask.NameToLayer("EnemyLayer"))
            {
                enemyUnits.Add(go);
            }
            if (go.layer == LayerMask.NameToLayer("NeutralLayer"))
            {
                neutralUnits.Add(go);
            }
        }
    }
}
