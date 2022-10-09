using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreeResource : BasicResource
{
    private void Update()
    {
        if ((currentPrefabProgress > 0 && currentWoodAmount == 0) ||
            (currentPrefabProgress == (progressPrefabs.Count - 1) && (startingWoodAmount - currentWoodAmount) > 0))
        {
            //Advance which prefab we are keeping active and keep the outline's state the same after the switch
            progressPrefabs[currentPrefabProgress].SetActive(false);
            currentPrefabProgress--;
            progressPrefabs[currentPrefabProgress].SetActive(true);
            if (currentOutline.enabled == true)
            {
                currentOutline.enabled = false;
                currentOutline = progressPrefabs[currentPrefabProgress].GetComponent<Outline>();
                currentOutline.enabled = true;
            }
            else
            {
                currentOutline = progressPrefabs[currentPrefabProgress].GetComponent<Outline>();
            }
            if (currentPrefabProgress == 0)
            {
                GetComponent<NavMeshObstacle>().enabled = false;
            }
        }
    }
    public override int HarvestResource(int harvestAmount, out int resourcesCollected, out ResourceType resourceType)
    {
        resourceType = ResourceType.Wood;
        return HarvestWood(harvestAmount, out resourcesCollected);
    }

    public int HarvestWood(int harvestAmount, out int resourcesCollected)
    {
        resourcesCollected = harvestAmount;
        currentWoodAmount -= harvestAmount;
        if (currentWoodAmount < 0)
        { // If we collected more resources than the tree can give, minus off what ever excess there was.
            resourcesCollected += currentWoodAmount;
        }
        if (currentWoodAmount < 0)
        {
            currentWoodAmount = 0;
        }
        return currentWoodAmount;
    }
}