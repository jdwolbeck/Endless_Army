using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitControls : MonoBehaviour
{
    public GameObject highlight;
    private NavMeshAgent navAgent;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }
    public void SelectUnit()
    {
        highlight.SetActive(true);
    }
    public void DeselectUnit()
    {
        highlight.SetActive(false);
    }
}
