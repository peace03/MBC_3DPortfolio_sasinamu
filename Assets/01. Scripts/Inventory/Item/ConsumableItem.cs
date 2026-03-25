using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ConsumableItem : Item
{
    private float _curDurability; //현재 내구도
    public ConsumableItem(ConsumableData data, float curDurability) : base(data)
    {
        _curDurability = curDurability;
    }

    public virtual float Use()
    {
        _curDurability -= _data.GetComponent<CureKitData>().DurabilityConsumetion;
        return _curDurability;
    }

    public ConsumableType Type => (_data as ConsumableData).ConsumableType;

    //아이템 스탯창 표시용
    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var consumableData = _data as ConsumableData;
        list.Add(("내구도", $"{_curDurability} / {consumableData.MaxDurability}"));

        return list;
    }

    
}
