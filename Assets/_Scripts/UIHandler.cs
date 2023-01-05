using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;
    public GameObject canvas;
    public GameObject workerMenu = null;
    public GameObject townCenterMenu = null;
    public GameObject barracksMenu = null;
    public GameObject createWorkerPB = null;
    public GameObject createFighterPB = null;
    private Slider createWorkerSlider;
    private Slider createFighterSlider;
    public GameObject hudMenu = null;
    public GameObject foodTextBox;
    public GameObject woodTextBox;
    public GameObject stoneTextBox;
    public GameObject focusFoodBtn;
    public GameObject focusWoodBtn;
    public GameObject focusStoneBtn;
    private TMP_Text foodText;
    private TMP_Text woodText;
    private TMP_Text stoneText;
    public GameObject MapGenerationMenu;
    public GameObject MapTypeDropdown;
    private bool isDebugMenuSet;
    private bool workerMenuActive;
    private bool debugBool;
    private void OnEnable()
    {
        InputHandler.SelectedUnitsChanged += SetWorkerMenu;
        InputHandler.SelectedBuildingsChanged += SetTownCenterMenu;
        InputHandler.SelectedBuildingsChanged += SetBarracksMenu;
    }
    private void OnDisable()
    {
        InputHandler.SelectedUnitsChanged -= SetWorkerMenu;
        InputHandler.SelectedBuildingsChanged -= SetTownCenterMenu;
        InputHandler.SelectedBuildingsChanged -= SetBarracksMenu;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        createWorkerSlider = createWorkerPB.GetComponent<Slider>();
        createFighterSlider = createFighterPB.GetComponent<Slider>();
        foodText = foodTextBox.GetComponent<TMP_Text>();
        woodText = woodTextBox.GetComponent<TMP_Text>();
        stoneText = stoneTextBox.GetComponent<TMP_Text>();
        canvas.SetActive(true);
    }
    private void Update()
    {
        if (TeamManager.instance.teamList.Count > 0 && TeamManager.instance.teamList[0].teamCurrentFood != int.Parse(foodText.text))
        {
            foodText.text = TeamManager.instance.teamList[0].teamCurrentFood.ToString();
        }
        if (TeamManager.instance.teamList.Count > 0 && TeamManager.instance.teamList[0].teamCurrentWood != int.Parse(woodText.text))
        {
            woodText.text = TeamManager.instance.teamList[0].teamCurrentWood.ToString();
        }
        if (TeamManager.instance.teamList.Count > 0 && TeamManager.instance.teamList[0].teamCurrentStone != int.Parse(stoneText.text))
        {
            stoneText.text = TeamManager.instance.teamList[0].teamCurrentStone.ToString();
        }
        if (workerMenuActive)
        {
            SetPrioritizationHighlight(DeterminePriorityHighlight());
        }
    }
    public void EnableCreateWorkerPB()
    {
        createWorkerPB.SetActive(true);
    }
    public void DisableCreateWorkerPB()
    {
        createWorkerPB.SetActive(false);
    }
    public void EnableCreateFighterPB()
    {
        createFighterPB.SetActive(true);
    }
    public void DisableCreateFighterPB()
    {
        createFighterPB.SetActive(false);
    }
    public void OnClickWorkerMenuBuildTownCenterButton()
    {
        // Tell the first worker to instantiate and setup TC for build.
        InputHandler.instance.selectedUnits[0].GetComponent<ConstructionHandler>().BuildTownCenter();
    }
    public void OnClickWorkerMenuBuildBarracksButton()
    {
        // Tell the first worker to instantiate and setup TC for build.
        InputHandler.instance.selectedUnits[0].GetComponent<ConstructionHandler>().BuildBarracks();
    }
    public void OnClickTownCenterMenuBuildWorkerButton()
    {
        for (int i = 0; i < InputHandler.instance.selectedBuildings.Count; i++)
        {
            InputHandler.instance.selectedBuildings[i].GetComponent<TownCenterBuilding>().AddWorkerToQueue();
        }
    }
    public void OnClickBarracksMenuBuildFighterButton()
    {
        for (int i = 0; i < InputHandler.instance.selectedBuildings.Count; i++)
        {
            InputHandler.instance.selectedBuildings[i].GetComponent<BarracksBuilding>().AddFighterToQueue();
        }
    }
    public void OnClickMapGenerationMenuGenerateMapButton()
    {
        if (MapTypeDropdown.GetComponent<TMP_Dropdown>().value == 1) // RTS Map type
            MapManager.instance.GenerateMap();
        else if (MapTypeDropdown.GetComponent<TMP_Dropdown>().value == 2) // Zombor Map type
            MapManager.instance.GenerateZomborMap();
    }
    public void OnClickMapGenerationMenuLoadMapButton()
    {
        MapManager.instance.LoadMap(null);
    }
    public void OnClickMapGenerationMenuClearMapButton()
    {
        MapManager.instance.ClearMap();
    }
    public void OnClickDebugMenuShowHideButton()
    {
        if (isDebugMenuSet)
        {
            MapGenerationMenu.SetActive(false);
            isDebugMenuSet = false;
        }
        else
        {
            MapGenerationMenu.SetActive(true);
            isDebugMenuSet = true;
        }
    }
    public void OnClickDebugMenuDoDebugTaskButton()
    {
        debugBool = !debugBool;
        // This function is used for any variety of debug tasks.
        GameObject playerTC = GameHandler.instance.playerBuildings[0];
        foreach (GameObject unit in TeamManager.instance.teamList[1].unitList)
        {
            if (unit.TryGetComponent(out BasicUnit basicUnit))
            {
                if (debugBool)
                    basicUnit.SetAttackTarget(playerTC.GetComponent<BasicObject>(), false);
                else
                {
                    basicUnit.ClearCurrentTarget();
                    basicUnit.SetMoveLocation(transform.position - new Vector3(0, 0, -5));
                }
            }
        }
    }
    public void SetWorkerProductionBar(float progress)
    {
        createWorkerSlider.normalizedValue = progress;
    }
    public void SetFighterProductionBar(float progress)
    {
        createFighterSlider.normalizedValue = progress;
    }
    public void SetWorkerMenu()
    {
        bool isActive = false;
        if (InputHandler.instance.selectedUnits.Count != 0)
        {
            isActive = true;
            foreach (BasicUnit unit in InputHandler.instance.selectedUnits)
            {
                if (unit is not WorkerUnit)
                {
                    isActive = false;
                    break;
                }
            }
        }
        workerMenu.SetActive(isActive);
        workerMenuActive = isActive;
    }
    public void SetTownCenterMenu()
    {
        bool isActive = false;
        if (InputHandler.instance.selectedBuildings.Count != 0)
        {
            isActive = true;
            foreach (BasicBuilding building in InputHandler.instance.selectedBuildings)
            {
                if (building is not TownCenterBuilding)
                {
                    isActive = false;
                    break;
                }
            }
        }
        townCenterMenu.SetActive(isActive);
    }
    public void SetBarracksMenu()
    {
        bool isActive = false;
        if (InputHandler.instance.selectedBuildings.Count != 0)
        {
            isActive = true;
            foreach (BasicBuilding building in InputHandler.instance.selectedBuildings)
            {
                if (building is not BarracksBuilding)
                {
                    isActive = false;
                    break;
                }
            }
        }
        barracksMenu.SetActive(isActive);
    }
    public void SetWorkerPrioritization(string resourceType)
    {
        string curPriority = "";
        if (workerMenuActive)
        {
            string discrepancyFound = DeterminePriorityHighlight();
            foreach (WorkerUnit worker in InputHandler.instance.selectedUnits)
            {
                if (discrepancyFound != "" || worker.GetPrioritization() != resourceType)
                    worker.SetPrioritization(resourceType);
            }
            curPriority = ((WorkerUnit)InputHandler.instance.selectedUnits[0]).GetPrioritization();
        }
        SetPrioritizationHighlight(curPriority);
    }
    private void SetPrioritizationHighlight(string resourceType)
    {
        switch (resourceType)
        {
            case "Food":
                focusFoodBtn.GetComponent<Image>().enabled = true;
                focusWoodBtn.GetComponent<Image>().enabled = false;
                focusStoneBtn.GetComponent<Image>().enabled = false;
                break;
            case "Wood":
                focusFoodBtn.GetComponent<Image>().enabled = false;
                focusWoodBtn.GetComponent<Image>().enabled = true;
                focusStoneBtn.GetComponent<Image>().enabled = false;
                break;
            case "Stone":
                focusFoodBtn.GetComponent<Image>().enabled = false;
                focusWoodBtn.GetComponent<Image>().enabled = false;
                focusStoneBtn.GetComponent<Image>().enabled = true;
                break;
            default:
                focusFoodBtn.GetComponent<Image>().enabled = false;
                focusWoodBtn.GetComponent<Image>().enabled = false;
                focusStoneBtn.GetComponent<Image>().enabled = false;
                break;
        }
    }
    private string DeterminePriorityHighlight()
    {
        string curPriority = "";
        List<string> priorityList = new List<string>();
        foreach (WorkerUnit worker in InputHandler.instance.selectedUnits)
        {
            priorityList.Add(worker.GetPrioritization());
        }
        if (priorityList.Count > 0)
        {
            curPriority = priorityList[0];
            for (int i = 0; i < priorityList.Count; i++)
            {
                for (int j = i + 1; j < priorityList.Count; j++)
                {
                    if (priorityList[i] != priorityList[j])
                    {
                        curPriority = "";
                        break;
                    }
                }
            }
        }
        return curPriority;
    }
}
