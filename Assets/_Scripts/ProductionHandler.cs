using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionHandler : MonoBehaviour
{
    public List<GameObject> productionQueue = new List<GameObject>();
    private BuildingControls buildingControls;
    private GameObject workerPrefab;
    private bool tcpbEnabled = false;
    private UIHandler uiHandler;
    private float startTime;
    private float workerProductionTime;
    private float progress = 0;
    private bool objIsSelected = false;

    private void Start()
    {
        buildingControls = GetComponent<BuildingControls>();
        workerPrefab = Resources.Load("Prefabs/Worker") as GameObject;
        uiHandler = GameHandler.instance.GetComponent<UIHandler>();
        workerProductionTime = 10.0f; // 10 Seconds to build
    }
    private void Update()
    {
        if (productionQueue.Count > 0)
        {
            progress = (Time.time - startTime) / workerProductionTime;
            if (objIsSelected)
            {
                uiHandler.SetWorkerProductionBar(progress);
            }
            if (progress >= 1.0f)
            {
                SpawnWorker();
            }
        }
        else if (tcpbEnabled && objIsSelected) // And Count == 0
        {
            uiHandler.DisableCreateWorkerPB();
            tcpbEnabled = false;
        }
    }
    public void BuildingSelectionEvent(bool isSelected)
    {
        objIsSelected = isSelected;
        if (objIsSelected)
        {
            if (tcpbEnabled)
            {
                uiHandler.EnableCreateWorkerPB();
            }
            else
            {
                uiHandler.DisableCreateWorkerPB();
            }
        }
    }
    public void AddWorkerToQueue()
    {
        Debug.Log("Adding worker to Production queue...");
        if (productionQueue.Count == 0)
        {
            uiHandler.EnableCreateWorkerPB();
            tcpbEnabled = true;
        }
        if (productionQueue.Count < 30)
        {
            productionQueue.Add(workerPrefab);
        }
        else
        {
            Debug.Log("Maximum amount of workers added to Production queue");
        }

        if (productionQueue.Count == 1)
        { // Reset the cooldown for production
            startTime = Time.time;
        }
    }
    private void SpawnWorker()
    {
        uiHandler.SetWorkerProductionBar(0);
        GameObject obj = Instantiate(productionQueue[0], transform.position - transform.forward, Quaternion.identity); 
        if (gameObject.layer == LayerMask.NameToLayer("PlayerBuildingLayer"))
        {
            GameHandler.instance.playerUnits.Add(obj);
            obj.layer = LayerMask.NameToLayer("PlayerUnitLayer");
        }
        else
        {
            GameHandler.instance.enemyUnits.Add(obj);
            obj.layer = LayerMask.NameToLayer("EnemyUnitLayer");
        }
        productionQueue.RemoveAt(0);
        startTime = Time.time;
    }
}
