using System.Collections;
using System.Collections.Generic;
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
public class UnitFormation
{
    public UnitFormation instance = null;
    public List<ArmyInfo> armysList;
    private const float UNIT_WIDTH_ROW = 1.5f;
    private const float UNIT_WIDTH_COLUMN = 1.5f;
    private const int LINE_COLUMN_COUNT = 10;

    public UnitFormation()
    {
        if (instance == null)
        {
            instance = new UnitFormation();
        }
        armysList = new List<ArmyInfo>();
    }
    public Vector3 GetMoveLocation(int armyIndex, Vector3 destinationCoords)
    {
        // Invalid armyIndex was passed in
        if (armyIndex >= armysList.Count)
            return Vector3.zero;

        Vector2 destination = new Vector2(destinationCoords.x, destinationCoords.z);
        switch (armysList[armyIndex].formationType)
        {
            case 0:
                Debug.Log("Starting position: " + armysList[armyIndex].currentArmyCenter.ToString() + " -> Destination: " + destination.ToString());
                // Get the line equation of the original vector (original center -> destination center)
                // Slope of original vector
                float origSlope = (destination.y - armysList[armyIndex].currentArmyCenter.y) / (destination.x - armysList[armyIndex].currentArmyCenter.x);
                // Intercept of original vector
                float origIntercept = armysList[armyIndex].currentArmyCenter.y - (origSlope * armysList[armyIndex].currentArmyCenter.x);
                Debug.Log("Original Equation: Y = " + origSlope + "X + " + origIntercept);
                // Equation of original vector: Y = origSlope (X) + origIntercept

                // Get the line equation of the perpendicular line to the original vector at the point, destination center
                // Slope of perpendicular line
                float destSlope = -1 / origSlope;
                // Intercept of the perpendicular line
                float destIntercept = destination.y - (destSlope * destination.x);
                // Equation of the perpendicular line: Y = destSlope (X) + destIntercept
                Debug.Log("Destination Equation: Y = " + destSlope + "X + " + destIntercept);

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
                Debug.Log("PosDirVector Point: " + posDirVec.ToString());
                if (armysList[armyIndex].armyPosition == armysList[armyIndex].armySize - 1)
                {
                    Debug.DrawRay(new Vector3(posDirVec.x , 0, posDirVec.y), new Vector3(0, 1, 0), Color.green, 120);
                }
                Vector2 posPoint = posDirVec;
                posDirVec = new Vector2(posPoint.x - destinationCoords.x, posPoint.y - destinationCoords.z);
                posDirVec.Normalize();

                // Positive direction vector; Choose a random x (destination - 1 here) and use y = mx + b to solve for y
                Vector2 negDirVec = new Vector2(destination.x - 1, (destSlope * (destination.x - 1)) + destIntercept);
                negDirVec = new Vector2(negDirVec.x - destinationCoords.x, negDirVec.y - destinationCoords.z);
                negDirVec.Normalize();

                // Determine how many unit positions left or right this specific unit is
                float centerUnit = (armysList[armyIndex].armySize - 1) / 2f;
                float unitOffset = (centerUnit - armysList[armyIndex].armyPosition) * UNIT_WIDTH_COLUMN;
                Debug.Log("Center unit: " + centerUnit + "  -  unitOffset: " + unitOffset + " [armyPosition: " + armysList[armyIndex].armyPosition + "]");

                // Utilize the vector equation to get this units position
                // destination is already set to destinationCoords
                destination += unitOffset * posDirVec;
                if (armysList[armyIndex].armyPosition == armysList[armyIndex].armySize - 1)
                {
                    Debug.DrawRay(destinationCoords, new Vector3(0, 1, 0), Color.magenta, 120);
                    Debug.DrawRay(new Vector3(armysList[armyIndex].currentArmyCenter.x, 0, armysList[armyIndex].currentArmyCenter.y), new Vector3(0, 1, 0), Color.yellow, 120);
                    Debug.DrawRay(new Vector3(armysList[armyIndex].currentArmyCenter.x, 0, armysList[armyIndex].currentArmyCenter.y), 
                        new Vector3(destinationCoords.x - armysList[armyIndex].currentArmyCenter.x, 0, destinationCoords.z - armysList[armyIndex].currentArmyCenter.y), 
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
                break;
            default:
                destination = Vector2.zero;
                break;
        }
        return new Vector3(destination.x, 0, destination.y);
    }
}
