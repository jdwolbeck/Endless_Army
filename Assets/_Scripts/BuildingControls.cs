using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingControls : MonoBehaviour
{
    public List<GameObject> progressPrefabs = new List<GameObject>();
    public GameObject blueprint;
    public GameObject progressBar;
    public GameObject highlight;
    public GameObject rallyPoint;
    public bool hasRallyPoint;
    public const int woodCost = 100;
    public int currentColliders;
    private Slider progressSlider;
    private float currentProgress;
    private int currentPrefabState;

    public void Start()
    {
        currentProgress = 0;
        currentPrefabState = 0;
        progressSlider = progressBar.GetComponent<Slider>();
        currentColliders = 0;
        hasRallyPoint = false;
    }
    public void SelectBuilding()
    {
        highlight.SetActive(true);
        GetComponent<ProductionHandler>().BuildingSelectionEvent(true);
        if (hasRallyPoint)
        {
            rallyPoint.SetActive(true);
        }
    }
    public void DeselectBuilding()
    {
        highlight.SetActive(false);
        GetComponent<ProductionHandler>().BuildingSelectionEvent(false);
        rallyPoint.SetActive(false);
    }
    public void DoAction()
    {
        RaycastHit hit;
        Debug.Log("Building: " + gameObject.ToString() + " Doing action...");
        hasRallyPoint = true;
        rallyPoint.SetActive(true);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        rallyPoint.transform.position = hit.point;
    }
    public bool SetPrefabState(int nextPrefabState)
    {
        bool stateChanged = false;

        Debug.Log("SetPrefabState: current / next " + currentPrefabState + " / " + nextPrefabState);
        if (currentPrefabState != nextPrefabState &&
            nextPrefabState <= progressPrefabs.Count &&
            progressPrefabs[nextPrefabState] != null)
        {
            progressPrefabs[currentPrefabState].SetActive(false);
            progressPrefabs[nextPrefabState].SetActive(true);
            if (nextPrefabState > 0 && nextPrefabState < 3) // Progress stages
            {
                progressBar.SetActive(true);
            }
            else
            { // Blueprint or Final stage
                progressBar.SetActive(false);
            }
            currentPrefabState = nextPrefabState;
            stateChanged = true;

            // When we are on the final stage, set the layer properly
            if (currentPrefabState == 3)
            {
                progressPrefabs[currentPrefabState].layer = progressPrefabs[currentPrefabState].transform.parent.gameObject.layer;
            }
        }

        return stateChanged;
    }
    public bool SetNextPrefabState()
    {
        int nextState = currentPrefabState + 1;
        return SetPrefabState(nextState);
    }
    public void IncreaseProgressBar(float progress)
    {
        SetProgressBar(currentProgress + progress);
    }
    public void ResetProgressBar()
    {
        SetProgressBar(0);
    }
    private void SetProgressBar(float progress)
    {
        if (currentProgress != progress)
        {
            progressSlider.normalizedValue = progress; // Set the slider to a percentage of the max.
            currentProgress = progress;
            if (currentPrefabState != 1 && (progress >= 0 && progress < 0.5f))
            {
                SetPrefabState(1);
            }
            else if (currentPrefabState != 2 && (progress >= 0.5f && progress < 1.0f))
            {
                SetPrefabState(2);
            }
            else if (currentPrefabState != 3 && progress >= 1.0f)
            {
                Debug.Log("Building constructed");
                SetPrefabState(3);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        currentColliders++;
    }
    public void OnTriggerExit(Collider other)
    {
        currentColliders--;
    }
}
