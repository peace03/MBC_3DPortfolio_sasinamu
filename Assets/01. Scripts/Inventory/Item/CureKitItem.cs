using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CureKitItem : ConsumableItem
{
    public CureKitItem(CureKitData data, float curDurability)
        : base(data, curDurability) { }

    //체력 회복량
    public float CureAmount => (_data as CureKitData).CureAmount;
    //현재 내구도
    public override float CurDurability => _curDurability;

    public override float DecreaseDurability()
    {
        _curDurability += (_data as CureKitData).DurabilityConsumetion;
        return _curDurability;
    }

    //아이템 스탯창에 표시용
    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var cureKitData = _data as CureKitData;
        list.Add(("생명력 회복", $"{cureKitData.CureAmount} (소모: {cureKitData.DurabilityConsumetion})"));

        return list;
    }


}

// 부모의 가상함수 실행되고 자식의 함수 실행되도록하는 방법
