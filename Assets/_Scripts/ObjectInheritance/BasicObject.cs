using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicObject : MonoBehaviour
{
    protected const int invalidTeamId = 1337;

    public bool isSpawnedFromInspector;
    public int Team = invalidTeamId;
    public GameObject highlight;
    public GameObject TeamIndicators;
    public int FoodCost;
    public int WoodCost;
    public int StoneCost;
    public float ProductionTime;
    public float MaxHealth;
    [SerializeField] protected GameObject HealthBar;
    protected GameObject HealthBarCanvas;
    protected Slider HealthSlider;
    protected float currentHealth;
    protected bool isAddedToObjectList;

    public delegate void ObjectDeathEvent(GameObject go);
    public event ObjectDeathEvent ObjectDied;

    protected virtual void Awake() 
    {
        HealthSlider = HealthBar.GetComponent<Slider>();
        HealthBarCanvas = HealthBar.transform.parent.gameObject;
        HealthBarCanvas.SetActive(false);
    }
    protected virtual void Start() 
    {
        //Team = TeamManager.instance.AssignTeam(gameObject.layer);
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
        SkinnedMeshRenderer skinMeshR;
        if (go.TryGetComponent<MeshRenderer>(out meshR))
        {
            meshR.material = mat;
        }
        else if (go.TryGetComponent<SkinnedMeshRenderer>(out skinMeshR))
        {
            skinMeshR.material = mat;
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
    public virtual void TakeDamage(float damage, BasicObject attackingObject)
    {
        currentHealth -= damage;
        //Debug.Log(gameObject.ToString() + ": Took damage");
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (!HealthBarCanvas.activeInHierarchy)
            {
                Debug.Log("Setting canvas to active.");
                HealthBarCanvas.SetActive(true);
            }
            HealthSlider.normalizedValue = currentHealth / MaxHealth;
        }
    }
}

/*
public enum TeamEnum
{
    Unknown,
    Player,
    Enemy
}
*/