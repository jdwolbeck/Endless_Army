using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControls : MonoBehaviour
{
    public List<GameObject> progressPrefabs = new List<GameObject>();
    private int currentPrefabState;

    public void Start()
    {
        currentPrefabState = 0;
    }
    public bool SetPrefabState(int nextPrefabState)
    {
        bool stateChanged = false;

        if (currentPrefabState != nextPrefabState &&
            nextPrefabState <= progressPrefabs.Count &&
            progressPrefabs[nextPrefabState] != null)
        {
            progressPrefabs[currentPrefabState].SetActive(false);
            progressPrefabs[nextPrefabState].SetActive(true);
            currentPrefabState = nextPrefabState;
            stateChanged = true;
        }

        return stateChanged;
    }
    public bool SetNextPrefabState()
    {
        int nextState = currentPrefabState + 1;
        return SetPrefabState(nextState);
    }
}
