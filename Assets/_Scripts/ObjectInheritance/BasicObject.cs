using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObject : MonoBehaviour
{
    public TeamEnum Team;
    protected virtual void Awake() { }
    protected virtual void Start() 
    {
        Team = TeamManager.instance.AssignTeam(gameObject.layer);
    }
    public virtual void DoAction() { }
    protected float DistanceToTarget(Transform target)
    {
        if (target != null)
        {
            return (Vector3.Distance(target.position, transform.position));
        }

        throw new Exception("Tried to calculate distance to a null target!");
    }
    protected void Die()
    {
        Destroy(gameObject);
    }
}
public enum TeamEnum
{
    Unknown,
    Player,
    Enemy
}