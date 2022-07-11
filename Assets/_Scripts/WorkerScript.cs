using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerScript : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private GameObject currentBuild;
    private int buildRange = 4;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(new Vector3(-5, 0, 0));
    }
    void Update()
    {
        if (currentBuild != null)
        {
            if (Vector3.Distance(gameObject.transform.position, currentBuild.transform.position) <= buildRange)
            {
                navAgent.isStopped = true;
                // Do something here to build the actual building.
                // Call currentBuild.StartBuilding
                //  This func will move along a progress bar.
                //  At certain % it will spawn the next prog prefab and delete the old one. Transfer over the build % and continue until complete.
            }
        }
    }
    public void ConstructBuild(GameObject build)
    {
        navAgent.SetDestination(build.transform.position);
        currentBuild = build;
    }
}
