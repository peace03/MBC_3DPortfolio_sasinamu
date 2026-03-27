using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class FoodItem : Item
{
    public FoodItem(FoodData data) : base(data) { }

    public float Thirst => (_data as FoodData).WaterRestoreAmount;
    public float Energy => (_data as FoodData).EnergyRestoreAmount;

    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var foodData = _data as FoodData;
        list.Add(("에너지", $"{foodData.EnergyRestoreAmount}"));
        list.Add(("수분", $"{foodData.WaterRestoreAmount}"));

        return list;
    }
}
