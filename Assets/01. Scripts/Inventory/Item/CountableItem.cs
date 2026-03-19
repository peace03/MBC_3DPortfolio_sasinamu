using Unity.Android.Gradle.Manifest;
using UnityEngine;

[System.Serializable]
public class CountableItem : Item
{
    public int _curAmount; //현재 아이템 개수
    public CountableItem(CountableData data, int curAmount = 1) : base(data)
    {
        _curAmount = curAmount;
    }

    public int MaxAmount => (_data as CountableData).maxAmount;
}
