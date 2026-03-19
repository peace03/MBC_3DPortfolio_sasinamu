using UnityEngine;

[CreateAssetMenu(fileName = "CountableData", menuName = "Data/CountableData")]
public class CountableData : ItemData
{
    [Header("스택 정보")]
    public int maxAmount;
}
