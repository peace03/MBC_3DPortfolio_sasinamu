using UnityEngine;

[System.Serializable]
public class CureKitItem : ConsumableItem
{
    public CureKitItem(CureKitData data, float curDurability)
        : base(data, curDurability) { }
}
