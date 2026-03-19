using UnityEngine;

[System.Serializable]
public class BulletItem : CountableItem
{
    public BulletItem(BulletData data, int curAmount = 1)
        : base(data, curAmount) { }
}
