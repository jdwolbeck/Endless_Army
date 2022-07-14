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
    private Outline outline;

    void Start()
    {
        startingWoodAmount = Random.Range(minStartingWoodAmount, maxStartingWoodAmount);
        startingStoneAmount = Random.Range(minStartingStoneAmount, maxStartingStoneAmount);
        startingFoodAmount = Random.Range(minStartingFoodAmount, maxStartingFoodAmount);
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }
    void Update()
    {
        
    }
    public void SelectResource()
    {
        outline.enabled = true;
    }
    public void DeselectResource()
    {
        outline.enabled = false;
    }
}
