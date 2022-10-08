using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicUnit : BasicObject
{
    public bool isSpawnedFromInspector;
    public BasicObject attacker;
    public bool takenDamageRecently { get; private set; }
    private float damageTimer;
    protected ItemManager equippedItemManager;
    [SerializeField] protected GameObject HealthBar;
    protected GameObject HealthBarCanvas;
    protected NavMeshAgent navAgent;
    protected Slider HealthSlider;
    protected float Damage;
    protected float AttackRange;
    protected float AttackSpeed;
    protected float attackCooldown;
    protected BasicUnit currentTarget;
    protected bool subscribedToTarget;
    protected bool inCombat;

    protected Animator animator;
    protected bool animatorPresent;
    protected bool aiAttacking;

    protected override void Awake()
    {
        base.Awake();
        animatorPresent = false;
        navAgent = GetComponent<NavMeshAgent>();
        HealthSlider = HealthBar.GetComponent<Slider>();
        HealthBarCanvas = HealthBar.transform.parent.gameObject;
        HealthBarCanvas.SetActive(false);
        TryGetComponent<ItemManager>( out equippedItemManager);
        if (equippedItemManager == null)
            Debug.Log("Unable to find the Item Manager!");
        TryGetComponent<Animator>(out animator);
        if (animator != null)
            animatorPresent = true;
    }
    protected override void Start()
    {
        base.Start();
        //SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(gameObject.layer));
        SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(Team));
    }
    protected virtual void Update()
    {
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
            SetAttackTarget(hit.transform.gameObject.GetComponent<BasicUnit>());
        }
        else if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("GroundLayer")))
        {
            navAgent.SetDestination(hit.point);
        }
    }
    public void SetAttackTarget(BasicUnit target)
    {
        Attack(target);
    }
    protected float DistanceToTarget(Transform target)
    {
        if (target != null)
        {
            return (Vector3.Distance(target.position, transform.position));
        }

        throw new Exception("Tried to calculate distance to a null target!");
    }
    protected void Attack(BasicUnit target)
    {
        if (target != null)
        {
            if (currentTarget != target)
            {// Set the current target if it wasnt previously set
                currentTarget = target;
                //Debug.Log("Setting current target to target: " + currentTarget.ToString() + " | " + target.ToString());
            }
            if (DistanceToTarget(currentTarget.transform) < AttackRange)
            {
                if (!subscribedToTarget)
                {
                    aiAttacking = true;
                    currentTarget.ObjectDied += ClearTarget;
                    subscribedToTarget = true;
                }
                inCombat = true;
                navAgent.SetDestination(transform.position);

                // Look at our current target
                Quaternion lookOnLook = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3);

                if (attackCooldown <= 0)
                {
                    //Debug.Log(gameObject.ToString() + ": Attacked enemy unit");
                    attackCooldown = 1 / AttackSpeed;
                    target.TakeDamage(Damage, gameObject.GetComponent<BasicUnit>());
                }
            }
            else
            {
                navAgent.SetDestination(currentTarget.transform.position);
            }
        }
    }
    protected void TakeDamage(float damage, BasicObject attackingObject)
    {
        attacker = attackingObject;
        currentHealth -= damage;
        //Debug.Log(gameObject.ToString() + ": Took damage");
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (!HealthBarCanvas.activeInHierarchy)
            {
                HealthBarCanvas.SetActive(true);
            }
            HealthSlider.normalizedValue = currentHealth / MaxHealth;
        }
        takenDamageRecently = true;
        damageTimer = Time.time;
    }
    protected override void Die()
    {
        GameHandler.instance.RemoveUnit(gameObject, Team);
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
            aiAttacking = false;
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
}