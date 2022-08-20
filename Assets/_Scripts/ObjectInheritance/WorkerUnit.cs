using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerUnit : BasicUnit
{
    //public static new int FoodCost = 50;
    public GameObject currentBuild { get; private set; }
    private int buildRange;
    private bool constructingBuild;
    private BasicBuilding currentBasicBuilding;
    private GameObject currentResource;
    private ResourceHandler resourceHandler;
    private bool isHarvesting;
    private int harvestRange;
    private float harvestCooldownTime;
    private float nextHarvestTime;
    private int harvestAmount;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        buildRange = 4;
        constructingBuild = false;
        isHarvesting = false;
        harvestRange = 3;
        harvestCooldownTime = 1.5f;
        nextHarvestTime = 0;
        harvestAmount = 10;
        if (isSpawnedFromInspector)
            LoadFromPreset((ScriptableUnit)ResourceDictionary.instance.GetPreset("Worker"));
    }
    protected override void Update()
    {
        base.Update();
        if (currentBuild != null)
        {
            if (constructingBuild == false && Vector3.Distance(gameObject.transform.position, currentBuild.transform.position) <= buildRange)
            {
                navAgent.SetDestination(transform.position);
                constructingBuild = true;
                currentBasicBuilding.UpdateActiveBuilders(true);
                Debug.Log("Set " + gameObject.ToString() + " as an active worker on " + currentBuild.ToString() + " basicBuild? " + currentBasicBuilding.ToString());
            }
            if (constructingBuild && currentBasicBuilding.IsBuildingBuilt())
            {
                StopBuilding();
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
            currentBasicBuilding = currentBuild.GetComponent<BasicBuilding>();
        }
        else
        {
            Debug.Log("Build was null");
        }
    }
    public void StopBuilding()
    {
        if (currentBasicBuilding != null && constructingBuild)
            currentBasicBuilding.UpdateActiveBuilders(false);
        currentBasicBuilding = null;
        constructingBuild = false;
        currentBuild = null;
        navAgent.SetDestination(transform.position);
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
        Debug.Log("Worker is going to harvest: " + resource.ToString() + " -- With Handler of: ");
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
    public override void DoAction()
    {
        // If we were in the middle of building, stop
        StopAction();
        base.DoAction();
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
            {
                navAgent.SetDestination(hit.point);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ConstructionLayer"))
            {
                GameObject go = hit.transform.gameObject;
                while(go.transform.parent != null)
                {
                    go = go.transform.parent.gameObject;
                }
                ConstructBuild(go);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ResourceLayer"))
            {
                Debug.Log("Hit = " + hit.ToString() + "   ---   hit.transform = " + hit.transform.ToString() + "   -----    hit.transform.gameObject = " + hit.transform.gameObject.ToString());
                HarvestResource(hit.transform.gameObject);
            }
        }
    }
}
