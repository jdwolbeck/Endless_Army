using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushResource : ResourceHandler
{
    private void Update()
    {
        if ((currentPrefabProgress == (progressPrefabs.Count - 1) && (FoodPercentageLeft() < 1f    && FoodPercentageLeft() >= 0.75f)) || // Just started harvesting Bush
            (currentPrefabProgress == (progressPrefabs.Count - 2) && (FoodPercentageLeft() < 0.75f && FoodPercentageLeft() >= 0.5f )) || // 3/4 State
            (currentPrefabProgress == (progressPrefabs.Count - 3) && (FoodPercentageLeft() < 0.5f  && FoodPercentageLeft() >= 0.25f)) || // Half State
            (currentPrefabProgress == (progressPrefabs.Count - 4) && (FoodPercentageLeft() < 0.25f && FoodPercentageLeft() > 0     )) || // Quarter State
            (currentPrefabProgress > 0 && currentFoodAmount == 0))                                                                       // Depleted State
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
        resourceType = ResourceType.Food;
        return HarvestFood(harvestAmount, out resourcesCollected);
    }

    public int HarvestFood(int harvestAmount, out int resourcesCollected)
    {
        resourcesCollected = harvestAmount;
        currentFoodAmount -= harvestAmount;
        if (currentFoodAmount < 0)
        { // If we collected more resources than the tree can give, minus off what ever excess there was.
            resourcesCollected += currentFoodAmount;
        }
        if (currentFoodAmount < 0)
        {
            currentFoodAmount = 0;
        }
        return currentFoodAmount;
    }
    private float FoodPercentageLeft()
    {
        return (float)currentFoodAmount / startingFoodAmount;
    }
}
