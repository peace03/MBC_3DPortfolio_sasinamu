using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ConsumableItem : Item
{
    protected float _curDurability; //현재 내구도
    public ConsumableItem(ConsumableData data, float curDurability) : base(data)
    {
        _curDurability = curDurability;
    }

    //내구도 감소
    public virtual float DecreaseDurability()
    {
        return _curDurability;
    }

    public float MaxDruability => (_data as ConsumableData).MaxDurability;
    public virtual float CurDurability => _curDurability;
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
