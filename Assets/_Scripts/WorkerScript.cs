using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerScript : MonoBehaviour
{
    public const int foodCost = 50;
    private NavMeshAgent navAgent;
    private GameObject currentBuild;
    private int buildRange;
    private bool constructingBuild;
    private BuildingControls buildControls;
    private float buildProgress;
    private float buildCooldownTime;
    private float nextBuildTime;
    private GameObject currentResource;
    private ResourceHandler resourceHandler;
    private bool isHarvesting;
    private int harvestRange;
    private float harvestCooldownTime;
    private float nextHarvestTime;
    private int harvestAmount;

    void Start()
    {
        buildRange = 4;
        constructingBuild = false;
        navAgent = GetComponent<NavMeshAgent>();
        buildCooldownTime = 1.0f;
        nextBuildTime = 0;
        isHarvesting = false;
        harvestRange = 3;
        harvestCooldownTime = 1.5f;
        nextHarvestTime = 0;
        harvestAmount = 10;
    }
    void Update()
    {
        if (currentBuild != null)
        {
            if (constructingBuild == false && Vector3.Distance(gameObject.transform.position, currentBuild.transform.position) <= buildRange)
            {
                navAgent.SetDestination(transform.position);
                constructingBuild = true;
                nextBuildTime = Time.time + buildCooldownTime;
                Debug.Log("Worker started construction");
            }
            if (Time.time > nextBuildTime && constructingBuild)
            {
                nextBuildTime = Time.time + buildCooldownTime;
                
                buildControls.IncreaseProgressBar(0.1f);
                if (buildProgress >= 1.0f)
                {
                    // Build complete
                    StopBuilding();
                }
            }
        }
        if (currentResource != null)
        {
            if (isHarvesting == false && Vector3.Distance(gameObject.transform.position, currentResource.transform.position) <= harvestRange)
            {
                navAgent.SetDestination(transform.position);
                isHarvesting = true;
                nextHarvestTime = Time.time + harvestCooldownTime;
                Debug.Log("Worker started harvesting " + currentResource.ToString());
            }
            if (Time.time > nextHarvestTime && isHarvesting)
            {
                nextHarvestTime = Time.time + harvestCooldownTime;

                int resourcesObtained;
                ResourceType resourceType;
                int resourcesRemaining = resourceHandler.HarvestResource(harvestAmount, out resourcesObtained, out resourceType);
                if (gameObject.layer == LayerMask.NameToLayer("PlayerUnitLayer"))
                {
                    switch (resourceType)
                    {
                        case ResourceType.Food:
                            PlayerResourceManger.instance.UpdatePlayerFood(resourcesObtained);
                            break;
                        case ResourceType.Wood:
                            PlayerResourceManger.instance.UpdatePlayerWood(resourcesObtained);
                            break;
                        case ResourceType.Stone:
                            PlayerResourceManger.instance.UpdatePlayerStone(resourcesObtained);
                            break;
                    }

                }
                if (resourcesRemaining <= 0)
                {
                    // Resource depleted
                    StopHarvesting();
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
                Debug.Log("Build controls was null for build " + build.ToString());
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
    public void HarvestResource(GameObject resource)
    {
        if (resource != null)
        {
            // Find the top level gameObject of this resource
            GameObject tempResource = resource;
            while (tempResource.transform.parent != null)
            {
                tempResource = tempResource.transform.parent.gameObject;
                resource = tempResource;
            }

            navAgent.SetDestination(resource.transform.position);
            currentResource = resource;
            resourceHandler = currentResource.GetComponent<ResourceHandler>();
            if (resourceHandler == null)
            {
                Debug.Log("Resource handler was null for resource " + resource.ToString());
            }
            Debug.Log("Setting worker destination to " + resource.transform.position.ToString());
        }
        else
        {
            Debug.Log("Resource was null");
        }
        Debug.Log("Worker is going to harvest: " + resource.ToString() + " -- With Handler of: ") ;
    }
    public void StopHarvesting()
    {
        resourceHandler = null;
        isHarvesting = false;
        currentResource = null;
        Debug.Log("Stopping harvesting...");
    }
    public void StopAction()
    {
        if (currentBuild != null)
        {
            StopBuilding();
        }
        if (currentResource != null)
        {
            StopHarvesting();
        }
    }
}
