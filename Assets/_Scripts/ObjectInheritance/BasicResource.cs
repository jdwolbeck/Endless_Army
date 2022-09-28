using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicResource : MonoBehaviour
{
    [SerializeField]
    protected int maxStartingFoodAmount;
    [SerializeField]
    protected int minStartingFoodAmount;
    [SerializeField]
    protected int maxStartingWoodAmount;
    [SerializeField]
    protected int minStartingWoodAmount;
    [SerializeField]
    protected int maxStartingStoneAmount;
    [SerializeField]
    protected int minStartingStoneAmount;

    protected int startingFoodAmount;
    protected int startingWoodAmount;
    protected int startingStoneAmount;
    protected int currentFoodAmount;
    protected int currentWoodAmount;
    protected int currentStoneAmount;

    [SerializeField]
    protected List<GameObject> progressPrefabs = new List<GameObject>();
    protected int currentPrefabProgress;
    protected Outline currentOutline;

    void Awake()
    {
        startingFoodAmount = Random.Range(minStartingFoodAmount, maxStartingFoodAmount);
        currentFoodAmount = startingFoodAmount;
        startingWoodAmount = Random.Range(minStartingWoodAmount, maxStartingWoodAmount);
        currentWoodAmount = startingWoodAmount;
        startingStoneAmount = Random.Range(minStartingStoneAmount, maxStartingStoneAmount);
        currentStoneAmount = startingStoneAmount;

        if (progressPrefabs.Count == 0)
        {
            throw new UnassignedReferenceException("No prefabs were added to the Progress Prefabs List...");
        }
        currentPrefabProgress = progressPrefabs.Count - 1;
        currentOutline = progressPrefabs[currentPrefabProgress].GetComponent<Outline>();
    }
    public void SelectResource()
    {
        currentOutline.enabled = true;
    }
    public void DeselectResource()
    {
        currentOutline.enabled = false;
    }
    public virtual int HarvestResource(int harvestAmount, out int resourcesCollected, out ResourceType resourceType)
    {
        throw new System.Exception("Base class does not implement HarvestResource");
    }
}

public enum ResourceType
{
    Food,
    Wood,
    Stone
}