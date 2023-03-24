using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWorkerUnit : AIBasicUnit
{
    //private float waitUntil;
    /*protected override void Update()
    {
        base.Update();
        if (Time.time > waitUntil)
        {

        }
        waitUntil += 0.083f; // 5/60 only call this at a bare minimum of 5 times every second.
    }*/
    public bool BuildBuildingNearby(BuildableObjectList buildObj)
    {
        string prefabStr = string.Empty;
        ScriptableBuilding scriptableBuilding = (ScriptableBuilding)ResourceDictionary.instance.GetPreset("TownCenter");
        switch (buildObj)
        {
            case (BuildableObjectList.TownCenter):
                Debug.Log("BuildBuildingNearby(TC)");
                if (TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentFood >= scriptableBuilding.FoodCost &&
                    TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentWood >= scriptableBuilding.WoodCost &&
                    TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentStone >= scriptableBuilding.StoneCost)
                {
                    Debug.Log("We have enough resources.");
                    prefabStr = "TownCenterEGO";
                }
                break;
            case (BuildableObjectList.Barracks):
                if (TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentFood >= scriptableBuilding.FoodCost &&
                    TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentWood >= scriptableBuilding.WoodCost &&
                    TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentStone >= scriptableBuilding.StoneCost)
                {
                    prefabStr = "BarracksEGO";
                }
                break;
        }
        if (prefabStr != string.Empty)
        {
            GameObject blueprint = Instantiate(ResourceDictionary.instance.GetPrefab("TownCenterEGO"), new Vector3(transform.position.x, 0, transform.position.y), Quaternion.identity);
            BasicBuilding basicBuild = blueprint.GetComponent<BasicBuilding>();
            blueprint.AddComponent<Rigidbody>().useGravity = false;
            blueprint.layer = LayerMask.NameToLayer("EnemyBuildingLayer");
            basicBuild.Team = GetComponent<WorkerUnit>().Team;
            bool foundBuildLocation = false;
            //blueprint.SetActive(false);

            Vector2 buildingOffset = Vector2.zero;
            while (buildingOffset.x < 10f && !foundBuildLocation)
            { // Try to build off to the right
                blueprint.transform.position = new Vector3(transform.position.x + buildingOffset.x, transform.position.y, transform.position.z + buildingOffset.y);
                if (basicBuild.canBuildHere)
                {
                    Debug.Log("AI found a build location off to the right (" + buildingOffset.x + ")");
                    foundBuildLocation = true;
                    break;
                }
                buildingOffset.x += 1f;
            }
            buildingOffset = Vector2.zero;
            while (buildingOffset.x > -10f && !foundBuildLocation)
            { // Try to build off to the left
                blueprint.transform.position = new Vector3(transform.position.x + buildingOffset.x, transform.position.y, transform.position.z + buildingOffset.y);
                if (basicBuild.canBuildHere)
                {
                    Debug.Log("AI found a build location off to the left (" + buildingOffset.x + ")");
                    foundBuildLocation = true;
                    break;
                }
                buildingOffset.x -= 1f;
            }
            buildingOffset = Vector2.zero;
            while (buildingOffset.y < 10f && !foundBuildLocation)
            { // Try to build upwards
                blueprint.transform.position = new Vector3(transform.position.x + buildingOffset.x, transform.position.y, transform.position.z + buildingOffset.y);
                if (basicBuild.canBuildHere)
                {
                    Debug.Log("AI found a build location upward (" + buildingOffset.y + ")");
                    foundBuildLocation = true;
                    break;
                }
                buildingOffset.y += 1f;
            }
            buildingOffset = Vector2.zero;
            while (buildingOffset.y > -10f && !foundBuildLocation)
            { // Try to build downwards
                blueprint.transform.position = new Vector3(transform.position.x + buildingOffset.x, transform.position.y, transform.position.z + buildingOffset.y);
                if (basicBuild.canBuildHere)
                {
                    Debug.Log("AI found a build location downward (" + buildingOffset.y + ")");
                    foundBuildLocation = true;
                    break;
                }
                buildingOffset.y -= 1f;
            }

            if (foundBuildLocation)
            {
                TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentFood -= scriptableBuilding.FoodCost;
                TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentWood -= scriptableBuilding.WoodCost;
                TeamManager.instance.teamList[GetComponent<BasicUnit>().Team].teamCurrentStone -= scriptableBuilding.StoneCost;
                Destroy(blueprint.GetComponent<Rigidbody>()); // So the remaining Town Center does not trigger anymore.
                //blueprint.SetActive(true);
                Debug.Log("Starting construction with this worker");
                GetComponent<ConstructionHandler>().StartConstruction(blueprint);
            }
            else
            {
                Destroy(blueprint);
            }
        }
        return false;
    }
    protected override void HandleCurrentAction()
    {
        if (CurrentActionList.Count > 0)
        {
            switch (CurrentActionList[0])
            {
                case AIAction.ChopNearestTree:
                    if (!actionInProgress)
                        ChopNearestTree();
                    else
                        EvaluateActionInProgress();
                    break;
                case AIAction.GatherNearestBush:
                    if (!actionInProgress)
                        GatherNearestBush();
                    else
                        EvaluateActionInProgress();
                    break;
                case AIAction.MineNearestStone:
                    if (!actionInProgress)
                        MineNearestStone();
                    else
                        EvaluateActionInProgress();
                    break;
            }
        }
        base.HandleCurrentAction();
    }
    protected override void EvaluateActionInProgress()
    {
        base.EvaluateActionInProgress();
        if (CurrentActionList.Count > 0)
        {
            switch (CurrentActionList[0])
            {
                case AIAction.ChopNearestTree:
                    if (!((WorkerUnit)basicUnit).IsBusy())
                    {
                        if (CurrentActionList.Count > 0)
                        {
                            LastAction = CurrentActionList[0];
                            CurrentActionList.RemoveAt(0);
                        }
                        actionInProgress = false;
                        if (CurrentActionList.Count == 0)
                        {
                            CurrentActionList.Add(AIAction.ChopNearestTree);
                        }
                        WaitRandomTimeAmount(0.25f, 0.75f);
                    }
                    break;
                case AIAction.GatherNearestBush:
                    if (!((WorkerUnit)basicUnit).IsBusy())
                    {
                        if (CurrentActionList.Count > 0)
                        {
                            LastAction = CurrentActionList[0];
                            CurrentActionList.RemoveAt(0);
                        }
                        actionInProgress = false;
                        if (CurrentActionList.Count == 0)
                        {
                            CurrentActionList.Add(AIAction.GatherNearestBush);
                        }
                        WaitRandomTimeAmount(0.25f, 0.75f);
                    }
                    break;
                case AIAction.MineNearestStone:
                    if (!((WorkerUnit)basicUnit).IsBusy())
                    {
                        if (CurrentActionList.Count > 0)
                        {
                            LastAction = CurrentActionList[0];
                            CurrentActionList.RemoveAt(0);
                        }
                        actionInProgress = false;
                        if (CurrentActionList.Count == 0)
                        {
                            CurrentActionList.Add(AIAction.MineNearestStone);
                        }
                        WaitRandomTimeAmount(0.25f, 0.75f);
                    }
                    break;
            }
        }
    }
    private void ChopNearestTree()
    {
        if (!((WorkerUnit)basicUnit).IsBusy())
        {
            if (((WorkerUnit)basicUnit).ChopNearestTree())
            {
                actionInProgress = true;
            }
            else
            {
                if (CurrentActionList.Count > 0)
                {
                    //Debug.Log("Removing most current task: " + CurrentActionList[0].ToString() + " Unable to find resource...");
                    CurrentActionList.RemoveAt(0);
                }
            }
        }
    }
    private void GatherNearestBush()
    {
        if (!((WorkerUnit)basicUnit).IsBusy())
        {
            if (((WorkerUnit)basicUnit).GatherNearestBush())
            {
                actionInProgress = true;
            }
            else
            {
                if (CurrentActionList.Count > 0)
                {
                    //Debug.Log("Removing most current task: " + CurrentActionList[0].ToString() + " Unable to find resource...");
                    CurrentActionList.RemoveAt(0);
                }
            }
        }
    }
    private void MineNearestStone()
    {
        if (!((WorkerUnit)basicUnit).IsBusy())
        {
            if (((WorkerUnit)basicUnit).MineNearestStone())
            {
                actionInProgress = true;
            }
            else
            {
                if (CurrentActionList.Count > 0)
                {
                    //Debug.Log("Removing most current task: " + CurrentActionList[0].ToString() + " Unable to find resource...");
                    CurrentActionList.RemoveAt(0);
                }
            }
        }
    }
}
