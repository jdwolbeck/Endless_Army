using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Formation
{
    public int formationType;
    public int armySize;
    public int armyPosition;
    private const float UNIT_WIDTH_ROW = 1.5f;
    private const float UNIT_WIDTH_COLUMN = 1.5f;
    private const int LINE_COLUMN_COUNT = 10;

    public Formation()
    {
        formationType = -1;
        armySize = 0;
        armyPosition = 0;
    }
    public Vector3 GetMoveLocation(Vector3 origDestination)
    {
        Vector3 destination = origDestination;
        switch (formationType)
        {
            case 0:
                // Line formation
                int rows = GetRowCount();
                int columns = GetColumnCount();
                int myRow = Mathf.CeilToInt(armyPosition / (float)LINE_COLUMN_COUNT);
                int myColumn = armyPosition % LINE_COLUMN_COUNT;
                Vector2 unitCoords = new Vector2(armyPosition % LINE_COLUMN_COUNT, armyPosition / LINE_COLUMN_COUNT);
                    //Debug.Log("Unit " + armyPosition + " has coordinates of (" + unitCoords.ToString() + ")");
                // Get the middle row and middle column.
                float centerColumn = (columns - 1) / 2f;
                float centerMyColumn = (GetThisUnitsColumnCount() - 1) / 2f;
                Debug.Log("Unit " + armyPosition + " column(old, new): (" + centerColumn + ", " + centerMyColumn + ")");
                float centerRow = (rows - 1) / 2f;
                float unitColumnOffset = (unitCoords.x - centerMyColumn) * UNIT_WIDTH_COLUMN;
                float unitRowOffset = (unitCoords.y - centerRow) * UNIT_WIDTH_ROW;
                Debug.Log("(ColumnOffset, RowOffset) (" + unitColumnOffset + ", " + unitRowOffset + ")");
                destination.x += unitColumnOffset;
                destination.z += unitRowOffset;
                    //Debug.Log("From an original destination: " + origDestination.ToString() + "  -- adding on: (column, row) (" + 
                        //unitColumnOffset + ", " + unitRowOffset + ")");
                break;
        }
        return destination;
    }
    private int GetRowCount()
    {
        switch (formationType) 
        { 
            case 0:
                return Mathf.CeilToInt(armySize / (float)LINE_COLUMN_COUNT);
            default:
                Debug.Log("ERROR: GetRowCount() unhandled formationType...");
                return -1;
        }
    }
    private int GetColumnCount()
    {
        switch (formationType)
        {
            case 0:
                if (armySize < LINE_COLUMN_COUNT)
                    return armySize;
                else
                    return LINE_COLUMN_COUNT;
            default:
                Debug.Log("ERROR: GetColumnCount() unhandled formationType...");
                return -1;
        }
    }
    private int GetThisUnitsColumnCount()
    {
        int thisUnitsRow = armyPosition / LINE_COLUMN_COUNT;
        if (thisUnitsRow == GetRowCount() - 1)
        {
            return armySize - ((GetRowCount() - 1) * LINE_COLUMN_COUNT);
        }
        else
        {
            return LINE_COLUMN_COUNT;
        }
    }
}
public class BasicUnit : BasicObject
{
    public BasicObject attacker;
    public bool takenDamageRecently { get; private set; }
    private float damageTimer;
    protected ItemManager equippedItemManager;
    protected NavMeshAgent navAgent;
    protected float Damage;
    protected float AttackRange;
    protected float AttackSpeed;
    protected float attackCooldown;
    protected BasicObject currentTarget;
    protected bool subscribedToTarget;
    protected bool inCombat;
    protected Formation currentFormation;

    protected Animator animator;
    protected bool animatorPresent;
    protected bool aiAttacking;

