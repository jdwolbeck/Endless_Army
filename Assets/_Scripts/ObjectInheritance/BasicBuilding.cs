using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicBuilding : BasicObject
{
    public GameObject buildingBlocks;
    public GameObject highlight;
    public GameObject rallyPoint;
    public GameObject progressBar;
    public bool canBuildHere;

    protected float buildTime;

    private List<Transform> prefabObjectList;
    private List<Material> buildingBlockMatList;
    private Slider progressSlider;
    private float currentProgress;
    private bool hasRallyPoint;
    private bool isBuilt;
    private bool blueprintMatsApplied;
    private int currentColliders;
    private int currentWorkers;

    protected override void Awake()
    {
        base.Awake();
        canBuildHere = true;
        buildTime = 10f;
        prefabObjectList = new List<Transform>();
        buildingBlockMatList = new List<Material>();
        progressSlider = progressBar.GetComponent<Slider>();
        currentProgress = 0;
        hasRallyPoint = false;
        isBuilt = false;
        blueprintMatsApplied = false;
        currentColliders = 0;
        currentWorkers = 0;
        SetBlueprintMats(true);
        SetLayerRecursively(buildingBlocks, LayerMask.NameToLayer("ConstructionLayer"));
    }
    protected virtual void Update()
    {
        // Logic related to the construction of this building
        if (currentWorkers > 0)
        {
            currentProgress += Time.deltaTime * currentWorkers;
            progressSlider.normalizedValue = currentProgress / buildTime;

            UpgradeProgressPrefab();
            if (currentProgress >= buildTime)
            {
                isBuilt = true;
                progressBar.SetActive(false);
                SetLayerRecursively(buildingBlocks, gameObject.layer);
                SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(gameObject.layer));
            }
        }
        // Change the blueprint to a red color if we are currently colliding with another object
        if (blueprintMatsApplied)
        {
            ChangeBlueprintColor();
        }
    }
    public override void DoAction()
    {
        base.DoAction();
        RaycastHit hit;
        hasRallyPoint = true;
        rallyPoint.SetActive(true);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        rallyPoint.transform.position = hit.point;
    }
    public virtual void SelectBuilding()
    {
        highlight.SetActive(true);
        if (hasRallyPoint)
        {
            rallyPoint.SetActive(true);
        }
    }
    public virtual void DeselectBuilding()
    {
        highlight.SetActive(false);
        rallyPoint.SetActive(false);
    }
    public void UpdateActiveBuilders(bool addWorker)
    {
        if (addWorker) // The building will be built only if some workers are currently working on it
            currentWorkers++;
        else
            currentWorkers--;
    }
    public bool IsBuildingBuilt()
    {
        return isBuilt;
    }
    private void UpgradeProgressPrefab()
    {
        // We started building our building, should only occur on the first call.
        if (blueprintMatsApplied)
        {
            SetBlueprintMats(false);
            progressBar.SetActive(true);
        }
        // Populate our object list with every 1st generation child of our top-level transform.
        if (prefabObjectList.Count == 0)
            foreach (Transform child in buildingBlocks.transform)
            {
                prefabObjectList.Add(child);
                child.gameObject.SetActive(false);
            }
        // Enable each object in our object list in order from the top to the bottom based on % down list
        for (int i = 0; i < prefabObjectList.Count; i++)
        {
            if (!prefabObjectList[i].gameObject.activeInHierarchy &&
                (float)i/(prefabObjectList.Count - 1) <= progressSlider.normalizedValue)
            {
                prefabObjectList[i].gameObject.SetActive(true);
            }
        }
    }
    private void SetBlueprintMats(bool setBPMats)
    {
        for(int i = 0; i < buildingBlocks.transform.childCount; i++)
        {
            // If a certain GO has a meshrenderer it means that it is a visible 3d object on a prefab.
            MeshRenderer meshR;
            NavMeshObstacle navMeshObstacle;
            if (buildingBlocks.transform.GetChild(i).TryGetComponent<MeshRenderer>(out meshR))
            {
                if (buildingBlocks.transform.GetChild(i).TryGetComponent<NavMeshObstacle>(out navMeshObstacle))
                    navMeshObstacle.enabled = !setBPMats;

                if (setBPMats)
                {
                    buildingBlockMatList.Add(meshR.material);
                    meshR.material = ResourceDictionary.instance.GetMaterial("BlueprintGoodMat");
                }
                else
                {
                    meshR.material = buildingBlockMatList[0];
                    buildingBlockMatList.RemoveAt(0);
                }
            }
        }

        blueprintMatsApplied = setBPMats;
    }
    private void ChangeBlueprintColor()
    {
        //Debug.Log(currentColliders);
        if (canBuildHere && currentColliders > 0)
        {
            canBuildHere = false;
            SetMaterialRecursively(buildingBlocks, ResourceDictionary.instance.GetMaterial("BlueprintBadMat"));
        }
        if (!canBuildHere && currentColliders == 0)
        {
            canBuildHere = true;
            SetMaterialRecursively(buildingBlocks, ResourceDictionary.instance.GetMaterial("BlueprintGoodMat"));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("GroundLayer"))
            currentColliders++;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("GroundLayer"))
            currentColliders--;
    }
}
