using UnityEngine;

[CreateAssetMenu(fileName = "VestData", menuName = "Data/VestData")]
public class VestData : ConsumableData
{
    [Header("조끼 정보")]
    [SerializeField] private float defensive; //방어력 20% 피해 감소

    public float Defensive => defensive;
}
