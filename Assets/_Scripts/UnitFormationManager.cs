using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class ArmyInfo
{
    public int formationType;
    public int armySize;
    public int armyPosition;
    public Vector2 currentArmyCenter;
    public ArmyInfo()
    {
        formationType = -1;
        armySize = 0;
        armyPosition = 0;
        currentArmyCenter = Vector2.zero;
    }
}
public class UnitFormationManager : MonoBehaviour
{
    public static UnitFormationManager instance;
    //public ArmyInfo armyInfo;

    public List<BasicUnit> armyList;
    public List<Vector3> armyPositionList;
    public int formationType;
    public int armySize;
    public int armyPosition;
    public Vector2 currentArmyCenter;
    public Vector3 currentDestination;

    private const float UNIT_WIDTH_ROW = 1.5f;
    private const float UNIT_WIDTH_COLUMN = 1.5f;
    private const int LINE_COLUMN_COUNT = 10;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        armyList = new List<BasicUnit>();
        armyPositionList = new List<Vector3>();
        ClearActiveArmy();
    }
    public void SetActiveArmy()
    {
        // TODO logic for handling different formation types
        formationType = 0;
        armyList.Clear();
        armyPositionList.Clear();

        // Find the center of all selected units
        Vector3 temp = Vector3.zero;
        BasicUnit bu;
        for (int i = 0; i < InputHandler.instance.selectedUnits.Count; i++)
        {
            bu = InputHandler.instance.selectedUnits[i];
            temp += bu.transform.position;
            armyList.Add(bu);
            bu.SetFormation(true);
        }
        temp /= InputHandler.instance.selectedUnits.Count;
        currentArmyCenter = new Vector2(temp.x, temp.z);
        armySize = armyList.Count;
    }
    public void ClearActiveArmy()
    {
        formationType = -1;
        armyList.Clear();
        armyPositionList.Clear();
        formationType = -1;
        armySize = 0;
        armyPosition = 0;
        currentArmyCenter = Vector2.zero;
        currentDestination = Vector3.zero;
    }
    public void SetArmyMoveLocation(Vector3 destinationCoords)
    {
        if (destinationCoords == currentDestination)
            return;
        else
            currentDestination = destinationCoords;

        SetActiveArmy();
        Vector2 destination = new Vector2(destinationCoords.x, destinationCoords.z);
        switch (formationType)
        {
            case 0:
                Debug.Log("Starting position: " + currentArmyCenter.ToString() + " -> Destination: " + destination.ToString());
                // Get the line equation of the original vector (original center -> destination center)
                // Slope of original vector
                float origSlope = (destination.y - currentArmyCenter.y) / (destination.x - currentArmyCenter.x);
                // Intercept of original vector
                float origIntercept = currentArmyCenter.y - (origSlope * currentArmyCenter.x);
                //Debug.Log("Original Equation: Y = " + origSlope + "X + " + origIntercept);
                // Equation of original vector: Y = origSlope (X) + origIntercept

                // Get the line equation of the perpendicular line to the original vector at the point, destination center
                // Slope of perpendicular line
                float destSlope = -1 / origSlope;
                // Intercept of the perpendicular line
                float destIntercept = destination.y - (destSlope * destination.x);
                // Equation of the perpendicular line: Y = destSlope (X) + destIntercept
                //Debug.Log("Destination Equation: Y = " + destSlope + "X + " + destIntercept);

                /*
                * Now that we have the equation of the line we are going to align our people onto,
                * we need to determine any vector along both ways of this line (to and from the destination point).
                * Once we have this vector we can normalize it and utilize this equation:
                * P1 = P0 +/- du, where d = UNIT_COLUMN_WIDTH * whatever place this unit is and u = the normalized vector..
                */
                /* Positive direction vector; Choose a random x (destination + 1 here) and use y = mx + b to solve for y
                * This will result in a new point in the positive direction down the Destination Equation. Now, use the two
                * points to create a vector.
                */
                Vector2 posDirVec = new Vector2(destination.x + 1, (destSlope * (destination.x + 1)) + destIntercept);
                //Debug.Log("PosDirVector Point: " + posDirVec.ToString());
                Debug.DrawRay(new Vector3(posDirVec.x , 0, posDirVec.y), new Vector3(0, 1, 0), Color.green, 120);
                
                Vector2 posPoint = posDirVec;
                posDirVec = new Vector2(posPoint.x - destination.x, posPoint.y - destination.y);
                posDirVec.Normalize();

                // Positive direction vector; Choose a random x (destination - 1 here) and use y = mx + b to solve for y
                Vector2 negDirVec = new Vector2(destination.x - 1, (destSlope * (destination.x - 1)) + destIntercept);
                negDirVec = new Vector2(negDirVec.x - destination.x, negDirVec.y - destination.y);
                negDirVec.Normalize();

                for (int armyPos = 0; armyPos < armyList.Count; armyPos++)
                {
                    destination = new Vector2(destinationCoords.x, destinationCoords.z);
                    // Determine how many unit positions left or right this specific unit is
                    float centerUnit = (armySize - 1) / 2f;
                    float unitOffset = (centerUnit - armyPos) * UNIT_WIDTH_COLUMN;
                    //Debug.Log("Center unit: " + centerUnit + "  -  unitOffset: " + unitOffset + " [armyPosition: " + armyPos + "]");

                    // Utilize the vector equation to get this units position
                    // destination is already set to destinationCoords
                    destination += unitOffset * posDirVec;
                    if (armyPos == armySize - 1)
                    {
                        Debug.DrawRay(destinationCoords, new Vector3(0, 1, 0), Color.magenta, 120);
                        Debug.DrawRay(new Vector3(currentArmyCenter.x, 0, currentArmyCenter.y), new Vector3(0, 1, 0), Color.yellow, 120);
                        Debug.DrawRay(new Vector3(currentArmyCenter.x, 0, currentArmyCenter.y),
                            new Vector3(destinationCoords.x - currentArmyCenter.x, 0, destinationCoords.z - currentArmyCenter.y),
                            Color.blue, 120); // orig line
                        Debug.DrawRay(destinationCoords,
                            new Vector3(4 * posDirVec.x, 0, 4 * posDirVec.y),
                            Color.red, 120); // dest line
                        Debug.DrawRay(destinationCoords,
                            new Vector3(-4 * posDirVec.x, 0, -4 * posDirVec.y),
                            Color.yellow, 120); // dest line
                        Debug.DrawRay(destinationCoords,
                            new Vector3(posPoint.x - destinationCoords.x, 0, posPoint.y - destinationCoords.z),
                            Color.black, 120); // dest line
                    }
                    armyPositionList.Add(new Vector3(destination.x, 0, destination.y));
                    Debug.Log("Adding vector: " + new Vector3(destination.x, 0, destination.y).ToString() + " listLength = " + armyPositionList.Count);
                }
                break;
        }
    }
    public Vector3 GetUnitMoveLocation(GameObject go)
    {
        for(int i = 0; i < armyList.Count; i++)
        {
            if (armyList[i].gameObject == go)
            {
                Debug.Log("Returning index " + i + " with vector: " + armyPositionList[i].ToString());
                return armyPositionList[i];
            }
        }
        return Vector3.zero;
    }
}
