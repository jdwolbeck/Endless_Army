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
    public GameObject currentResource;
    private BasicResource resourceHandler;
    protected bool harvestingWood;
    protected bool harvestingStone;
    protected bool harvestingBerries;
    private bool isHarvesting;
    private int harvestRange;
    private float harvestCooldownTime;
    private float nextHarvestTime;
    private int harvestAmount;
    private float nextWorkerAnimUpdate;
    private bool aiHarvesting;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        buildRange = 4;
        harvestRange = 3;
        harvestCooldownTime = 1.5f;
        harvestAmount = 10;
        equippedItemManager.SetDefaultEquipment((ScriptableItem)ResourceDictionary.instance.GetPreset("Hammer"));
        equippedItemManager.EquipDefaultEquipment(EquipmentSlot.RightWeapon);
        //if (isSpawnedFromInspector)
        LoadFromPreset((ScriptableUnit)ResourceDictionary.instance.GetPreset("Worker"));
    }
    protected override void Update()
    {
        base.Update();
        if (animatorPresent && Time.time > nextWorkerAnimUpdate)
        {
            nextWorkerAnimUpdate = Time.time + 0.083f; // 5/60 -- 5 times a second at 
            //Debug.Log("HarvestingBerries = " + animator.GetBool("HarvestingBerries") + "   ====   harvestingBerries " + harvestingBerries);
            animator.SetBool("HarvestingWood", harvestingWood);
            animator.SetBool("HarvestingStone", harvestingStone);
            animator.SetBool("HarvestingBerries", harvestingBerries);
            animator.SetBool("ConstructingBuild", constructingBuild);
        }
        if (currentBuild != null)
        {
            if (constructingBuild == false && Vector3.Distance(gameObject.transform.position, currentBuild.transform.position) <= buildRange)
            {
                // Look at the building we're constructing
                transform.LookAt(currentBuild.transform, new Vector3 (0, 1, 0));
                navAgent.SetDestination(transform.position);
                constructingBuild = true;
                equippedItemManager.Equip((ScriptableItem)ResourceDictionary.instance.GetPreset("Hammer"));
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

                if (animatorPresent)
                {
                    ResourceType resourceType;
                    int resourcesObtained;
                    resourceHandler.HarvestResource(0, out resourcesObtained, out resourceType);
                    if (resourcesObtained > 0)
                    {
                        Debug.Log("Uh oh, boyo, too many resources collected for boolean setting??");
                    }
                    switch (resourceType)
                    {
                        case ResourceType.Food:
                            harvestingBerries = true;
                            equippedItemManager.Unequip(EquipmentSlot.RightWeapon);
                            equippedItemManager.Unequip(EquipmentSlot.LeftWeapon);
                            break;
                        case ResourceType.Wood:
                            harvestingWood = true;
                            equippedItemManager.Equip((ScriptableItem)ResourceDictionary.instance.GetPreset("LumberAxe"));
                            break;
                        case ResourceType.Stone:
                            harvestingStone = true;
                            equippedItemManager.Equip((ScriptableItem)ResourceDictionary.instance.GetPreset("Pickaxe"));
                            break;
                    }
                }
                //Debug.Log("Worker started harvesting " + currentResource.ToString());
            }
            if (Vector3.Distance(gameObject.transform.position, currentResource.transform.position) <= harvestRange)
            {
                // Look at the resource we're harvesting
                Quaternion lookOnLook = Quaternion.LookRotation(currentResource.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3);
            }
            if (Time.time > nextHarvestTime && isHarvesting)
            {
                nextHarvestTime = Time.time + harvestCooldownTime;

                int resourcesObtained;
                ResourceType resourceType;
                int resourcesRemaining = resourceHandler.HarvestResource(harvestAmount, out resourcesObtained, out resourceType);
                switch (resourceType)
                {
                    case ResourceType.Food:
                        TeamManager.instance.teamList[Team].UpdateTeamFood(resourcesObtained);
                        break;
                    case ResourceType.Wood:
                        TeamManager.instance.teamList[Team].UpdateTeamWood(resourcesObtained);
                        break;
                    case ResourceType.Stone:
                        TeamManager.instance.teamList[Team].UpdateTeamStone(resourcesObtained);
                        break;
                }
                if (resourcesRemaining <= 0)
                {
                    if (aiHarvesting)
                    {
                        if(gameObject.TryGetComponent(out AIWorkerUnit aiWorker))
                        {
                            switch (resourceType)
                            {
                                case ResourceType.Food:
                                    aiWorker.AddNewAction(AIAction.GatherNearestBush, false);
                                    break;
                                case ResourceType.Wood:
                                    aiWorker.AddNewAction(AIAction.ChopNearestTree, false);
                                    break;
                                case ResourceType.Stone:
                                    aiWorker.AddNewAction(AIAction.MineNearestStone, false);
                                    break;
                            }
                        }
                    }
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
        equippedItemManager.EquipDefaultEquipment(EquipmentSlot.RightWeapon);
        equippedItemManager.EquipDefaultEquipment(EquipmentSlot.LeftWeapon);
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
            resourceHandler = currentResource.GetComponent<BasicResource>();
            if (resourceHandler == null)
            {
                Debug.Log("Resource handler was null for resource " + resource.ToString());
            }
            //Debug.Log("Setting worker destination to " + resource.transform.position.ToString());
        }
        else
        {
            Debug.Log("Resource was null");
        }
        //Debug.Log("Worker is going to harvest: " + resource.ToString() + " -- With Handler of: ");
    }
    public void StopHarvesting()
    {
        resourceHandler = null;
        isHarvesting = false;
        harvestingBerries = false;
        harvestingStone = false;
        harvestingWood = false;
        currentResource = null;
        aiHarvesting = false;
        equippedItemManager.EquipDefaultEquipment(EquipmentSlot.RightWeapon);
        equippedItemManager.EquipDefaultEquipment(EquipmentSlot.LeftWeapon);
        //Debug.Log("Stopping harvesting...");
    }
    public void StopAction()
    {
        if (currentBuild != null)
        {
            StopBuilding();
            equippedItemManager.EquipDefaultEquipment(EquipmentSlot.RightWeapon);
        }
        if (currentResource != null)
        {
            StopHarvesting();
        }
        if (HasActiveTarget())
        {
            ClearTarget(currentTarget.gameObject);
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

        if (Physics.Raycast(ray, out hit, 10000))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
            {
                navAgent.SetDestination(hit.point);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ConstructionLayer"))
            {
                GameObject go = hit.transform.gameObject;
                while (go.transform.parent != null)
                {
                    go = go.transform.parent.gameObject;
                }
                ConstructBuild(go);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ResourceLayer"))
            {
                //Debug.Log("Hit = " + hit.ToString() + "   ---   hit.transform = " + hit.transform.ToString() + "   -----    hit.transform.gameObject = " + hit.transform.gameObject.ToString());
                HarvestResource(hit.transform.gameObject);
                aiHarvesting = true;
            }
        }
    }
    public void SetPrioritization(string resourceType)
    {
        switch (resourceType)
        {
            case "Food":
                break;
            case "Wood":
                break;
            case "Stone":
                break;
        }
    }
    public bool IsBusy()
    {
        if (currentBuild == null && currentResource == null && currentTarget == null)
        {
            return false;
        }
        return true;
    }
    public bool ChopNearestTree()
    {
        StopAction();
        return FindNearestResource(typeof(TreeResource));
    }
    public bool GatherNearestBush()
    {
        StopAction();
        return FindNearestResource(typeof(BushResource));
    }
    public bool MineNearestStone()
    {
        StopAction();
        return FindNearestResource(typeof(StoneResource));
    }
    private bool FindNearestResource(System.Type resourceType)
    {
        int loopBreak = 0;
        while (loopBreak < 25)
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, 0.5f + loopBreak, LayerMask.GetMask("ResourceLayer"));
            GameObject go;
            for (int i = 0; i < rangeChecks.Length; i++)
            {
                go = rangeChecks[i].gameObject;
                while (go.transform.parent != null)
                {
                    go = go.transform.parent.gameObject;
                }
                BasicResource foundResource;
                if (go.TryGetComponent(out foundResource))
                {
                    if (foundResource.GetType() == resourceType && 
                       (foundResource.currentFoodAmount > 0 || foundResource.currentWoodAmount > 0 || foundResource.currentStoneAmount > 0))
                    {
                        HarvestResource(foundResource.gameObject);
                        return true;
                    }
                }
            }
            loopBreak++;
        }
        return false;
    }
}
