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
}
