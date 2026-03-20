using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletItem : CountableItem
{
    public BulletItem(BulletData data, int curAmount = 1)
        : base(data, curAmount) { }

    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var bulletData = _data as BulletData;
        list.Add(("데미지", bulletData.Damage.ToString()));
        list.Add(("방어구 파괴", bulletData.DurabilityDamage.ToString()));

        return list;
    }
}
