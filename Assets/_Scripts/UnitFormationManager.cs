using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class UnitFormation
{
    public int formationType;
    public int armySize;
    public int armyPosition;
    public Vector2 currentArmyCenter;
    private const float UNIT_WIDTH_ROW = 1.5f;
    private const float UNIT_WIDTH_COLUMN = 1.5f;
    private const int LINE_COLUMN_COUNT = 10;

    public UnitFormation()
    {
        formationType = -1;
        armySize = 0;
        armyPosition = 0;
        currentArmyCenter = Vector2.zero;
    }
    public Vector3 GetMoveLocation(Vector3 destinationCoords)
    {
        //Vector3 destination = destinationCoords;
        Vector2 destination = new Vector2(destinationCoords.x, destinationCoords.z);
        switch (formationType)
        {
            case 0:
                if (false)
                {
                    // Line formation
                    int rows = GetRowCount();
                    int columns = GetColumnCount();
                    int myRow = Mathf.CeilToInt(armyPosition / (float)LINE_COLUMN_COUNT);
                    int myColumn = armyPosition % LINE_COLUMN_COUNT;
                    Vector2 unitCoords = new Vector2(armyPosition % LINE_COLUMN_COUNT, armyPosition / LINE_COLUMN_COUNT);
                    //Debug.Log("Unit " + armyPosition + " has coordinates of (" + unitCoords.ToString() + ")");
                    // Get the middle row and middle column.
                    float centerColumn = (columns - 1) / 2f;
                    float centerMyColumn = (GetThisUnitsColumnCount() - 1) / 2f;
                    float centerRow = (rows - 1) / 2f;
                    float unitColumnOffset = (unitCoords.x - centerMyColumn) * UNIT_WIDTH_COLUMN;
                    float unitRowOffset = (unitCoords.y - centerRow) * UNIT_WIDTH_ROW;
                    destination.x += unitColumnOffset;
                    destination.y += unitRowOffset;
                    Debug.Log("Setting dest: " + destination.ToString());
                    //Debug.Log("From an original destination: " + origDestination.ToString() + "  -- adding on: (column, row) (" + 
                    //unitColumnOffset + ", " + unitRowOffset + ")");
                }
                else if (false)
                {
                    Debug.Log("Starting position: " + currentArmyCenter.ToString() + " -> Destination: " + destination.ToString());
                    // Get the line equation of the original vector (original center -> destination center)
                    // Slope of original vector
                    float origSlope = (destination.y - currentArmyCenter.y) / (destination.x - currentArmyCenter.x);
                    // Intercept of original vector
                    float origIntercept = currentArmyCenter.y - (origSlope * currentArmyCenter.x);
                    Debug.Log("Original Equation: Y = " + origSlope + "X + " + origIntercept);
                    // Equation of original vector: Y = origSlope (X) + origIntercept

                    // Get the line equation of the perpendicular line to the original vector at the point, destination center
                    // Slope of perpendicular line
                    float destSlope = -1 / origSlope;
                    // Intercept of the perpendicular line
                    float destIntercept = destination.y - (destSlope * destination.x);
                    // Equation of the perpendicular line: Y = destSlope (X) + destIntercept
                    Debug.Log("Destination Equation: Y = " + destSlope + "X + " + destIntercept);

                    // Equation 1: M*X2 - Y2 = -B + D * sqrt(m^2 + 1)
                    // Equation 2: M*X2 - Y2 = -B - D * sqrt(m^2 + 1)
                    // Equation 3: X2 - X1 = -m * (Y2 - Y1)
                    // Eq 1: unitPointY = destSlope * unitPointX + destIntercept - (UNIT_WIDTH_ROW * Mathf.Sqrt((destSlope * destSlope) + 1));
                    // Eq 2: unitPointY = destSlope * unitPointX + destIntercept + (UNIT_WIDTH_ROW * Mathf.Sqrt((destSlope * destSlope) + 1));
                    // Eq 3: unitPointX = destination.x - (destSlope * (unitPointY - destination.y))
                    // Eq 4: unitPointY = ((unitPointX - destination.x) / -destSlope) + destination.y
                    // Eq 5: unitPointX = ((destSlope * destIntercept) - (destSlope * UNIT_WIDTH_ROW * Mathf.Sqrt((destSlope * destSlope) + 1)) - (destSlope * destination.y) + destination.x) / ((destSlope * destSlope) + 1)

                    // Solve equations 1 & 3 for first possible point
                    float unitPointX = (destSlope * destIntercept) - (destSlope * UNIT_WIDTH_ROW * armyPosition * Mathf.Sqrt((destSlope * destSlope) + 1)) - (destSlope * destination.y) + destination.x;
                    unitPointX      /= (destSlope * destSlope) + 1;
                    float unitPointY = ((unitPointX - destination.x) / -destSlope) + destination.y;

                    float unitPointX1 = (destination.x + ((destSlope * UNIT_WIDTH_ROW) * Mathf.Sqrt((destSlope * destSlope) + 1)) + (destSlope * destination.y)) / ((destSlope * destSlope) + 1);
                    float unitPointY1 = ((unitPointX1 - destination.x) / -destSlope) + destination.y;

                    // Solve equations 2 & 3 for second possible point
                    float unitPointX2 = (destination.x - ((destSlope * UNIT_WIDTH_ROW) * Mathf.Sqrt((destSlope * destSlope) + 1)) + (destSlope * destination.y)) / ((destSlope * destSlope) + 1);
                    float unitPointY2 = ((unitPointX2 - destination.x) / -destSlope) + destination.y;
                    destination.x = unitPointX;
                    destination.y = unitPointY;
                    Debug.Log("Final destination: " + destination.ToString());
                }
                else
                {
                    Debug.Log("Starting position: " + currentArmyCenter.ToString() + " -> Destination: " + destination.ToString());
                    // Get the line equation of the original vector (original center -> destination center)
                    // Slope of original vector
                    float origSlope = (destination.y - currentArmyCenter.y) / (destination.x - currentArmyCenter.x);
                    // Intercept of original vector
                    float origIntercept = currentArmyCenter.y - (origSlope * currentArmyCenter.x);
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
                    // Positive direction vector; Choose a random x (destination - 1 here) and use y = mx + b to solve for y
                    Vector2 posDirVec = new Vector2(destination.x + 1, (destSlope * (destination.x + 1)) + destIntercept);
                    posDirVec.Normalize();

                    // Positive direction vector; Choose a random x (destination - 1 here) and use y = mx + b to solve for y
                    Vector2 negDirVec = new Vector2(destination.x - 1, (destSlope * (destination.x - 1)) + destIntercept);
                    negDirVec.Normalize();

                    // Determine how many unit positions left or right this specific unit is
                    float centerUnit = armySize / 2f;
                    float unitOffset = (centerUnit - armyPosition) * UNIT_WIDTH_COLUMN;

                    // Utilize the vector equation to get this units position
                    // destination is already set to destinationCoords
                    destination += unitOffset * posDirVec;
                    if (armyPosition == armySize - 1)
                    {
                        Debug.DrawRay(new Vector3(currentArmyCenter.x, 0, currentArmyCenter.y), 
                            new Vector3(currentArmyCenter.x - destinationCoords.x, 0, currentArmyCenter.y - destinationCoords.z), 
                            Color.blue, 30); // orig line
                        Debug.DrawRay(destinationCoords, 
                            4 * posDirVec, 
                            Color.red, 30); // dest line
                        Debug.DrawRay(destinationCoords,
                            -4 * posDirVec,
                            Color.red, 30); // dest line
                    }
                }
                break;
        }
        return new Vector3(destination.x, 0, destination.y);
    }
    private int GetRowCount()
    {
        switch (formationType)
        {
            case 0:
                return Mathf.CeilToInt(armySize / (float)LINE_COLUMN_COUNT);
            default:
                Debug.Log("ERROR: GetRowCount() unhandled formationType...");
                return -1;
        }
    }
    private int GetColumnCount()
    {
        switch (formationType)
        {
            case 0:
                if (armySize < LINE_COLUMN_COUNT)
                    return armySize;
                else
                    return LINE_COLUMN_COUNT;
            default:
                Debug.Log("ERROR: GetColumnCount() unhandled formationType...");
                return -1;
        }
    }
    private int GetThisUnitsColumnCount()
    {
        int thisUnitsRow = armyPosition / LINE_COLUMN_COUNT;
        if (thisUnitsRow == GetRowCount() - 1)
        {
            return armySize - ((GetRowCount() - 1) * LINE_COLUMN_COUNT);
        }
        else
        {
            return LINE_COLUMN_COUNT;
        }
    }
}
