using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentSlot
{
    Head,
    Shoulder,
    Chest,
    Hands,
    Legs,
    Feet,
    LeftWeapon,
    RightWeapon
}

public class ItemManager : MonoBehaviour
{
    public ScriptableItem[] DefaultEquipment;
    public ScriptableItem[] CurrentEquipment;
    public MeshRenderer[] CurrentMeshRenderersList;
    public Transform leftHandHoldBone;
    public Transform rightHandHoldBone;

    private void Awake()
    {
        int numOfSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        DefaultEquipment = new ScriptableItem[numOfSlots];
        CurrentEquipment = new ScriptableItem[numOfSlots];
        CurrentMeshRenderersList = new MeshRenderer[numOfSlots];
    }

    public void Equip(ScriptableItem newItem)
    {
        if (newItem == null)
            return;

        //Debug.Log("Equipping " + newItem.itemName);
        int slotIndex = (int)newItem.equipSlot;
        Unequip((EquipmentSlot)slotIndex);
        CurrentEquipment[slotIndex] = newItem;

        Transform parentTransform; 
        if (newItem.equipSlot == EquipmentSlot.LeftWeapon)
        {
            parentTransform = leftHandHoldBone.transform;
            GetComponent<BasicUnit>().SetAnimatorLayerWeight("LeftHand Layer", 1.0f);
            if (newItem.isTwoHanded)
                Unequip(EquipmentSlot.RightWeapon);
        }
        else if (newItem.equipSlot == EquipmentSlot.RightWeapon)
        {
            parentTransform = rightHandHoldBone.transform;
            GetComponent<BasicUnit>().SetAnimatorLayerWeight("RightHand Layer", 1.0f);
            if (newItem.isTwoHanded)
                Unequip(EquipmentSlot.LeftWeapon);
        }
        else
        {
            parentTransform = transform;
        }

        MeshRenderer newMeshRenderer = Instantiate<MeshRenderer>(newItem.meshRenderer, parentTransform);
        CurrentMeshRenderersList[slotIndex] = newMeshRenderer;
    }
    public void SetDefaultEquipment(ScriptableItem item)
    {
        DefaultEquipment[(int)item.equipSlot] = item;
    }
    public void EquipDefaultEquipment(EquipmentSlot slot)
    {
        if (DefaultEquipment[(int)slot] == null)
            Unequip(slot);
        else
            Equip(DefaultEquipment[(int)slot]);
    }
    public void Unequip(EquipmentSlot slot)
    {
        //Debug.Log("Unequip slot " + slot.ToString());
        if (CurrentEquipment[(int)slot] != null)
        {
            CurrentEquipment[(int)slot] = null;
            Destroy(CurrentMeshRenderersList[(int)slot].gameObject);
            CurrentMeshRenderersList[(int)slot] = null;


            if (slot == EquipmentSlot.LeftWeapon)
            {
                GetComponent<BasicUnit>().SetAnimatorLayerWeight("LeftHand Layer", 0);
            }
            else if (slot == EquipmentSlot.RightWeapon)
            {
                GetComponent<BasicUnit>().SetAnimatorLayerWeight("RightHand Layer", 0);
            }
        }
    }
}
