using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VestItem : ConsumableItem
{
    public VestItem(VestData data, float curDurability)
        : base(data, curDurability)
    {
        
    }

    public override float DecreaseDurability()
    {
        _curDurability += -5;
        return _curDurability;
    }

    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var VestData = _data as VestData;
        list.Add(("방어력", $"{VestData.Defensive}"));

        return list;
    }
}
