using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Data/FoodData")]
public class FoodData : ItemData
{
    [Header("음식 정보")]
    public float waterRestoreAmount; //수분 회복력
    public float energyRestoreAmount; //에너지 회복력
}
