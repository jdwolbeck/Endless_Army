using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceManger : MonoBehaviour
{
    public static PlayerResourceManger instance;
    public int playerCurrentFood;
    public int playerCurrentWood;
    public int playerCurrentStone;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void UpdatePlayerFood(int foodAmount)
    {
        playerCurrentFood += foodAmount;
        if (playerCurrentFood < 0)
        {
            playerCurrentFood = 0;
        }
    }
    public void UpdatePlayerWood(int woodAmount)
    {
        playerCurrentWood += woodAmount;
        if (playerCurrentWood < 0)
        {
            playerCurrentWood = 0;
        }
    }
    public void UpdatePlayerStone(int stoneAmount)
    {
        playerCurrentStone += stoneAmount;
        if (playerCurrentStone < 0)
        {
            playerCurrentStone = 0;
        }
    }
}
