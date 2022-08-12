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
    public GameObject createWorkerPB = null;
    private Slider createWorkerSlider;
    public GameObject hudMenu = null;
    public GameObject foodTextBox;
    public GameObject woodTextBox;
    public GameObject stoneTextBox;
    private TMP_Text foodText;
    private TMP_Text woodText;
    private TMP_Text stoneText;
    private void OnEnable()
    {
        InputHandler.SelectedUnitsChanged += SetWorkerMenu;
    }
    private void OnDisable()
    {
        InputHandler.SelectedUnitsChanged -= SetWorkerMenu;
    }
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        createWorkerSlider = createWorkerPB.GetComponent<Slider>();
        foodText = foodTextBox.GetComponent<TMP_Text>();
        woodText = woodTextBox.GetComponent<TMP_Text>();
        stoneText = stoneTextBox.GetComponent<TMP_Text>();
        canvas.SetActive(true);
    }
    private void Update()
    {
        if (InputHandler.instance.selectedBuildings.Count == 1)
        {   // Only allow the build menu to be open when there is a single worker selected.
            townCenterMenu.SetActive(true);
        }
        else
        {
            townCenterMenu.SetActive(false);
        }

        if (PlayerResourceManger.instance.playerCurrentFood != int.Parse(foodText.text))
        {
            foodText.text = PlayerResourceManger.instance.playerCurrentFood.ToString();
        }
        if (PlayerResourceManger.instance.playerCurrentWood != int.Parse(woodText.text))
        {
            woodText.text = PlayerResourceManger.instance.playerCurrentWood.ToString();
        }
        if (PlayerResourceManger.instance.playerCurrentStone != int.Parse(stoneText.text))
        {
            stoneText.text = PlayerResourceManger.instance.playerCurrentStone.ToString();
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
    public void OnClickWorkerMenuBuildTownCenterButton()
    {
        // Tell the first worker to instantiate and setup TC for build.
        InputHandler.instance.selectedUnits[0].GetComponent<ConstructionHandler>().BuildTownCenter();
        /*for (int i = 0; i < InputHandler.instance.selectedUnits.Count; i++)
        {
            InputHandler.instance.selectedUnits[i].GetComponent<ConstructionHandler>().BuildTownCenter();
        }*/
    }
    public void OnClickTownCenterMenuBuildWorkerButton()
    {
        for (int i = 0; i < InputHandler.instance.selectedBuildings.Count; i++)
        {
            InputHandler.instance.selectedBuildings[i].GetComponent<ProductionHandler>().AddWorkerToQueue();
        }
    }
    public void OnClickMapGenerationMenuGenerateMapButton()
    {
        MapManager.instance.GenerateMap();
    }
    public void OnClickMapGenerationMenuLoadMapButton()
    {
        MapManager.instance.LoadMap(null);
    }
    public void OnClickMapGenerationMenuClearMapButton()
    {
        MapManager.instance.ClearMap();
    }
    public void SetWorkerProductionBar(float progress)
    {
        createWorkerSlider.normalizedValue = progress;
    }
    public void SetWorkerMenu()
    {
        bool isActive = false;
        if (InputHandler.instance.selectedUnits.Count != 0)
        {
            isActive = true;
            foreach (BasicUnit unit in InputHandler.instance.selectedUnits)
            {
                if (unit is FighterUnit)
                {
                    isActive = false;
                    break;
                }
            }
        }
        workerMenu.SetActive(isActive);
    }
}
