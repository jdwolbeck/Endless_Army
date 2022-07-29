using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FighterUnit : BasicUnit
{
    protected override void Awake()
    {
        base.Awake();
        MaxHealth = Health = 50f;
        AttackRange = 2f;
        AttackSpeed = 0.75f;
        Damage = 5f;
    }
    protected override void Start()
    {
        base.Start();
        navAgent.SetDestination(new Vector3(2, 0, 1));
    }
    protected override void Update()
    {
        base.Update();
    }
}
