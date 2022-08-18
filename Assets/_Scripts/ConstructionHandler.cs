using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionHandler : MonoBehaviour
{
    private GameObject currentBuild;
    private BuildingControls currentBuildControls;

    private BasicBuilding currentBasicBuilding;
    
    private bool buildingInProgress = false;
    private GameObject townCenterPrefab;
    private Material tcBlueprintGoodMat;
    private Material tcBlueprintBadMat;

    private void Start()
    {
        buildingInProgress = false;
        currentBuild = null;
        currentBuildControls = null;
        townCenterPrefab = ResourceDictionary.instance.GetPrefab("TownCenterAIO");
        tcBlueprintGoodMat = ResourceDictionary.instance.GetMaterial("BlueprintGoodMat");
        tcBlueprintBadMat = ResourceDictionary.instance.GetMaterial("BlueprintBadMat");
    }
    private void Update()
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

        if (buildingInProgress && Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject() &&
            currentBasicBuilding.canBuildHere)
        {
            InputHandler.instance.lockSelectedObjects = false;
            Destroy(currentBuild.GetComponent<Rigidbody>()); // So the remaining Town Center does not trigger anymore.
            Debug.Log("Building placed, start construction...");
            buildingInProgress = false;
            PlayerResourceManger.instance.playerCurrentWood -= BuildingControls.woodCost;
            //Now that the TC is placed, tell all workers to go build it (if applicable)
            for (int i = 0; i < InputHandler.instance.selectedUnits.Count; i++)
            {
                InputHandler.instance.selectedUnits[i].GetComponent<ConstructionHandler>().StartConstruction(currentBuild);
            }
            currentBuild = null;
            currentBuildControls = null;
        }
        if (buildingInProgress && Input.GetMouseButtonDown(1))
        {
            InputHandler.instance.lockSelectedObjects = false;
            Debug.Log("Building cancelled (Right-Click)");
            buildingInProgress = false;
            GameHandler.instance.playerBuildings.Remove(currentBuild);
            Destroy(currentBuild);
            currentBuild = null;
            currentBuildControls = null;
        }
    }
    public void BuildTownCenter()
    {
        if (!buildingInProgress && PlayerResourceManger.instance.playerCurrentWood >= BuildingControls.woodCost)
        {
            InputHandler.instance.lockSelectedObjects = true;
            buildingInProgress = true;
            currentBuild = Instantiate(townCenterPrefab);
            currentBuild.AddComponent<Rigidbody>().useGravity = false;
            currentBuildControls = currentBuild.GetComponent<BuildingControls>();
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
    public void BuildBarracks()
    {
        if (!buildingInProgress && PlayerResourceManger.instance.playerCurrentWood >= BuildingControls.woodCost)
        {
            InputHandler.instance.lockSelectedObjects = true;
            buildingInProgress = true;
            currentBuild = Instantiate(ResourceDictionary.instance.GetPrefab("BarracksEGO"));
            currentBuild.AddComponent<Rigidbody>().useGravity = false;
            currentBasicBuilding = currentBuild.GetComponent<BasicBuilding>();
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
    public void StartConstruction(GameObject build)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            GetComponent<WorkerUnit>().StopAction();
            GetComponent<WorkerUnit>().ConstructBuild(build);
        }
        else if (GetComponent<WorkerUnit>().currentBuild == null)
        {
            GetComponent<WorkerUnit>().ConstructBuild(build);
        }
    }
}
