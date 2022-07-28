using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneResource : ResourceHandler
{
    private void Update()
    {
        if ((currentPrefabProgress == (progressPrefabs.Count - 1) && (StonePercentageLeft() < 0.5f && StonePercentageLeft() > 0)) || // Half Depleted
            (currentPrefabProgress > 0 && currentStoneAmount == 0))                                                                  // Depleted State
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
        }
    }
    public override int HarvestResource(int harvestAmount, out int resourcesCollected, out ResourceType resourceType)
    {
        resourceType = ResourceType.Stone;
        return HarvestStone(harvestAmount, out resourcesCollected);
    }

    public int HarvestStone(int harvestAmount, out int resourcesCollected)
    {
        resourcesCollected = harvestAmount;
        currentStoneAmount -= harvestAmount;
        if (currentStoneAmount < 0)
        { // If we collected more resources than the tree can give, minus off what ever excess there was.
            resourcesCollected += currentStoneAmount;
        }
        if (currentStoneAmount < 0)
        {
            currentStoneAmount = 0;
        }
        return currentStoneAmount;
    }
    private float StonePercentageLeft()
    {
        return (float)currentStoneAmount / startingStoneAmount;
    }
}
