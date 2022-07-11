using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;
    public List<Transform> selectedUnits = new List<Transform>();
    private bool isDragging = false;
    private RaycastHit hit;
    private Vector3 mousePos;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void OnGUI()
    {
        if (isDragging)
        {
            Rect rect = Multiselect.GetScreenRect(mousePos, Input.mousePosition);
            Multiselect.DrawScreenRect(rect, new Color(0f, 0f, 0f, 0.25f));
            Multiselect.DrawScreenRectBorder(rect, 3, Color.gray);
        }
    }
    void Update()
    {
        // Check what we are clicking on with left mouse.
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("PlayerLayer")))
            {
                // Grab the script that is responsible for handling all of a units actions and prefab objects.
                UnitControls unitControls = hit.transform.GetComponent<UnitControls>();
                if (unitControls != null)
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectUnits();
                    }
                    unitControls.SelectUnit();
                    selectedUnits.Add(hit.transform);
                }
            }
            else
            {
                isDragging = true;
                DeselectUnits();
            }
        }

        // Logic for when we release the left click (click-and-drag logic)
        if (Input.GetMouseButtonUp(0))
        {
            if (GameHandler.instance.playerUnits != null)
            {
                foreach (GameObject unit in GameHandler.instance.playerUnits)
                {
                    if (IsWithinSelectionBounds(unit))
                    {
                        unit.GetComponent<UnitControls>().SelectUnit();
                        selectedUnits.Add(unit.transform);
                    }
                }
            }
            isDragging = false;
        }

        // When we right click with units selected, do something.
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            mousePos = Input.mousePosition;
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                selectedUnits[i].GetComponent<UnitControls>().DoAction();
            }
        }
    }
    private void DeselectUnits()
    {
        if (selectedUnits != null)
        {
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                selectedUnits[i].GetComponent<UnitControls>().DeselectUnit();
            }
        }
        selectedUnits.Clear();
        //GameHandler.instance.GetComponent<UIHandler>().DisableWorkerMenu();
    }
    private bool IsWithinSelectionBounds(GameObject obj)
    {
        //if were not dragging, this shouldnt be called
        if (!isDragging)
        {
            return false;
        }

        Camera cam = Camera.main;
        Bounds viewPortBounds = Multiselect.GetViewPortBounds(cam, mousePos, Input.mousePosition);

        return viewPortBounds.Contains(cam.WorldToViewportPoint(obj.transform.position));
    }
}
