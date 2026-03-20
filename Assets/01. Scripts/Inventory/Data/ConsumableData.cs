using UnityEngine;

public enum ConsumableType
{
    Helmat,
    Vest,
    other
}

[CreateAssetMenu(fileName = "ConsumableData", menuName = "Data/ConsumableData")]
public class ConsumableData : ItemData
{
    [Header("내구도 정보")]
    [SerializeField] private ConsumableType _consumableType; //소모성 아이템의 종류
    [SerializeField] private float maxDurability; //내구도

    public float MaxDurability => maxDurability;
    public ConsumableType consumableType => _consumableType;
}
