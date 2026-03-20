using UnityEngine;

[CreateAssetMenu(fileName = "CureKitData", menuName = "Data/CureKitData")]
public class CureKitData : ConsumableData
{
    [Header("응급상자 정보")]
    [SerializeField] private float cureAmount; //회복량
    [SerializeField] private float durabilityConsumetion; //내구도 소모량

    public float CureAmount => cureAmount;
    public float DurabilityConsumetion => durabilityConsumetion;
}
