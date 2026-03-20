using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConsumableItem : Item
{
    private float _curDurability; //현재 내구도
    public ConsumableItem(ConsumableData data, float curDurability) : base(data)
    {
        _curDurability = curDurability;
    }

    public ConsumableType Type => (_data as ConsumableData).consumableType;

    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var consumableData = _data as ConsumableData;
        list.Add(("내구도", $"{_curDurability} / {consumableData.MaxDurability}"));

        return list;
    }
}
