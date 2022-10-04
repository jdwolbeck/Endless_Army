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
        for (int i = 0; i < map.NumberOfPlayers; i++)
        {
            teamList.Add(Instantiate(ResourceDictionary.instance.GetPrefab("Team"), transform).GetComponent<TeamResourceManager>());
            teamList[i].playerCurrentFood = map.StartingFoodAmount;
            teamList[i].playerCurrentWood = map.StartingWoodAmount;
            teamList[i].playerCurrentStone = map.StartingStoneAmount;
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
