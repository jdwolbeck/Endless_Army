using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionHandler : MonoBehaviour
{
    private GameObject currentBuild;
    private bool buildingInProgress = false;
    private GameObject townCenterPrefab;

    private void Start()
    {
        buildingInProgress = false;
        Debug.Log("new worker, buliding in progress = false");
        currentBuild = null;
        townCenterPrefab = Resources.Load("Prefabs/TownCenter/TownCenterAIO") as GameObject;
    }
    void Update()
    {
        if (buildingInProgress)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("GroundLayer")))
            {
                Vector3 newPos = hit.point;
                newPos.y = currentBuild.transform.position.y; // Keep the y component of the GameObject to not make it phase through ground
                currentBuild.transform.position = newPos;
            } 
            else
            {
                Debug.Log("Ray did not hit anything.");
            }
        }

        if (buildingInProgress && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            //GameObject prefab = Resources.Load("Prefabs/TownCenter/TownCenterProg0") as GameObject;
            //GameObject go = Instantiate(prefab);

            //Vector3 pos = currentBuild.transform.position;
            //pos.y = go.transform.position.y; // Keep the y component of the GameObject
            //go.transform.position = pos;

            currentBuild.GetComponent<BuildingControls>().ResetProgressBar();
            currentBuild.GetComponent<BuildingControls>().SetNextPrefabState();
            Debug.Log("Building placed, start construction...");
            buildingInProgress = false;
            //Destroy(currentBuild);
            InputHandler.instance.selectedUnits[0].GetComponent<WorkerScript>().StopAction();
            PlayerResourceManger.instance.playerCurrentWood -= BuildingControls.woodCost;
            InputHandler.instance.selectedUnits[0].GetComponent<WorkerScript>().ConstructBuild(currentBuild);
            currentBuild = null;
            //InputHandler.instance.selectedUnits[0].GetComponent<WorkerScript>().ConstructBuild(go);
        }
        if (buildingInProgress && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Buliding cancelled (Right-Click)");
            buildingInProgress = false;
            Destroy(currentBuild);
            currentBuild = null;
        }
    }
    public void BuildTownCenter()
    {
        if (!buildingInProgress && PlayerResourceManger.instance.playerCurrentWood >= BuildingControls.woodCost)
        {
            buildingInProgress = true;
            currentBuild = Instantiate(townCenterPrefab);
            if (gameObject.layer == LayerMask.NameToLayer("PlayerUnitLayer"))
            {
                GameHandler.instance.playerBuildings.Add(currentBuild);
                currentBuild.layer = LayerMask.NameToLayer("PlayerBuildingLayer");
            }
            else
            {
                GameHandler.instance.enemyBuildings.Add(currentBuild);
                currentBuild.layer = LayerMask.NameToLayer("EnemyBuildingLayer");
            }
            if (currentBuild == null)
            {
                Debug.Log("Current build was null, cancel building (Instantiate failed?)");
                buildingInProgress = false;
            }
        }
    }
}
