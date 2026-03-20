using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BagItem : Item
{
    public BagItem(BagData data) : base(data) { }

    public override List<(string statName, string statValue)> GetItemStats()
    {
        var list = base.GetItemStats();

        var bagData = _data as BagData;
        list.Add(("가방 공간", "+" + bagData.AddSlotNum));

        return list;
    }
}
