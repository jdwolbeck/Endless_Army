using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenterBuilding : BasicBuilding
{
    public override void SelectBuilding()
    {
        base.SelectBuilding();
        GetComponent<ProductionHandler>().BuildingSelectionEvent(true);
    }
    public override void DeselectBuilding()
    {
        base.DeselectBuilding();
        GetComponent<ProductionHandler>().BuildingSelectionEvent(false);
    }
}
