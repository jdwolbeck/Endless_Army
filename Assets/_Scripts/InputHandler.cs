using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;
    public List<GameObject> selectedUnits = new List<GameObject>();
    public List<GameObject> selectedBuildings = new List<GameObject>();
    public GameObject selectedResource;
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

            if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("PlayerUnitLayer")))
            {
                // Grab the script that is responsible for handling all of a units actions and prefab objects.
                UnitControls unitControls = hit.transform.GetComponent<UnitControls>();
                if (unitControls != null)
                {
                    DeselectResource();
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectUnits();
                    }
                    if (selectedBuildings.Count > 0)
                    {
                        DeselectBuildings();
                    }
                    unitControls.SelectUnit();
                    selectedUnits.Add(hit.transform.gameObject);
                }
            }
            else if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("PlayerBuildingLayer")))
            {
                Debug.Log("Selecting player buliding object...");
                // Grab the script that is responsible for handling all of a units actions and prefab objects.
                BuildingControls buildingControls = hit.transform.parent.GetComponent<BuildingControls>();
                if (buildingControls != null)
                {
                    DeselectResource();
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectBuildings();
                    }
                    if (selectedUnits.Count > 0)
                    {
                        DeselectUnits();
                    }
                    buildingControls.SelectBuilding();
                    selectedBuildings.Add(hit.transform.parent.gameObject);
                }
            }
            else if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("ResourceLayer")))
            {
                // Grab the script that is responsible for handling all of a units actions and prefab objects.
                ResourceHandler resourceHandler = hit.transform.parent.GetComponent<ResourceHandler>();
                if (resourceHandler != null)
                {
                    DeselectResource();
                    resourceHandler.SelectResource();
                    selectedResource = hit.transform.parent.gameObject;
                }
            }
            else
            {
                isDragging = true;
                DeselectResource();
                DeselectUnits();
                DeselectBuildings();
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
                        selectedUnits.Add(unit.transform.gameObject);
                    }
                }
            }
            isDragging = false;
        }

        // When we right click with units selected, do something.
        if (Input.GetMouseButtonDown(1) && (selectedUnits.Count > 0 || selectedBuildings.Count > 0))
        {
            mousePos = Input.mousePosition;
            if (selectedUnits.Count > 0)
            {
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    selectedUnits[i].GetComponent<UnitControls>().DoAction();
                }
            }
            else // selectedBuildings.Count > 0
            {
                for (int i = 0; i < selectedBuildings.Count; i++)
                {
                    selectedBuildings[i].GetComponent<BuildingControls>().DoAction();
                }
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
    private void DeselectBuildings()
    {
        if (selectedBuildings != null)
        {
            for (int i = 0; i < selectedBuildings.Count; i++)
            {
                selectedBuildings[i].GetComponent<BuildingControls>().DeselectBuilding();
            }
        }
        selectedBuildings.Clear();
    }
    private void DeselectResource()
    {
        if (selectedResource != null)
        {
            selectedResource.GetComponent<ResourceHandler>().DeselectResource();
            selectedResource = null;
        }
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
