using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public GameObject Ground;
    public List<MapGrid> MapList;
    private List<Button> btnMapList;
    [SerializeField] private Image UIMapGrid;
    private int tempGeneratedCount = 0;
    public Button generatedMapButton;
    private int selectedMap = -1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        MapList = new List<MapGrid>();
        btnMapList = new List<Button>();
    }
    public void GenerateMap()
    {
        MapGrid newMap = new MapGrid();
        if (!MapGeneration.instance.GenerateNewMap(newMap))
        {
            Debug.Log("Map generation failed...");
            return;
        }
        MapList.Add(newMap);
        Button btn = Instantiate(generatedMapButton, Vector3.zero, Quaternion.identity, UIMapGrid.transform);
        btn.GetComponentInChildren<TMP_Text>().text = "GeneratedMap " + tempGeneratedCount.ToString();
        btn.GetComponent<Button>().onClick.AddListener(() => SelectMap(btn.GetComponentInChildren<TMP_Text>().text));
        btnMapList.Add(btn);
        tempGeneratedCount++;
    }
    public void LoadMap(MapGrid map)
    {
        if (map == null)
        { // null passed in, lets try to see if a map was selected from the GUI.
            if (selectedMap != -1 && MapList.Count > 0)
                map = MapList[selectedMap];
            else
            {
                Debug.Log("No map selected to Load");
                return;
            }
        }
        UnloadCurrentMap();
        TeamManager.instance.teamList[0].playerCurrentFood = map.MapScriptable.StartingFoodAmount;
        TeamManager.instance.teamList[0].playerCurrentWood = map.MapScriptable.StartingWoodAmount;
        TeamManager.instance.teamList[0].playerCurrentStone = map.MapScriptable.StartingStoneAmount;
        ResourceDictionary.instance.GetMaterial("GroundMat").color = map.MapScriptable.GroundColor;
        map.InstantiateMap();
    }
    public void ClearMap()
    {
        if (selectedMap != -1 && selectedMap < MapList.Count)
        {
            MapList.RemoveAt(selectedMap);
            Button btn = btnMapList[selectedMap];
            btnMapList.Remove(btn);
            Destroy(btn.gameObject);
            tempGeneratedCount--;
            if (btnMapList.Count > 0)
            {
                for (int i = 0; i < btnMapList.Count; i++)
                {
                    btnMapList[i].GetComponentInChildren<TMP_Text>().text = "GeneratedMap " + i.ToString();
                }
            }
        }
        else
        {
            Debug.Log("Selected vs Count: " + selectedMap + " --- " + MapList.Count);
        }
    }
    void SelectMap(string btnName)
    {
        int i = 0;
        string[] tmp = btnName.Split(" ");
        foreach (string t in tmp)
        {
            int.TryParse(t, out i);
        }

        selectedMap = i;
    }
    void UnloadCurrentMap()
    {
        if (Ground != null)
        {
            Ground.transform.localScale = Vector3.one;
        }
        GameHandler.instance.RemoveAllObjects();
    }
}
