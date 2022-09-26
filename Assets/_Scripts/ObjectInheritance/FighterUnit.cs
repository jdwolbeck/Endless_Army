using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FighterUnit : BasicUnit
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        equippedItemManager.SetDefaultEquipment((ScriptableItem)ResourceDictionary.instance.GetPreset("ShortSword"));
        equippedItemManager.EquipDefaultEquipment(EquipmentSlot.RightWeapon);
        //if (isSpawnedFromInspector)
        LoadFromPreset((ScriptableUnit)ResourceDictionary.instance.GetPreset("Fighter"));
    }
    protected override void Update()
    {
        base.Update();
    }
}
