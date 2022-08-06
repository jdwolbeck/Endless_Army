using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableMap")]
public class ScriptableMap : ScriptableObject
{
    public int NumberOfPlayers;
    public int StartingFoodAmount;
    public int StartingWoodAmount;
    public int StartingStoneAmount;
    public Color groundColor;
}
