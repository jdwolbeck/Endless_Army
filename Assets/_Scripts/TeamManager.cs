using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;
    public List<TeamResourceManager> teamList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        teamList.Add(Instantiate(ResourceDictionary.instance.GetPrefab("Team"), transform).GetComponent<TeamResourceManager>());
        if (teamList.Count > 0 && teamList[0].gameObject.TryGetComponent<AITeamController>(out AITeamController aiTeamController))
        {
            teamList[0].teamNumber = 0;
            // Remove the AI component of the player's Team controller.
            Destroy(aiTeamController);
        }
    }
    public TeamEnum AssignTeam(int objectLayer)
    {
        if (objectLayer == LayerMask.NameToLayer("PlayerUnitLayer") ||
            objectLayer == LayerMask.NameToLayer("PlayerBuildingLayer"))
        {
            return TeamEnum.Player;
        }
        else if (objectLayer == LayerMask.NameToLayer("EnemyUnitLayer") ||
                 objectLayer == LayerMask.NameToLayer("EnemyBuildingLayer"))
        {
            return TeamEnum.Enemy;
        }

        Debug.Log("Unhandled object layer was passed into TeamManager.AssignTeam...");
        return TeamEnum.Unknown;
    }
    public Material AssignTeamMaterial(int objectLayer)
    {
        if (objectLayer == LayerMask.NameToLayer("PlayerUnitLayer") ||
            objectLayer == LayerMask.NameToLayer("PlayerBuildingLayer"))
        {
            return ResourceDictionary.instance.GetMaterial("TeamPlayerMat");
        }
        else if (objectLayer == LayerMask.NameToLayer("EnemyUnitLayer") ||
                 objectLayer == LayerMask.NameToLayer("EnemyBuildingLayer"))
        {
            return ResourceDictionary.instance.GetMaterial("TeamEnemyMat");
        }

        throw new Exception("Unhandled object layer was passed into TeamManager.AssignTeamMaterial...");
    }
    public void InitializeTeams(ScriptableMap map)
    {
        RemoveAllTeams();
        for (int i = 0; i < map.NumberOfTeams; i++)
        {
            teamList.Add(Instantiate(ResourceDictionary.instance.GetPrefab("Team"), transform).GetComponent<TeamResourceManager>());
            teamList[i].teamCurrentFood = map.StartingFoodAmount;
            teamList[i].teamCurrentWood = map.StartingWoodAmount;
            teamList[i].teamCurrentStone = map.StartingStoneAmount;
            teamList[i].teamNumber = i;
            if (i == 0)
            {
                teamList[0].teamColor = Color.blue;
            }
            else if (i == 1)
            {
                teamList[1].teamColor = Color.red;
            }
            else
            {
                float randRed = UnityEngine.Random.Range(0f, 1f);
                float randBlue = UnityEngine.Random.Range(0f, 1f);
                float randGreen = UnityEngine.Random.Range(0f, 1f);
                teamList[i].teamColor = new Color(randRed, randGreen, randBlue);
            }

            if (i == 0)
            {
                if (teamList[0].gameObject.TryGetComponent<AITeamController>(out AITeamController aiTeamController))
                {
                    // Remove the AI component of the player's Team controller.
                    Destroy(aiTeamController);
                }
            }
        }
    }
    private void RemoveAllTeams()
    {
        for (int i = 0; i < teamList.Count; i++)
        {
            Destroy(teamList[0].gameObject);
            teamList.RemoveAt(0);
        }
        teamList.Clear();
    }
}
