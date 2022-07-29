using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FighterUnit : BasicUnit
{
    void Update()
    {
        navAgent.SetDestination(new Vector3(2, 0, 1));
    }
}
