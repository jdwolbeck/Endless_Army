using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionHandler : MonoBehaviour
{
    private GameObject currentBuild;
    private BasicBuilding currentBasicBuilding;
    
    private bool buildingInProgress = false;
    private bool clearLock = false;
    private ScriptableBuilding scriptableBuilding;
    private delegate void LastBuildingConstructed();
    private LastBuildingConstructed lastBuild;

    private void Update()
    {
        if (clearLock)
        { // Had to put this here to fix a race condition.
            clearLock = false;
            InputHandler.instance.lockSelectedObjects = false;
        }

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
            PlayerResourceManger.instance.playerCurrentFood >= scriptableBuilding.FoodCost &&
            PlayerResourceManger.instance.playerCurrentWood >= scriptableBuilding.WoodCost &&
            PlayerResourceManger.instance.playerCurrentStone >= scriptableBuilding.StoneCost &&
            currentBasicBuilding.canBuildHere)
        {
            Destroy(currentBuild.GetComponent<Rigidbody>()); // So the remaining Town Center does not trigger anymore.
            Debug.Log("Building placed, start construction...");
            buildingInProgress = false;
            PlayerResourceManger.instance.playerCurrentFood -= scriptableBuilding.FoodCost;
            PlayerResourceManger.instance.playerCurrentWood -= scriptableBuilding.WoodCost;
            PlayerResourceManger.instance.playerCurrentStone -= scriptableBuilding.StoneCost;
            //Now that the TC is placed, tell all workers to go build it (if applicable)
            for (int i = 0; i < InputHandler.instance.selectedUnits.Count; i++)
            {
                InputHandler.instance.selectedUnits[i].GetComponent<ConstructionHandler>().StartConstruction(currentBuild);
            }
            currentBuild = null;
            currentBasicBuilding = null;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                lastBuild();
            }
            else if (!buildingInProgress)
            {
                clearLock = true;
            }
        }
        if (buildingInProgress && Input.GetMouseButtonDown(1))
        {
            buildingInProgress = false;
            GameHandler.instance.playerBuildings.Remove(currentBuild);
            Destroy(currentBuild);
            currentBuild = null;
            currentBasicBuilding = null;
            clearLock = true;
        }
    }
    public void BuildTownCenter()
    {
        scriptableBuilding = (ScriptableBuilding)ResourceDictionary.instance.GetPreset("TownCenter");
        if (!buildingInProgress &&
            PlayerResourceManger.instance.playerCurrentFood >= scriptableBuilding.FoodCost &&
            PlayerResourceManger.instance.playerCurrentWood >= scriptableBuilding.WoodCost &&
            PlayerResourceManger.instance.playerCurrentStone >= scriptableBuilding.StoneCost)
        {
            lastBuild = new LastBuildingConstructed(BuildTownCenter);
            InputHandler.instance.lockSelectedObjects = true;
            buildingInProgress = true;
            currentBuild = Instantiate(ResourceDictionary.instance.GetPrefab("TownCenterEGO"));
            currentBuild.AddComponent<Rigidbody>().useGravity = false;
            currentBasicBuilding = currentBuild.GetComponent<BasicBuilding>();
            currentBasicBuilding.LoadFromPreset(scriptableBuilding);
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
        else
        {
            clearLock = true;
        }
    }
    public void BuildBarracks()
    {
        scriptableBuilding = (ScriptableBuilding)ResourceDictionary.instance.GetPreset("Barracks");
        if (!buildingInProgress &&
            PlayerResourceManger.instance.playerCurrentFood >= scriptableBuilding.FoodCost &&
            PlayerResourceManger.instance.playerCurrentWood >= scriptableBuilding.WoodCost &&
            PlayerResourceManger.instance.playerCurrentStone >= scriptableBuilding.StoneCost)
        {
            lastBuild = new LastBuildingConstructed(BuildBarracks);
            InputHandler.instance.lockSelectedObjects = true;
            buildingInProgress = true;
            currentBuild = Instantiate(ResourceDictionary.instance.GetPrefab("BarracksEGO"));
            currentBuild.AddComponent<Rigidbody>().useGravity = false;
            currentBasicBuilding = currentBuild.GetComponent<BasicBuilding>();
            currentBasicBuilding.LoadFromPreset(scriptableBuilding);
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
        else if (!buildingInProgress)
        {
            clearLock = true;
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
