using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamResourceManager : MonoBehaviour
{
    public int playerCurrentFood;
    public int playerCurrentWood;
    public int playerCurrentStone;

    private void Awake()
    {
        playerCurrentFood = 500;
        playerCurrentWood = 500;
        playerCurrentStone = 500;
    }
    public void UpdateTeamFood(int foodAmount)
    {
        playerCurrentFood += foodAmount;
        if (playerCurrentFood < 0)
        {
            playerCurrentFood = 0;
        }
    }
    public void UpdateTeamWood(int woodAmount)
    {
        playerCurrentWood += woodAmount;
        if (playerCurrentWood < 0)
        {
            playerCurrentWood = 0;
        }
    }
    public void UpdateTeamStone(int stoneAmount)
    {
        playerCurrentStone += stoneAmount;
        if (playerCurrentStone < 0)
        {
            playerCurrentStone = 0;
        }
    }
}
