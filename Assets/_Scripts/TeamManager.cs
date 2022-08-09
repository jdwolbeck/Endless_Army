using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
}
