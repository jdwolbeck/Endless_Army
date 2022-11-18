using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicBuilding : BasicObject
{
    public GameObject buildingBlocks;
    public GameObject rallyPoint;
    public GameObject progressBar;
    public bool canBuildHere;
    public bool hasRallyPoint;

    private List<Transform> prefabObjectList;
    private List<Material> buildingBlockMatList;
    private Slider progressSlider;
    private float currentProgress;
    private bool isBuilt;
    private bool blueprintMatsApplied;
    private int currentColliders;
    private int currentWorkers;

    protected override void Awake()
    {
        base.Awake();
        canBuildHere = true;
        prefabObjectList = new List<Transform>();
        buildingBlockMatList = new List<Material>();
        progressSlider = progressBar.GetComponent<Slider>();
    }
    protected override void Start()
    {
        base.Start();
        if (isSpawnedFromInspector)
        {
            FinishBuilding();
        }
        else if (!isBuilt)
        {
            SaveBuildingBlocksMats();
            SetMaterialRecursively(buildingBlocks, ResourceDictionary.instance.GetMaterial("BlueprintGoodMat"));
            blueprintMatsApplied = true;
            SetLayerRecursively(buildingBlocks, LayerMask.NameToLayer("ConstructionLayer"));
        }
    }
    protected virtual void Update()
    {
        if (!isAddedToObjectList && Team != invalidTeamId)
        {
            TeamManager.instance.teamList[Team].buildingList.Add(gameObject);
            isAddedToObjectList = true;
        }
        // Logic related to the construction of this building
        if (currentWorkers > 0)
        {
            currentProgress += Time.deltaTime * currentWorkers;
            progressSlider.normalizedValue = currentProgress / ProductionTime;

            UpgradeProgressPrefab();
            if (currentProgress >= ProductionTime)
            {
                isBuilt = true;
                progressBar.SetActive(false);
                SetLayerRecursively(buildingBlocks, gameObject.layer);
                //SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(gameObject.layer));
                SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(Team));
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
    public override void SelectObject()
    {
        base.SelectObject();
        if (hasRallyPoint)
        {
            rallyPoint.SetActive(true);
        }
    }
    public override void DeselectObject()
    {
        base.DeselectObject();
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
    public void FinishBuilding()
    {
        if (gameObject.layer == 0)
        {
            Debug.Log("Finish building called on a building that has no layer set.");
        }

        progressSlider.normalizedValue = 1f;
        UpgradeProgressPrefab();
        isBuilt = true;
        progressBar.SetActive(false);
        SetLayerRecursively(buildingBlocks, gameObject.layer);
        //SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(gameObject.layer));
        SetMaterialRecursively(TeamIndicators, TeamManager.instance.AssignTeamMaterial(Team));
    }
    private void UpgradeProgressPrefab()
    {
        // We started building our building, should only occur on the first call.
        if (blueprintMatsApplied)
        {
            ApplyBuildingBlocksMats();
            blueprintMatsApplied = false;
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
    private void SaveBuildingBlocksMats()
    {
        for (int i = 0; i < buildingBlocks.transform.childCount; i++)
        {
            // If a certain GO has a meshrenderer it means that it is a visible 3d object on a prefab.
            MeshRenderer meshR;
            NavMeshObstacle navMeshObstacle;
            if (buildingBlocks.transform.GetChild(i).TryGetComponent<MeshRenderer>(out meshR))
            {
                if (buildingBlocks.transform.GetChild(i).TryGetComponent<NavMeshObstacle>(out navMeshObstacle))
                    navMeshObstacle.enabled = false;

                buildingBlockMatList.Add(meshR.material);
            }
        }
    }
    private void ApplyBuildingBlocksMats()
    {
        for (int i = 0; i < buildingBlocks.transform.childCount; i++)
        {
            // If a certain GO has a meshrenderer it means that it is a visible 3d object on a prefab.
            MeshRenderer meshR;
            NavMeshObstacle navMeshObstacle;
            if (buildingBlocks.transform.GetChild(i).TryGetComponent<MeshRenderer>(out meshR))
            {
                if (buildingBlocks.transform.GetChild(i).TryGetComponent<NavMeshObstacle>(out navMeshObstacle))
                    navMeshObstacle.enabled = true;

                meshR.material = buildingBlockMatList[0];
                buildingBlockMatList.RemoveAt(0);
            }
        }
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
