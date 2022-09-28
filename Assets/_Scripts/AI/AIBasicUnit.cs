using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIAction
{
    RandomMovement,
    RandomRotation,
    AttackNearestEnemy
}

public class AIBasicUnit : MonoBehaviour
{
    private List<AIAction> CurrentActionList;
    private NavMeshAgent navAgent;
    private bool actionInProgress;
    private Vector3 destination;
    private float waitUntil;
    private float timeSlerp;
    private Quaternion rotationTarget;
    private BasicUnit basicUnit;
    private float startTime;
    protected virtual void Awake()
    {
        CurrentActionList = new List<AIAction>();
        if (!TryGetComponent(out navAgent))
            Debug.Log("Unable to get NavMeshAgent on AI Basic Unit!");
        basicUnit = GetComponent<BasicUnit>();
    }
    protected virtual void Update()
    {
        if (Time.time > waitUntil)
        {
            if (basicUnit.takenDamageRecently)
            {
                if (CurrentActionList.Count > 0)
                {
                    CurrentActionList.RemoveAt(0);
                    actionInProgress = false;
                }
                basicUnit.SetAttackTarget((BasicUnit)basicUnit.attacker);
                WaitRandomTimeAmount(4f, 6f);
            }
            else
            {
                if (CurrentActionList.Count > 0)
                {
                    switch (CurrentActionList[0])
                    {
                        case AIAction.RandomMovement:
                            if (!actionInProgress)
                                DoRandomMovement();
                            else
                                EvaluateActionInProgress();
                            break;
                        case AIAction.RandomRotation:
                            if (!actionInProgress)
                                DoRandomRotation();
                            else
                                EvaluateActionInProgress();
                            break;
                        case AIAction.AttackNearestEnemy:
                            if (!actionInProgress)
                                AttackNearestEnemy();
                            else
                                EvaluateActionInProgress();
                            break;
                    }
                }
                if (navAgent != null && navAgent.velocity.magnitude < 0.1 && CurrentActionList.Count == 0)
                {
                    startTime = Time.time;
                    float rand = Random.Range(0f, 1f);
                    if (rand < 0.8f)
                    {
                        CurrentActionList.Add(AIAction.RandomMovement);
                    }
                    else if (rand < 1f)
                    {
                        CurrentActionList.Add(AIAction.RandomRotation);
                    }
                    else
                    {
                        CurrentActionList.Add(AIAction.AttackNearestEnemy);
                    }
                }
            }
            waitUntil += 0.083f; // 5/60 only call this at a bare minimum of 5 times every second.
        }
    }
    protected void DoRandomMovement()
    {
        float randXMovement;
        float randZMovement;
        Vector3 targetDestination;

        // From our current location move ourselves to the new point as long as we are on Ground yet.
        randXMovement = Random.Range(-10f, 10f);
        randZMovement = Random.Range(-10f, 10f);
        targetDestination = transform.position + new Vector3(randXMovement, 0, randZMovement);
        Ray ray = new Ray();
        ray.origin = targetDestination + new Vector3(0, 10, 0);
        ray.direction = new Vector3(0, -1, 0);
        //Debug.DrawRay(rayTest.origin, rayTest.direction * 200, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
            {
                navAgent.SetDestination(targetDestination);
                actionInProgress = true;
                //Debug.Log("AI setting NavAgent destination to " + targetDestination.ToString() + " distance to dest = " + navAgent.remainingDistance);
            }
            else
            {
                WaitRandomTimeAmount(0f, 2f);
            }
        }
    }
    protected void DoRandomRotation()
    {
        timeSlerp = 0;
        actionInProgress = true;
        rotationTarget = Quaternion.Euler(0, Random.Range(0, 360), 0);
        //Debug.Log("AI setting NavAgent rotation to " + rotationTarget.eulerAngles.y + " degrees");
    }
    protected void AttackNearestEnemy()
    {
        string targetLayerString = "PlayerUnitLayer";
        if (gameObject.layer == LayerMask.NameToLayer("PlayerUnitLayer"))
            targetLayerString = "EnemyUnitLayer";
        //Debug.Log("Attack nearest target... I am on " + LayerMask.LayerToName(gameObject.layer) + " targettingLayer = " + targetLayerString);
        int loopBreak = 0;
        while (loopBreak < 25)
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, 0.5f + loopBreak, LayerMask.GetMask(targetLayerString));
            int startIndex = 0;
            if (loopBreak != 0)
                startIndex = Random.Range(0, rangeChecks.Length);
            for (int i = startIndex; i < rangeChecks.Length; i++)
            {
                if (rangeChecks[i].gameObject.transform.parent != null)
                    continue;
                BasicUnit target;
                if (rangeChecks[i].TryGetComponent(out target))
                {
                    GetComponent<BasicUnit>().SetAttackTarget(target);
                    actionInProgress = true;
                    break;
                }
            }
            loopBreak++;
            if (actionInProgress)
                break;
        }
        if (!actionInProgress)
        {
            if (CurrentActionList.Count > 0)
                CurrentActionList.RemoveAt(0);
            WaitRandomTimeAmount(0.5f, 1.5f);
        }
    }
    protected void EvaluateActionInProgress()
    {
        switch (CurrentActionList[0])
        {
            case AIAction.RandomMovement:
                if (navAgent.remainingDistance < 0.1f)
                {
                    navAgent.SetDestination(transform.position);
                    if (CurrentActionList.Count > 0)
                        CurrentActionList.RemoveAt(0);
                    actionInProgress = false;
                    WaitRandomTimeAmount(1f, 5f);
                }
                break;
            case AIAction.RandomRotation:
                if (Mathf.Abs(transform.rotation.eulerAngles.y - rotationTarget.eulerAngles.y) > 0.1)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotationTarget, timeSlerp);
                    timeSlerp += Time.deltaTime / 20;
                }
                else
                {
                    if (CurrentActionList.Count > 0)
                        CurrentActionList.RemoveAt(0);
                    actionInProgress = false;
                    WaitRandomTimeAmount(0.5f, 3f);
                }
                break;
            case AIAction.AttackNearestEnemy:
                if (basicUnit.HasActiveTarget())
                {
                    if (CurrentActionList.Count > 0)
                        CurrentActionList.RemoveAt(0);
                    actionInProgress = false;
                    WaitRandomTimeAmount(0.5f, 3f);
                }
                break;
        }

        if ((startTime + 25f) < Time.time)
        {
            if (CurrentActionList.Count > 0)
                CurrentActionList.RemoveAt(0);
            actionInProgress = false;
            WaitRandomTimeAmount(0.5f, 2f);
        }
    }
    protected void WaitRandomTimeAmount(float minWaitTime, float maxWaitTime)
    {
        waitUntil = Time.time + Random.Range(minWaitTime, maxWaitTime);
    }
}