    protected override void Awake()
    {
        base.Awake();
        animatorPresent = false;
        navAgent = GetComponent<NavMeshAgent>();
        TryGetComponent<ItemManager>( out equippedItemManager);
        if (equippedItemManager == null)
            Debug.Log("Unable to find the Item Manager!");
        TryGetComponent<Animator>(out animator);
        if (animator != null)
            animatorPresent = true;
        currentFormation = new Formation();
    }
    protected override void Start()
    {
        base.Start();
        //SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(gameObject.layer));
        SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(Team));
    }
    protected virtual void Update()
    {
        if (!isAddedToObjectList && Team != invalidTeamId)
        {
            TeamManager.instance.teamList[Team].unitList.Add(gameObject);
            isAddedToObjectList = true;
        }
        if (animatorPresent)
        {
            float speedPercent = navAgent.velocity.magnitude / navAgent.speed;
            animator.SetFloat("SpeedPercent", speedPercent, .1f, Time.deltaTime); // BlendTree Variable, local speed%, transition time between animations, deltaTime
            animator.SetBool("InCombat", inCombat);
        }
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (currentTarget != null)
        {
            Attack(currentTarget);
        }
        if (takenDamageRecently)
        {
            if (Time.time > damageTimer + 5f)
            {
                takenDamageRecently = false;
            }
        }
    }
    public override void DoAction()
    {
        base.DoAction();
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (currentTarget != null)
            ClearTarget(currentTarget.gameObject);
        if (gameObject.TryGetComponent(out AIBasicUnit aiBaseUnit))
        {
            aiBaseUnit.ClearAllTasks();
        }
        if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("EnemyUnitLayer")))
        {
            SetAttackTarget(hit.transform.gameObject.GetComponent<BasicObject>(), true);
        }
        else if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("EnemyBuildingLayer")))
        {
            GameObject go = hit.transform.gameObject;
            for (int i = 0; i < 100 && go.transform.parent != null; i++)
            {
                go = go.transform.parent.gameObject;
            }
            SetAttackTarget(go.GetComponent<BasicObject>(), true);
        }
        else if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("GroundLayer")))
        {
            SetMoveLocation(hit.point);
        }
    }
    public void SetAttackTarget(BasicObject target, bool findNewTarget)
    {
        aiAttacking = findNewTarget;
        Attack(target);
    }
    public void SetMoveLocation(Vector3 moveLocation)
    {
        if (currentFormation.formationType != -1)
        {
            navAgent.SetDestination(currentFormation.GetMoveLocation(moveLocation));
        }
        else
        {
            navAgent.SetDestination(moveLocation);
        }
    }
    protected float DistanceToTarget(Transform target)
    {
        if (target != null)
        {
            return (Vector3.Distance(target.position, transform.position));
        }

        throw new Exception("Tried to calculate distance to a null target!");
    }
    protected void Attack(BasicObject target)
    {
        if (target != null)
        {
            if (currentTarget != target)
            {// Set the current target if it wasnt previously set
                currentTarget = target;
                Debug.Log("Setting current target to target: " + currentTarget.ToString() + " | " + target.ToString());
            }
            if (DistanceToTarget(currentTarget.transform) < AttackRange)
            {
                if (!subscribedToTarget)
                {
                    currentTarget.ObjectDied += ClearTarget;
                    subscribedToTarget = true;
                }
                inCombat = true;
                SetMoveLocation(transform.position);

                // Look at our current target
                Quaternion lookOnLook = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3);

                if (attackCooldown <= 0)
                {
                    //Debug.Log(gameObject.ToString() + ": Attacked enemy unit");
                    attackCooldown = 1 / AttackSpeed;
                    target.TakeDamage(Damage, gameObject.GetComponent<BasicObject>());
                }
            }
            else
            {
                SetMoveLocation(currentTarget.transform.position);
            }
        }
    }
    public override void TakeDamage(float damage, BasicObject attackingObject)
    {
        base.TakeDamage(damage, attackingObject);
        attacker = attackingObject;
        takenDamageRecently = true;
        damageTimer = Time.time;
    }
    protected override void Die()
    {
        GameHandler.instance.RemoveUnit(gameObject, Team);
        TeamManager.instance.teamList[(int)Team].unitList.Remove(gameObject);
        /*
        if (Team == TeamEnum.Player)
        {
            GameHandler.instance.playerUnits.Remove(gameObject);
        }
        else
        {
            GameHandler.instance.enemyUnits.Remove(gameObject);
        }
        */

        if (currentTarget != null)
        {
            currentTarget.ObjectDied -= ClearTarget;
        }
        base.Die();
    }
    public bool ClearCurrentTarget()
    {
        if (currentTarget != null)
        {
            ClearTarget(currentTarget.gameObject);
            return true;
        }
        return false;
    }
    protected void ClearTarget(GameObject go)
    {
        if (subscribedToTarget)
        {
            currentTarget.ObjectDied -= ClearTarget;
            subscribedToTarget = false;
        }
        currentTarget = null;
        inCombat = false;
        if (aiAttacking)
        {
            if (gameObject.TryGetComponent(out AIBasicUnit aiUnit))
            {
                aiUnit.AddNewAction(AIAction.AttackNearestEnemy, false);
            }
        }
    }
    public override void LoadFromPreset(ScriptableObj obj)
    {
        base.LoadFromPreset(obj);
        AttackRange = ((ScriptableUnit)obj).AttackRange;
        AttackSpeed = ((ScriptableUnit)obj).AttackSpeed;
        Damage = ((ScriptableUnit)obj).Damage;
    }
    public void SetAnimatorLayerWeight(string layerName, float weight)
    {
        animator.SetLayerWeight(animator.GetLayerIndex(layerName), weight);
    }
    public bool HasActiveTarget()
    {
        return currentTarget != null;
    }
    public void SetMovementFormation(int formationType, int armyPosition, int armySize)
    {
        Debug.Log("Setting formation of unit " + gameObject.ToString() + ": FormationType/Pos/Size: " + formationType + "/" + armyPosition + "/" + armySize);
        currentFormation.formationType = formationType;
        currentFormation.armyPosition = armyPosition;
        currentFormation.armySize = armySize;
    }
}