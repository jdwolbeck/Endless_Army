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
    public void DoAction()
    {
        Vector3 mousePosition = Input.mousePosition;
        Debug.Log(transform.ToString() + ": Doing action... *Right click pressed @ " + mousePosition.ToString() + "*");
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            // If we were in the middle of building, stop
            GetComponent<WorkerScript>().StopBuilding();
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("GroundLayer"))
            {
                navAgent.SetDestination(hit.point);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ConstructionLayer"))
            {
                GetComponent<WorkerScript>().ConstructBuild(hit.transform.parent.gameObject);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ResourceLayer"))
            {

            }
        }
        Debug.Log("hit: " + hit.ToString() + " layer " + hit.transform.gameObject.layer + " transform/position " + hit.transform + "/" + hit.transform.position + " point " + hit.point);
    }
}
