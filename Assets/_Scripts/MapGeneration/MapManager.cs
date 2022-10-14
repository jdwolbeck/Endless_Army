using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public GameObject Ground;
    public List<MapGrid> MapList;
    public bool UpdateNavMesh;
    private NavMeshSurface navSurface;
    private List<Button> btnMapList;
    [SerializeField] private Image UIMapGrid;
    private int tempGeneratedCount = 0;
    public Button generatedMapButton;
    private int selectedMap = -1;
    //private bool navMeshBuilt;
    //private float waitUntil = 0.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        MapList = new List<MapGrid>();
        btnMapList = new List<Button>();
        //navSurface = Ground.GetComponent<NavMeshSurface>();
        //navSurface.minRegionArea = 5f;
    }
 /*   private void Update()
    {
        if (UpdateNavMesh && !navMeshBuilt)
        {
            navSurface.BuildNavMesh();
            navMeshBuilt = true;
        }
        if (UpdateNavMesh && Time.time > waitUntil)
        {
            navSurface.UpdateNavMesh(navSurface.navMeshData);
            waitUntil = Time.time + 180f;
        }
    }*/
    public void GenerateMap()
    {
        MapGrid newMap = new MapGrid();
        if (!MapGeneration.instance.GenerateNewMap(newMap))
        {
            //Debug.Log("Map generation failed...");
            return;
        }
        newMap.MapType = MapTypeEnum.Rts;
        MapList.Add(newMap);
        Button btn = Instantiate(generatedMapButton, Vector3.zero, Quaternion.identity, UIMapGrid.transform);
        btn.GetComponentInChildren<TMP_Text>().text = "RTS Map " + tempGeneratedCount.ToString();
        btn.GetComponent<Button>().onClick.AddListener(() => SelectMap(btn.GetComponentInChildren<TMP_Text>().text));
        btnMapList.Add(btn);
        tempGeneratedCount++;
    }
    public void GenerateZomborMap()
    {
        MapGrid newMap = new MapGrid();
        if (!MapGeneration.instance.GenerateNewMap(newMap))
        {
            //Debug.Log("Map generation failed...");
            return;
        }
        newMap.MapType = MapTypeEnum.Zombor;
        MapList.Add(newMap);
        Button btn = Instantiate(generatedMapButton, Vector3.zero, Quaternion.identity, UIMapGrid.transform);
        btn.GetComponentInChildren<TMP_Text>().text = "Zombor Map " + tempGeneratedCount.ToString();
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
        TeamManager.instance.InitializeTeams(map);
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
