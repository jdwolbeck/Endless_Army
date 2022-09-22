using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObject : MonoBehaviour
{
    public TeamEnum Team;
    public GameObject highlight;
    public GameObject TeamIndicators;
    public int FoodCost;
    public int WoodCost;
    public int StoneCost;
    public float ProductionTime;
    public float MaxHealth;
    protected float currentHealth;

    public delegate void ObjectDeathEvent(GameObject go);
    public event ObjectDeathEvent ObjectDied;

    protected virtual void Awake() { }
    protected virtual void Start() 
    {
        Team = TeamManager.instance.AssignTeam(gameObject.layer);
    }
    public virtual void DoAction() { }
    public virtual void SelectObject()
    {
        highlight.SetActive(true);
    }
    public virtual void DeselectObject()
    {
        highlight.SetActive(false);
    }
    protected virtual void Die()
    {
        if (ObjectDied != null)
        {
            ObjectDied(gameObject);
        }
        Destroy(gameObject);
    }
    protected virtual void SetLayerRecursively(GameObject go, int newLayer)
    {
        if (go == null)
            return;

        go.layer = newLayer;
        foreach (Transform child in go.transform)
        {
            if (child == null)
                continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    protected virtual void SetMaterialRecursively(GameObject go, Material mat)
    {
        if (go == null)
            return;

        MeshRenderer meshR;
        if (go.TryGetComponent<MeshRenderer>(out meshR))
        {
            meshR.material = mat;
        }
        foreach (Transform child in go.transform)
        {
            if (child == null)
                continue;
            SetMaterialRecursively(child.gameObject, mat);
        }
    }
    public virtual void LoadFromPreset(ScriptableObj obj)
    {
        FoodCost = obj.FoodCost;
        WoodCost = obj.WoodCost;
        StoneCost = obj.StoneCost;
        ProductionTime = obj.ProductionTime;
        MaxHealth = obj.MaxHealth;
        currentHealth = MaxHealth;
    }
}
public enum TeamEnum
{
    Unknown,
    Player,
    Enemy
}