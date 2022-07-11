using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControls : MonoBehaviour
{
    public GameObject highlight;
    public GameObject uiHandler;
    public void SelectUnit()
    {
        highlight.SetActive(true);
    }
    public void DeselectUnit()
    {
        highlight.SetActive(false);
    }
    public void DoAction(Vector3 mousePosition)
    {
        Debug.Log(transform.ToString() + ": Doing action... *Right click pressed @ " + mousePosition.ToString() + "*");
    }
}
