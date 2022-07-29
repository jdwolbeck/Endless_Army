using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionHandler : MonoBehaviour
{
    private GameObject currentBuild;
    private BuildingControls currentBuildControls;
    private bool buildingInProgress = false;
    private GameObject townCenterPrefab;
    private Material tcBlueprintGoodMat;
    private Material tcBlueprintBadMat;

    private void Start()
    {
        buildingInProgress = false;
        currentBuild = null;
        currentBuildControls = null;
        townCenterPrefab = Resources.Load("Prefabs/TownCenter/TownCenterAIO") as GameObject;
        tcBlueprintGoodMat = Resources.Load("Materials/BlueprintGoodMat") as Material;
        tcBlueprintBadMat = Resources.Load("Materials/BlueprintBadMat") as Material;
    }
    void Update()
    {
        if (buildingInProgress)
        {
            if (currentBuildControls.currentColliders > 0)
            {
                currentBuildControls.blueprint.GetComponent<MeshRenderer>().material = tcBlueprintBadMat;
            }
            else
            {
                currentBuildControls.blueprint.GetComponent<MeshRenderer>().material = tcBlueprintGoodMat;
            }
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
            currentBuildControls.currentColliders == 0)
        {
            //GameObject prefab = Resources.Load("Prefabs/TownCenter/TownCenterProg0") as GameObject;
            //GameObject go = Instantiate(prefab);

            //Vector3 pos = currentBuild.transform.position;
            //pos.y = go.transform.position.y; // Keep the y component of the GameObject
            //go.transform.position = pos;

            currentBuildControls.ResetProgressBar();
            currentBuildControls.SetNextPrefabState();
            Destroy(currentBuild.GetComponent<Rigidbody>()); // So the remaining Town Center does not trigger anymore.
            //currentBuild.GetComponent<BuildingControls>().blueprint.GetComponent<BoxCollider>().isTrigger = false; // This is to make the blueprint not trigger any more after a building is fully built
            Debug.Log("Building placed, start construction...");
            buildingInProgress = false;
            //Destroy(currentBuild);
            //InputHandler.instance.selectedUnits[0].GetComponent<WorkerUnit>().StopAction();
            GetComponent<WorkerUnit>().StopAction();
            PlayerResourceManger.instance.playerCurrentWood -= BuildingControls.woodCost;
            //InputHandler.instance.selectedUnits[0].GetComponent<WorkerUnit>().ConstructBuild(currentBuild);
            GetComponent<WorkerUnit>().ConstructBuild(currentBuild);
            currentBuild = null;
            currentBuildControls = null;
            //InputHandler.instance.selectedUnits[0].GetComponent<WorkerScript>().ConstructBuild(go);
        }
        if (buildingInProgress && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Buliding cancelled (Right-Click)");
            buildingInProgress = false;
            Destroy(currentBuild);
            currentBuild = null;
            currentBuildControls = null;
        }
    }
    public void BuildTownCenter()
    {
        if (!buildingInProgress && PlayerResourceManger.instance.playerCurrentWood >= BuildingControls.woodCost)
        {
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
}
