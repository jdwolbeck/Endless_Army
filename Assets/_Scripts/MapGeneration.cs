using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public static MapGeneration instance;
    private const int width = 100;
    private const int height = 100;
    private int[,] pixelArray = new int[width, height];
    private int totalHits = 0;
    private GameObject[,] primArray = new GameObject[width, height];

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        //StartCoroutine(GenerateRandomPoints());
    }
    IEnumerator GenerateRandomPoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            //string debugStr = "";
            //Debug.Log("Pixel Array:");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pixelArray[i, j] = 0;
                    if (Random.Range(0, width * height / 10) == 0)
                    {
                        SpawnPrim(i, j);
                    }
                    //debugStr += pixelArray[i,j].ToString();
                    //debugStr += " ";
                }
                //Debug.Log(debugStr);
                //debugStr = "";
            }
            BeginClusterCreep();

            float hitPercentage = (totalHits * 100f) / ((float)width * height);
            int minPercentage = 25;
            Debug.Log("Generation complete, total Hits/size: " + totalHits + " / " + width * height + " = " + hitPercentage + "%");
            if (hitPercentage >= minPercentage)
            {
                Debug.Log("Tree " + hitPercentage + "% above " + minPercentage + "%, continuing...");
                break;
            }
            Debug.Log("Tree " + hitPercentage + "% below " + minPercentage + "%, removing and regenerating...");
            yield return new WaitForSeconds(3f);
            RemoveClusters();
        }
    }
    /*public void GenerateRandomPoints()
    {
        //string debugStr = "";
        //Debug.Log("Pixel Array:");
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixelArray[i, j] = 0;
                if (Random.Range(0, width * height / 10) == 0)
                {
                    SpawnPrim(i, j);
                }
                //debugStr += pixelArray[i,j].ToString();
                //debugStr += " ";
            }
            //Debug.Log(debugStr);
            //debugStr = "";
        }
        BeginClusterCreep();

        Debug.Log("Generation complete, total Hits/size: " + totalHits + " / " + width * height + " = " + (float)(totalHits*100)/(width * height) + "%");
        RemoveClusters();
    }*/
    void BeginClusterCreep()
    {
        int[,] startingPoints = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                startingPoints[i, j] = pixelArray[i, j];
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (startingPoints[i, j] == 1)
                {
                    CreepPoint(new Vector2(i, j), 0);
                }
            }
        }
    }
    void CreepPoint(Vector2 parentPoint, int generation)
    {
        int randChance = 0;
        float dampeningFactor = generation / 4f;
        // Check Left
        if (parentPoint.x - 1 >= 0 && pixelArray[(int)parentPoint.x - 1, (int)parentPoint.y] == 0)
        {
            if (Random.Range(0f, 10f) >= randChance + dampeningFactor)
            {
                SpawnPrim((int)parentPoint.x - 1, (int)parentPoint.y);
                CreepPoint(new Vector2(parentPoint.x - 1, parentPoint.y), generation + 1);
            }
        }
        // Check Right
        if (parentPoint.x + 1 < width && pixelArray[(int)parentPoint.x + 1, (int)parentPoint.y] == 0)
        {
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                SpawnPrim((int)parentPoint.x + 1, (int)parentPoint.y);
                CreepPoint(new Vector2(parentPoint.x + 1, parentPoint.y), generation + 1);
            }
        }
        // Check Up
        if (parentPoint.y + 1 < height && pixelArray[(int)parentPoint.x, (int)parentPoint.y + 1] == 0)
        {
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                SpawnPrim((int)parentPoint.x, (int)parentPoint.y + 1);
                CreepPoint(new Vector2(parentPoint.x, parentPoint.y + 1), generation + 1);
            }
        }
        // Check Down
        if (parentPoint.y - 1 >= 0 && pixelArray[(int)parentPoint.x, (int)parentPoint.y - 1] == 0)
        {
            if (Random.Range(0, 10) >= randChance + dampeningFactor)
            {
                SpawnPrim((int)parentPoint.x, (int)parentPoint.y - 1);
                CreepPoint(new Vector2(parentPoint.x, parentPoint.y - 1), generation + 1);
            }
        }
    }
    void RemoveClusters()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (pixelArray[i, j] == 1)
                {
                    Destroy(primArray[i, j]);
                    pixelArray[i, j] = 0;
                    totalHits--;
                }
            }
        }
    }
    void SpawnPrim(int x, int y)
    {
        float compressionFactor = 10f / ((width + height) / 2f);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //GameObject go = Instantiate()
        go.transform.position = new Vector3(compressionFactor * (x + .5f - width / 2), compressionFactor * 0.5f, compressionFactor * (y + .5f - height / 2));
        go.transform.localScale = new Vector3(compressionFactor, compressionFactor, compressionFactor);
        go.GetComponent<MeshRenderer>().material.color = Color.black;

        // Increment pixelArray and keep track of how many we have
        pixelArray[x, y] = 1;
        totalHits++;
        primArray[x, y] = go;
    }
}
