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
    public float maxDurability; //내구도

    public ConsumableType consumableType => _consumableType;
}
//나머지 변수도 캡슐화 해놓기
