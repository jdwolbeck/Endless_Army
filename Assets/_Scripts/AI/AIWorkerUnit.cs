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
