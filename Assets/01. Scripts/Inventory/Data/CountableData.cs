using UnityEngine;

[CreateAssetMenu(fileName = "CountableData", menuName = "Data/CountableData")]
public class CountableData : ItemData
{
    [Header("스택 정보")]
    [SerializeField] private int maxAmount;

    public int MaxAmount => maxAmount;
}
