using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CureKitItem : ConsumableItem
{
    public CureKitItem(CureKitData data, float curDurability)
        : base(data, curDurability) { }

    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var cureKitData = _data as CureKitData;
        list.Add(("생명력 회복", $"{cureKitData.CureAmount} (소모: {cureKitData.DurabilityConsumetion})"));

        return list;
    }
}
