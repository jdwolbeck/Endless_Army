using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TownCenterBuilding : BasicBuilding
{
    public List<GameObject> productionQueue = new List<GameObject>();
    private UIHandler uiHandler;
    private ScriptableUnit scriptableUnit;
    private float startTime;
    private float lastTickTime;
    private float haltedTickTime;
    private float progress;
    private bool tcpbEnabled;
    private bool objIsSelected;

    protected override void Awake()
    {
        base.Awake();
        maxOperators = 5;
        currentOperators = maxOperators;
    }
    protected override void Start()
    {
        base.Start();
        uiHandler = UIHandler.instance.GetComponent<UIHandler>();
        LoadFromPreset((ScriptableBuilding)ResourceDictionary.instance.GetPreset("TownCenter"));
    }
    protected override void Update()
    {
        base.Update();
        if (productionQueue.Count > 0)
        {
            if (currentOperators > 0)
            {
                if (haltedTickTime > lastTickTime)
                {
                    // If we have haulted for a while (while we were on fire and have since been repaired)
                    // Modify the start time to adjust for the time while we were on fire, so we pick back up where we left off.
                    startTime += haltedTickTime - lastTickTime;
                }
                progress = (Time.time - startTime) / scriptableUnit.ProductionTime;
                if (objIsSelected)
                {
                    uiHandler.SetWorkerProductionBar(progress);
                }
                if (progress >= 1.0f)
                {
                    SpawnWorker();
                }
                // The last time we had enough operators to keep spawning this fighter
                lastTickTime = Time.time;
            }
            else // The last time in which we were unable to keep spawning this fighter
                haltedTickTime = Time.time;
        }
        else if (tcpbEnabled && objIsSelected) // And Count == 0
        {
            uiHandler.DisableCreateWorkerPB();
            tcpbEnabled = false;
        }
    }
    public override void SelectObject()
    {
        base.SelectObject();
        BuildingSelectionEvent(true);
    }
    public override void DeselectObject()
    {
        base.DeselectObject();
       BuildingSelectionEvent(false);
    }
    public void AddWorkerToQueue()
    {
        scriptableUnit = (ScriptableUnit)ResourceDictionary.instance.GetPreset("Worker");
        if (TeamManager.instance.teamList[Team].teamCurrentFood < scriptableUnit.FoodCost ||
            TeamManager.instance.teamList[Team].teamCurrentWood < scriptableUnit.WoodCost ||
            TeamManager.instance.teamList[Team].teamCurrentStone < scriptableUnit.StoneCost)
        {
            return;
        }
        TeamManager.instance.teamList[Team].teamCurrentFood -= scriptableUnit.FoodCost;
        TeamManager.instance.teamList[Team].teamCurrentWood -= scriptableUnit.WoodCost;
        TeamManager.instance.teamList[Team].teamCurrentStone -= scriptableUnit.StoneCost;
        if (Team == 0 && productionQueue.Count == 0)
        {
            uiHandler.EnableCreateWorkerPB();
            tcpbEnabled = true;
        }
        if (productionQueue.Count < 30)
        {
            //Debug.Log("Adding worker to Production queue...");
            productionQueue.Add(ResourceDictionary.instance.GetPrefab("Blend_WorkerUnit"));
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
        //obj.GetComponent<BasicUnit>().LoadFromPreset(scriptableUnit);
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
        obj.GetComponent<WorkerUnit>().Team = Team;
        productionQueue.RemoveAt(0);
        if (hasRallyPoint)
        {
            obj.GetComponent<NavMeshAgent>().SetDestination(rallyPoint.transform.position);
        }
        startTime = Time.time;
    }
    public void BuildingSelectionEvent(bool isSelected)
    {
        objIsSelected = isSelected;
        if (objIsSelected)
        {
            if (tcpbEnabled)
            {
                uiHandler.EnableCreateFighterPB();
            }
            else
            {
                uiHandler.DisableCreateFighterPB();
            }
        }
    }
}
