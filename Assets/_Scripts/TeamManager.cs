using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;
    private Material TeamPlayerMat;
    private Material TeamEnemyMat;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        TeamPlayerMat = Resources.Load("Materials/TeamPlayerMat") as Material;
        TeamEnemyMat = Resources.Load("Materials/TeamEnemyMat") as Material;
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
            return TeamPlayerMat;
        }
        else if (objectLayer == LayerMask.NameToLayer("EnemyUnitLayer") ||
                 objectLayer == LayerMask.NameToLayer("EnemyBuildingLayer"))
        {
            return TeamEnemyMat;
        }

        throw new Exception("Unhandled object layer was passed into TeamManager.AssignTeamMaterial...");
    }
}
