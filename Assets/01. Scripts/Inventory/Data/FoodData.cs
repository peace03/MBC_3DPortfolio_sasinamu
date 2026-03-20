using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Data/FoodData")]
public class FoodData : ItemData
{
    [Header("음식 정보")]
    [SerializeField] private float waterRestoreAmount; //수분 회복력
    [SerializeField] private float energyRestoreAmount; //에너지 회복력

    public float WaterRestoreAmount => waterRestoreAmount;
    public float EnergyRestoreAmount => energyRestoreAmount;
}
