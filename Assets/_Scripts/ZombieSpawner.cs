using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    private Vector2 spawnBoundsBotLeft;
    private Vector2 spawnBoundsTopRight;
    private float spawnCooldown = 1f;
    private float lastSpawnTime;
    private void Start()
    {
        spawnBoundsBotLeft = Vector2.zero;
        spawnBoundsTopRight = Vector2.zero;
        GetSpawnBounds();
    }
    private void Update()
    {
        if (Time.time > lastSpawnTime)
        {
            SpawnRandomZombie();
            lastSpawnTime = Time.time + spawnCooldown;
        }
    }
    private void GetSpawnBounds()
    {
        Transform groundTrans = MapManager.instance.Ground.transform;
        spawnBoundsBotLeft = new Vector2(groundTrans.position.x - 5 * groundTrans.localScale.x, groundTrans.position.z - 5 * groundTrans.localScale.z);
        spawnBoundsTopRight = new Vector2(groundTrans.position.x + 5 * groundTrans.localScale.x, groundTrans.position.z + 5 * groundTrans.localScale.z);
    }
    private void SpawnRandomZombie()
    {
        Vector3 spawnLoc = new Vector3(Random.Range(spawnBoundsBotLeft.x, spawnBoundsTopRight.x), 0, Random.Range(spawnBoundsBotLeft.y, spawnBoundsTopRight.y));
        GameObject newZomb = Instantiate(ResourceDictionary.instance.GetPrefab("Blend_WorkerUnit"), spawnLoc, Quaternion.identity);
        newZomb.GetComponent<BasicUnit>().Team = 1;
        newZomb.layer = LayerMask.NameToLayer("EnemyUnitLayer");
    }
}
