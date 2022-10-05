using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableMap")]
public class ScriptableMap : ScriptableObject
{    
    public int StartingFoodAmount;
    public int StartingWoodAmount;
    public int StartingStoneAmount;
    public int NumberOfTeams;
    public int MapWidth;
    public int MapHeight;
    public int SpawnRadius;
    public int InitialTreePoints;
    public int InitialStonePoints;
    public int InitialBushPoints;
    public float ResourceSpreadFactor;
    public float TreeDampeningFactor;
    public float StoneDampeningFactor;
    public float BushDampeningFactor;
    public float MinTreeFill;
    public float MinStoneFill;
    public float MinBushFill;
    public Color GroundColor;
    public List<Vector2> TeamSpawns;
}
