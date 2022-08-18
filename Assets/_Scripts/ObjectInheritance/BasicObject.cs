using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObject : MonoBehaviour
{
    public TeamEnum Team;
    public GameObject TeamIndicators; // TODO
    protected virtual void Awake() { }
    protected virtual void Start() 
    {
        Team = TeamManager.instance.AssignTeam(gameObject.layer);
    }
    public virtual void DoAction() { }
    protected virtual void Die()
    {
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
}
public enum TeamEnum
{
    Unknown,
    Player,
    Enemy
}