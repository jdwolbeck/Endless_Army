using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerScript : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private GameObject currentBuild;
    private int buildRange;
    private bool constructingBuild;
    private BuildingControls buildControls;
    private float buildProgress;
    private float cooldownTime;
    private float nextBuildTime;

    void Start()
    {
        buildRange = 4;
        constructingBuild = false;
        navAgent = GetComponent<NavMeshAgent>();
        cooldownTime = 1.0f;
        nextBuildTime = 0;
    }
    void Update()
    {
        if (currentBuild != null)
        {
            if (constructingBuild == false && Vector3.Distance(gameObject.transform.position, currentBuild.transform.position) <= buildRange)
            {
                navAgent.SetDestination(transform.position);
                constructingBuild = true;
                nextBuildTime = Time.time + cooldownTime;
                Debug.Log("Worker started construction");
            }
            if (Time.time > nextBuildTime && constructingBuild)
            {
                nextBuildTime = Time.time + cooldownTime;
                
                buildControls.IncreaseProgressBar(0.1f);
                if (buildProgress >= 1.0f)
                {
                    // Build complete
                    StopBuilding();
                }
            }
        }
    }
    public void ConstructBuild(GameObject build)
    {
        if (build != null)
        {
            navAgent.SetDestination(build.transform.position);
            currentBuild = build;
            buildControls = currentBuild.GetComponent<BuildingControls>();
            if (buildControls == null)
            {
                Debug.Log("Build controls is null!");
            }
            Debug.Log("Setting worker destination to " + build.transform.position.ToString());
        }
        else
        {
            Debug.Log("Build was null");
        }
    }
    public void StopBuilding()
    {
        buildControls = null;
        constructingBuild = false;
        currentBuild = null;
    }
}
