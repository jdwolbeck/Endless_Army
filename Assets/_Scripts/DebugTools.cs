using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int spawnAmount = 2500;
        Debug.Log("Spawning " + spawnAmount + " units");
        GameObject obj;
        float rand;
        float randLocX;
        float randLocZ;
        for (int i = 0; i < spawnAmount; i++)
        {
            rand = Random.Range(0.0f, 2.0f);
            randLocX = Random.Range(-60.0f, 60.0f);
            randLocZ = Random.Range(-50.0f, 50.0f);
            if (rand < 1f)
            {
                obj = Instantiate(ResourceDictionary.instance.GetPrefab("Blend_WorkerUnit"), new Vector3(randLocX, 0, randLocZ), Quaternion.identity);
                //obj.GetComponent<BasicUnit>().LoadFromPreset((ScriptableObj)ResourceDictionary.instance.GetPreset("Worker"));
                if (rand < 0.5f || rand >= 1.5f)
                {
                    obj.layer = LayerMask.NameToLayer("EnemyUnitLayer");
                    GameHandler.instance.enemyUnits.Add(obj);
                }
                else
                {
                    obj.layer = LayerMask.NameToLayer("PlayerUnitLayer");
                    GameHandler.instance.playerUnits.Add(obj);
                }
            }
            else
            {
                obj = Instantiate(ResourceDictionary.instance.GetPrefab("Blend_FighterUnit"), new Vector3(randLocX, 0, randLocZ), Quaternion.identity);
                //obj.GetComponent<BasicUnit>().LoadFromPreset((ScriptableObj)ResourceDictionary.instance.GetPreset("Fighter"));
                if (rand >= 0.5f && rand < 1.5f)
                {
                    obj.layer = LayerMask.NameToLayer("EnemyUnitLayer");
                    GameHandler.instance.enemyUnits.Add(obj);
                }
                else
                {
                    obj.layer = LayerMask.NameToLayer("PlayerUnitLayer");
                    GameHandler.instance.playerUnits.Add(obj);
                }
            }
        }
    }
}
