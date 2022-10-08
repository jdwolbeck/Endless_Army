using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamResourceManager : MonoBehaviour
{
    public int teamCurrentFood;
    public int teamCurrentWood;
    public int teamCurrentStone;
    public int teamNumber;
    public Color teamColor;
    public List<GameObject> unitList;
    public List<GameObject> buildingList;

    private void Awake()
    {
        teamCurrentFood = 500;
        teamCurrentWood = 500;
        teamCurrentStone = 500;
        teamColor = Color.white;
    }
    public void UpdateTeamFood(int foodAmount)
    {
        teamCurrentFood += foodAmount;
        if (teamCurrentFood < 0)
        {
            teamCurrentFood = 0;
        }
    }
    public void UpdateTeamWood(int woodAmount)
    {
        teamCurrentWood += woodAmount;
        if (teamCurrentWood < 0)
        {
            teamCurrentWood = 0;
        }
    }
    public void UpdateTeamStone(int stoneAmount)
    {
        teamCurrentStone += stoneAmount;
        if (teamCurrentStone < 0)
        {
            teamCurrentStone = 0;
        }
    }
}
