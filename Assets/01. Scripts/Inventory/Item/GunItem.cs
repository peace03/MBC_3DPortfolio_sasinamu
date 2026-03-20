using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunItem : ConsumableItem
{
    public int _curAmmo; //현재 탄약 수
    public GunItem(GunData data, int curAmmo, float curDurability)
        : base(data, curDurability)
    {
        _curAmmo = curAmmo;
    }

    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var gunData = _data as GunData;
        list.Add(("현재 장전량", $"{_curAmmo} / {gunData.MaxAmmo}"));
        list.Add(("발사 속도", $"{gunData.FireSpeed}"));

        return list;
    }
}
