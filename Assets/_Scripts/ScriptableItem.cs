using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItem")]
public class ScriptableItem : ScriptableObject
{
   // public GameObject itemModel;
    public string itemName;
    public EquipmentSlot equipSlot;
    public MeshRenderer meshRenderer;
    public bool isTwoHanded; // move to Item -> Weapon

    // TODO: Ideas that could be nice for integrating new items
    public float itemWeight; // Maybe change animation speed based on itemWeight;

    // These will go into Item -> Weapon
    public float attackSpeed;
    public float damage;

    // These will go into Item -> Armor
    public float rangedDefense;
    public float meleeDefense;
    public float magicDefense;
}
