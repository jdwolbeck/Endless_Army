using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksBuilding : BasicBuilding
{
    public List<GameObject> productionQueue = new List<GameObject>();
    private UIHandler uiHandler;
    private ScriptableUnit scriptableUnit;
    private float startTime;
    private float progress;
    private bool tcpbEnabled;
    private bool objIsSelected;

    protected override void Awake()
    {
        base.Awake();
        uiHandler = GameHandler.instance.GetComponent<UIHandler>();
    }
    protected override void Update()
    {
        base.Update();
        if (productionQueue.Count > 0)
        {
            progress = (Time.time - startTime) / scriptableUnit.ProductionTime;
            if (objIsSelected)
            {
                uiHandler.SetFighterProductionBar(progress);
            }
            if (progress >= 1.0f)
            {
                SpawnFighter();
            }
        }
        else if (tcpbEnabled && objIsSelected) // And Count == 0
        {
            uiHandler.DisableCreateFighterPB();
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
    public void AddFighterToQueue()
    {
        scriptableUnit = (ScriptableUnit)ResourceDictionary.instance.GetPreset("Fighter");
        if (PlayerResourceManger.instance.playerCurrentFood < scriptableUnit.FoodCost ||
            PlayerResourceManger.instance.playerCurrentWood < scriptableUnit.WoodCost ||
            PlayerResourceManger.instance.playerCurrentStone < scriptableUnit.StoneCost)
        {
            return;
        }
        PlayerResourceManger.instance.playerCurrentFood -= scriptableUnit.FoodCost;
        PlayerResourceManger.instance.playerCurrentWood -= scriptableUnit.WoodCost;
        PlayerResourceManger.instance.playerCurrentStone -= scriptableUnit.StoneCost;
        if (productionQueue.Count == 0)
        {
            uiHandler.EnableCreateFighterPB();
            tcpbEnabled = true;
        }
        if (productionQueue.Count < 30)
        {
            Debug.Log("Adding fighter to Production queue...");
            productionQueue.Add(ResourceDictionary.instance.GetPrefab("Fighter"));
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
    private void SpawnFighter()
    {
        uiHandler.SetFighterProductionBar(0);
        GameObject obj = Instantiate(productionQueue[0], transform.position - transform.forward, Quaternion.identity);
        obj.GetComponent<BasicUnit>().LoadFromPreset(scriptableUnit);
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
        if (hasRallyPoint)
        {
            obj.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(rallyPoint.transform.position);
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
