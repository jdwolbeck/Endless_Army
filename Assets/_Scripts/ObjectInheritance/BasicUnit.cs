using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicUnit : BasicObject
{
    public bool isSpawnedFromInspector;
    protected ItemManager equippedItemManager;
    [SerializeField] protected GameObject HealthBar;
    protected NavMeshAgent navAgent;
    protected Slider HealthSlider;
    protected float Damage;
    protected float AttackRange;
    protected float AttackSpeed;
    protected float attackCooldown;
    protected BasicUnit currentTarget;
    protected bool inCombat;

    protected Animator animator;
    protected bool animatorPresent;

    protected override void Awake()
    {
        base.Awake();
        animatorPresent = false;
        navAgent = GetComponent<NavMeshAgent>();
        HealthSlider = HealthBar.GetComponent<Slider>();
        HealthBar.SetActive(false);
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
        SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(gameObject.layer));
        //TeamShirt.GetComponent<MeshRenderer>().material = TeamManager.instance.AssignTeamMaterial(gameObject.layer);
        //TeamShirtShoulder.GetComponent<MeshRenderer>().material = TeamManager.instance.AssignTeamMaterial(gameObject.layer);
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
    }
    public override void DoAction()
    {
        base.DoAction();
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (currentTarget != null)
        {
            currentTarget = null;
        }
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("EnemyUnitLayer")))
        {
            Attack(hit.transform.gameObject.GetComponent<BasicUnit>());
            hit.transform.gameObject.GetComponent<BasicObject>().ObjectDied += ClearTarget;
        }
        else if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("GroundLayer")))
        {
            if (inCombat)
                inCombat = false;
            navAgent.SetDestination(hit.point);
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
    protected void Attack(BasicUnit target)
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
                inCombat = true;
                navAgent.SetDestination(gameObject.transform.position);
                if (attackCooldown <= 0)
                {
                    //Debug.Log(gameObject.ToString() + ": Attacked enemy unit");
                    attackCooldown = 1 / AttackSpeed;
                    target.TakeDamage(Damage);
                }
            }
            else
            {
                navAgent.SetDestination(currentTarget.transform.position);
            }
        }
    }
    protected void TakeDamage(float damage)
    {
        currentHealth -= damage;
        //Debug.Log(gameObject.ToString() + ": Took damage");
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (!HealthBar.activeInHierarchy)
            {
                HealthBar.SetActive(true);
            }
            HealthSlider.normalizedValue = currentHealth / MaxHealth;
        }
    }
    protected override void Die()
    {
        if (Team == TeamEnum.Player)
        {
            GameHandler.instance.playerUnits.Remove(gameObject);
        }
        else
        {
            GameHandler.instance.enemyUnits.Remove(gameObject);
        }
        base.Die();
    }
    protected void ClearTarget()
    {
        currentTarget = null;
        inCombat = false;
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
}