using UnityEngine;

[CreateAssetMenu(fileName = "CureKitData", menuName = "Data/CureKitData")]
public class CureKitData : ConsumableData
{
    [Header("응급상자 정보")]
    public float cureAmount; //회복량
    public float durabilityConsumetion; //내구도 소모량
}
