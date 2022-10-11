using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AITeamState
{
    FocusFood,
    FocusWood,
    FocusStone
}
public enum BuildableObjectList
{
    None,
    Worker
}

public class AITeamController : MonoBehaviour
{
    private TeamResourceManager myTeamManager;
    private AITeamState nextState;
    private BuildableObjectList nextObjectToMake;
    private float desiredFoodRatio;
    private float desiredWoodRatio;
    private float desiredStoneRatio;
    private float allowedVarience;
    private TownCenterBuilding mainTownCenter;
    private List<WorkerUnit> idleWorkers;
    private void Start()
    {
        myTeamManager = gameObject.GetComponent<TeamResourceManager>();
        desiredFoodRatio = 0.80f;
        desiredWoodRatio = 0.15f;
        desiredStoneRatio = 0.05f;
        allowedVarience = 0.025f;
        idleWorkers = new List<WorkerUnit>();
    }
    private void Update()
    {
        DetermineNewState();
        HandleStateActions();
        DetermineWhatToBuild();
        HandleNewBuild();
    }
    private void DetermineNewState()
    {
        float totalResources = myTeamManager.teamCurrentFood + myTeamManager.teamCurrentWood + myTeamManager.teamCurrentStone;
        if ( (myTeamManager.teamCurrentFood / totalResources) + allowedVarience > desiredFoodRatio ||
             (myTeamManager.teamCurrentFood / totalResources) - allowedVarience < desiredFoodRatio )
        {
            nextState = AITeamState.FocusFood;
        }
        else if ( (myTeamManager.teamCurrentWood / totalResources) + allowedVarience > desiredWoodRatio ||
                  (myTeamManager.teamCurrentWood / totalResources) - allowedVarience < desiredWoodRatio )
        {
            nextState = AITeamState.FocusWood;
        }
        else if ( (myTeamManager.teamCurrentStone / totalResources) + allowedVarience > desiredStoneRatio ||
                  (myTeamManager.teamCurrentStone / totalResources) - allowedVarience < desiredStoneRatio)
        {
            nextState = AITeamState.FocusStone;
        }
    }
    private void HandleStateActions()
    {
        if (idleWorkers.Count == 0)
        {
            FindAnyIdleWorkers();
        }
        switch (nextState)
        {
            case AITeamState.FocusFood:
                if (idleWorkers.Count > 0)
                {
                    for (int i = idleWorkers.Count - 1; i >= 0; i--)
                    {
                        WorkerUnit worker = idleWorkers[i];
                        AIWorkerUnit workerAI = worker.gameObject.GetComponent<AIWorkerUnit>();
                        if (workerAI.CheckActionCount() == 0)
                        {
                            if (AnyResourceWithinRadius(worker.gameObject, typeof(BushResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.GatherNearestBush, false);
                            }
                            else if (AnyResourceWithinRadius(worker.gameObject, typeof(TreeResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.ChopNearestTree, false);
                            }
                            else if (AnyResourceWithinRadius(worker.gameObject, typeof(StoneResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.MineNearestStone, false);
                            }
                            else
                            {
                                workerAI.AddNewAction(AIAction.RandomMovement, false);
                            }
                        }
                        idleWorkers.Remove(worker);
                    }
                }
                break;
            case AITeamState.FocusWood:
                if (idleWorkers.Count > 0)
                {
                    for (int i = idleWorkers.Count - 1; i >= 0; i--)
                    {
                        WorkerUnit worker = idleWorkers[i];
                        AIWorkerUnit workerAI = worker.gameObject.GetComponent<AIWorkerUnit>();
                        if (workerAI.CheckActionCount() == 0)
                        {
                            if (AnyResourceWithinRadius(worker.gameObject, typeof(TreeResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.ChopNearestTree, false);
                            }
                            else if (AnyResourceWithinRadius(worker.gameObject, typeof(BushResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.GatherNearestBush, false);
                            }
                            else if (AnyResourceWithinRadius(worker.gameObject, typeof(StoneResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.MineNearestStone, false);
                            }
                            else
                            {
                                workerAI.AddNewAction(AIAction.RandomMovement, false);
                            }
                        }
                        idleWorkers.Remove(worker);
                    }
                }
                break;
            case AITeamState.FocusStone:
                if (idleWorkers.Count > 0)
                {
                    for (int i = idleWorkers.Count - 1; i >= 0; i--)
                    {
                        WorkerUnit worker = idleWorkers[i];
                        AIWorkerUnit workerAI = worker.gameObject.GetComponent<AIWorkerUnit>();
                        if (workerAI.CheckActionCount() == 0)
                        {
                            if (AnyResourceWithinRadius(worker.gameObject, typeof(StoneResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.MineNearestStone, false);
                            }
                            else if (AnyResourceWithinRadius(worker.gameObject, typeof(BushResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.GatherNearestBush, false);
                            }
                            else if (AnyResourceWithinRadius(worker.gameObject, typeof(TreeResource), 24.5f))
                            {
                                workerAI.AddNewAction(AIAction.ChopNearestTree, false);
                            }
                            else
                            {
                                workerAI.AddNewAction(AIAction.RandomMovement, false);
                            }
                        }
                        idleWorkers.Remove(worker);
                    }
                }
                break;
        }
    }
    private void DetermineWhatToBuild()
    {
        if (myTeamManager.teamCurrentFood >= ((ScriptableUnit)ResourceDictionary.instance.GetPreset("Worker")).FoodCost)
        {
            nextObjectToMake = BuildableObjectList.Worker;
        }
        else
        {
            nextObjectToMake = BuildableObjectList.None;
        }
    }
    private void HandleNewBuild()
    {
        switch (nextObjectToMake)
        {
            case BuildableObjectList.Worker:
                if (mainTownCenter == null)
                {
                    foreach (GameObject go in myTeamManager.buildingList)
                    {
                        go.TryGetComponent(out mainTownCenter);
                    }
                    if (mainTownCenter == null)
                        Debug.Log("Main TC is still null, all town centers destroyed?");
                }
                else
                {
                    mainTownCenter.AddWorkerToQueue();
                    nextObjectToMake = BuildableObjectList.None;
                }
                break;
        }
    }
    private void FindAnyIdleWorkers()
    {
        foreach (GameObject go in myTeamManager.unitList)
        {
            if (go.TryGetComponent(out WorkerUnit worker))
            {
                if (!worker.IsBusy())
                {
                    if (!idleWorkers.Contains(worker))
                    {
                        idleWorkers.Add(worker);
                    }
                }
            }
        }
    }
    private bool AnyResourceWithinRadius(GameObject go, System.Type resourceType, float radius)
    {
        Collider[] rangeChecks = Physics.OverlapSphere(go.transform.position, radius, LayerMask.GetMask("ResourceLayer"));
        GameObject tempGo;
        for (int i = 0; i < rangeChecks.Length; i++)
        {
            tempGo = rangeChecks[i].gameObject;
            while (tempGo.transform.parent != null)
            {
                tempGo = tempGo.transform.parent.gameObject;
            }
            BasicResource foundResource;
            if (tempGo.TryGetComponent(out foundResource))
            {
                if (foundResource.GetType() == resourceType &&
                    (foundResource.currentFoodAmount > 0 || foundResource.currentWoodAmount > 0 || foundResource.currentStoneAmount > 0))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
