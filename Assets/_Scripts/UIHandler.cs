using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject workerMenu = null;
    public GameObject townCenterMenu = null;
    public GameObject createWorkerPB = null;
    private Slider createWorkerSlider;

    private void Start()
    {
        createWorkerSlider = createWorkerPB.GetComponent<Slider>();
    }
    private void Update()
    {
        if (InputHandler.instance.selectedUnits.Count == 1)
        {   // Only allow the build menu to be open when there is a single worker selected.
            workerMenu.SetActive(true);
        }
        else
        {
            workerMenu.SetActive(false);
        }
        if (InputHandler.instance.selectedBuildings.Count == 1)
        {   // Only allow the build menu to be open when there is a single worker selected.
            townCenterMenu.SetActive(true);
        }
        else
        {
            townCenterMenu.SetActive(false);
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
    public void OnClickBuildTownCenterButton()
    {
        for (int i = 0; i < InputHandler.instance.selectedUnits.Count; i++)
        {
            InputHandler.instance.selectedUnits[i].GetComponent<ConstructionHandler>().BuildTownCenter();
        }
    }
    public void OnClickBuildWorkerButton()
    {
        for (int i = 0; i < InputHandler.instance.selectedBuildings.Count; i++)
        {
            InputHandler.instance.selectedBuildings[i].GetComponent<ProductionHandler>().AddWorkerToQueue();
        }
    }
    public void SetWorkerProductionBar(float progress)
    {
        createWorkerSlider.normalizedValue = progress;
    }
}
