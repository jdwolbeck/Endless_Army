using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceDictionary : MonoBehaviour
{
    public static ResourceDictionary instance;
    public List<GameObject> Prefabs { get; private set; }
    public List<Material> Materials { get; private set; }
    private Dictionary<string, GameObject> PrefabsDict;
    private Dictionary<string, Material> MaterialsDict;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Prefabs = new List<GameObject>();
        Prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Bush").ToList());
        Prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Stone").ToList());
        Prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/TownCenter").ToList());
        Prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tree").ToList());
        Prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Units").ToList());
        Materials = Resources.LoadAll<Material>("Materials").ToList();
        PrefabsDict = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in Prefabs)
        {
            PrefabsDict.Add(prefab.name, prefab);
        }
        MaterialsDict = new Dictionary<string, Material>();
        foreach (Material mat in Materials)
        {
            MaterialsDict.Add(mat.name, mat);
        }
    }
    public GameObject GetPrefab(string prefabName)
    {
        return PrefabsDict[prefabName];
    }

    public Material GetMaterial(string matName)
    {
        return MaterialsDict[matName];
    }
}