using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public enum Resources
    {
        WOOD = 0,
        STONE,
        FOOD
    }

    public int maxStartingWoodAmount;
    public int minStartingWoodAmount;
    public int maxStartingStoneAmount;
    public int minStartingStoneAmount;
    public int maxStartingFoodAmount;
    public int minStartingFoodAmount;
    public int startingWoodAmount;
    public int startingStoneAmount;
    public int startingFoodAmount;
    public int currentWoodAmount;
    public int currentStoneAmount;
    public int currentFoodAmount;
    private Outline outline;

    void Start()
    {
        startingWoodAmount = Random.Range(minStartingWoodAmount, maxStartingWoodAmount);
        currentWoodAmount = startingWoodAmount;
        startingStoneAmount = Random.Range(minStartingStoneAmount, maxStartingStoneAmount);
        currentStoneAmount = startingStoneAmount;
        startingFoodAmount = Random.Range(minStartingFoodAmount, maxStartingFoodAmount);
        currentFoodAmount = startingFoodAmount;
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }
    public void SelectResource()
    {
        outline.enabled = true;
    }
    public void DeselectResource()
    {
        outline.enabled = false;
    }
    public int HarvestResource(int harvestAmount, out int resourcesCollected)
    {
        resourcesCollected = 0;
        if (startingWoodAmount > 0)
        {
            return HarvestWood(harvestAmount, out resourcesCollected);
        }
        else if (startingStoneAmount > 0)
        {
            return HarvestStone(harvestAmount, out resourcesCollected);
        }
        else if (startingFoodAmount > 0)
        {
            return HarvestFood(harvestAmount, out resourcesCollected);
        }
        return 0;
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
    public int HarvestStone(int harvestAmount, out int resourcesCollected)
    {
        resourcesCollected = harvestAmount;
        currentStoneAmount -= harvestAmount;
        if (currentStoneAmount < 0)
        { // If we collected more resources than the rock can give, minus off what ever excess there was.
            resourcesCollected += currentStoneAmount;
        }
        if (currentStoneAmount < 0)
        {
            currentStoneAmount = 0;
        }
        return currentStoneAmount;
    }
    public int HarvestFood(int harvestAmount, out int resourcesCollected)
    {
        resourcesCollected = harvestAmount;
        currentFoodAmount -= harvestAmount;
        if (currentFoodAmount < 0)
        { // If we collected more resources than the animal/crop can give, minus off what ever excess there was.
            resourcesCollected += currentFoodAmount;
        }
        if (currentFoodAmount < 0)
        {
            currentFoodAmount = 0;
        }
        return currentFoodAmount;
    }
}
