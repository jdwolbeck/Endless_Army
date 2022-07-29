using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicUnit : BasicObject
{
    [SerializeField]
    protected GameObject HealthBar;
    protected Slider HealthSlider;
    [SerializeField]
    protected GameObject TeamShirt;
    [SerializeField]
    protected GameObject TeamShirtShoulder;
    protected NavMeshAgent navAgent;
    protected float AttackRange;
    protected float AttackSpeed;
    protected float Damage;
    protected float Health;
    protected float MaxHealth;
    private BasicUnit currentTarget;
    private bool isMovingToTarget;
    private float attackCooldown;

    protected override void Awake()
    {
        base.Awake();
        navAgent = GetComponent<NavMeshAgent>();
        MaxHealth = Health = 5f;
        HealthSlider = HealthBar.GetComponent<Slider>();
        AttackRange = 1f;
        AttackSpeed = 1f;
        Damage = 1f;
        isMovingToTarget = false;
        attackCooldown = 0f;
        HealthBar.SetActive(false);
    }
    protected override void Start()
    {
        base.Start();
        TeamShirt.GetComponent<MeshRenderer>().material = TeamManager.instance.AssignTeamMaterial(gameObject.layer);
        TeamShirtShoulder.GetComponent<MeshRenderer>().material = TeamManager.instance.AssignTeamMaterial(gameObject.layer);
    }
    protected virtual void Update()
    {
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
        }
        else if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("GroundLayer")))
        {
            navAgent.SetDestination(hit.point);
        }
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
                navAgent.SetDestination(gameObject.transform.position);
                if (attackCooldown <= 0)
                {
                    Debug.Log(gameObject.ToString() + ": Attacked enemy unit");
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
        Health -= damage;
        Debug.Log(gameObject.ToString() + ": Took damage");
        if (Health <= 0)
        {
            Die();
        }
        else
        {
            if (!HealthBar.activeInHierarchy)
            {
                HealthBar.SetActive(true);
            }
            HealthSlider.normalizedValue = Health / MaxHealth;
        }
    }
}
