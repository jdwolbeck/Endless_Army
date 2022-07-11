using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FighterScript : MonoBehaviour
{
    private NavMeshAgent navAgent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected void OnEnable()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        navAgent.SetDestination(new Vector3(2, 0, 1));
    }
}
