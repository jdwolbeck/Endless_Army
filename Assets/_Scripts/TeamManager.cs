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
        if (teamList.Count > 0 && teamList[0].gameObject.TryGetComponent(out AITeamController aiTeamController))
        {
            teamList[0].teamNumber = 0;
            // Remove the AI component of the player's Team controller.
            Destroy(aiTeamController);
        }
        teamList.Add(Instantiate(ResourceDictionary.instance.GetPrefab("Team"), transform).GetComponent<TeamResourceManager>());
        if (teamList.Count > 1 && teamList[1].gameObject.TryGetComponent(out aiTeamController))
        {
            teamList[1].teamNumber = 1;
            Destroy(aiTeamController);
        }
        /*teamList.Add(Instantiate(ResourceDictionary.instance.GetPrefab("Team"), transform).GetComponent<TeamResourceManager>());
        if (teamList.Count > 2 && teamList[2].gameObject.TryGetComponent(out aiTeamController))
        {
            teamList[2].teamNumber = 2;
            Destroy(aiTeamController);
        }*/
    }
    /*public TeamEnum AssignTeam(int objectLayer)
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
    }*/
    public Material AssignTeamMaterial(int teamNumber)
    {
        if (teamNumber >= 0 && teamNumber <= 7)
        {
            return ResourceDictionary.instance.GetMaterial("Team" + teamNumber.ToString() + "Mat");
        }
        /*
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
        */

        throw new Exception("Unhandled object layer was passed into TeamManager.AssignTeamMaterial...");
    }
    public void InitializeTeams(MapGrid map)
    {
        RemoveAllTeams();
        for (int i = 0; i < map.MapScriptable.NumberOfTeams; i++)
        {
            teamList.Add(Instantiate(ResourceDictionary.instance.GetPrefab("Team"), transform).GetComponent<TeamResourceManager>());
            teamList[i].teamCurrentFood = map.MapScriptable.StartingFoodAmount;
            teamList[i].teamCurrentWood = map.MapScriptable.StartingWoodAmount;
            teamList[i].teamCurrentStone = map.MapScriptable.StartingStoneAmount;
            teamList[i].teamNumber = i;
            teamList[i].teamColor = GenerateTeamColor(i);
            teamList[i].startingSpawn = map.TeamSpawns[i];

            ResourceDictionary.instance.GetMaterial("Team" + i.ToString() + "Mat").color = teamList[i].teamColor;

            Vector2 gamePosition = map.TranslateCoordinatesToGameWorld(map.MapScriptable, map.TeamSpawns[i]);
            if (i == 0)
            {
                if (teamList[0].gameObject.TryGetComponent<AITeamController>(out AITeamController aiTeamController))
                {
                    // Remove the AI component of the player's Team controller.
                    Destroy(aiTeamController);
                }
                // Spawn in the TownCenter at the middle of this players spawn.
                GameObject go = GameObject.Instantiate(ResourceDictionary.instance.GetPrefab("TownCenterEGO"), new Vector3(gamePosition.x, 0, gamePosition.y), Quaternion.identity);
                go.GetComponent<BasicObject>().Team = i;
                go.layer = LayerMask.NameToLayer("PlayerBuildingLayer");
                GameHandler.instance.playerBuildings.Add(go);
                go.GetComponent<BasicBuilding>().FinishBuilding();

                // Spawn in three workers below the town center.
                for (int j = 0; j < 3; j++)
                {
                    go = Instantiate(ResourceDictionary.instance.GetPrefab("Blend_WorkerUnit"), new Vector3(gamePosition.x - 3f + (3f * j), 0, gamePosition.y - 7f), Quaternion.identity);
                    go.GetComponent<BasicObject>().Team = i;
                    go.layer = LayerMask.NameToLayer("PlayerUnitLayer");
                    GameHandler.instance.playerUnits.Add(go);
                }
            }
            else
            {
                // Spawn in the TownCenter at the middle of this players spawn.
                GameObject go = GameObject.Instantiate(ResourceDictionary.instance.GetPrefab("TownCenterEGO"), new Vector3(gamePosition.x, 0, gamePosition.y), Quaternion.identity);
                go.GetComponent<BasicObject>().Team = i;
                go.layer = LayerMask.NameToLayer("EnemyBuildingLayer");
                GameHandler.instance.enemyBuildings.Add(go);
                go.GetComponent<BasicBuilding>().FinishBuilding();

                // Spawn in three workers below the town center.
                for (int j = 0; j < 3; j++)
                {
                    go = Instantiate(ResourceDictionary.instance.GetPrefab("Blend_WorkerUnit"), new Vector3(gamePosition.x - 3f + (3f * j), 0, gamePosition.y - 7f), Quaternion.identity);
                    go.GetComponent<BasicObject>().Team = i;
                    go.layer = LayerMask.NameToLayer("EnemyUnitLayer");
                    GameHandler.instance.enemyUnits.Add(go);
                }
            }
        }
    }
    private void RemoveAllTeams()
    {
        for (int i = teamList.Count - 1; i >= 0; i--)
        {
            Destroy(teamList[i].gameObject);
        }
        teamList.Clear();
    }
    private Color GenerateTeamColor(int teamNumber)
    {
        Color color;
        switch (teamNumber)
        {
            case 0: color = Color.blue;                     break;
            case 1: color = Color.red;                      break;
            case 2: color = Color.green;                    break;
            case 3: color = new Color(.627f, .125f, 1f); break; // Purple(#A020F0)
            case 4: color = Color.yellow;                   break;
            case 5: color = new Color(1f, .529f, 0);        break; // Orange(#FFA500)
            case 6: color = Color.cyan;                     break;
            case 7: color = new Color(.984f, .475f, .702f); break; // Pink(#FB79B3)
            default:
                float randRed = UnityEngine.Random.Range(0f, 1f);
                float randBlue = UnityEngine.Random.Range(0f, 1f);
                float randGreen = UnityEngine.Random.Range(0f, 1f);
                color = new Color(randRed, randGreen, randBlue);
                break;
        }
        return color;
    }
}
