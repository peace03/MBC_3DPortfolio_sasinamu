using UnityEngine;

[System.Serializable]
public class ConsumableItem : Item
{
    public float _curDurability; //현재 내구도
    public ConsumableItem(ConsumableData data, float curDurability) : base(data)
    {
        _curDurability = curDurability;
    }

    public ConsumableType Type => (_data as ConsumableData).consumableType;
}
