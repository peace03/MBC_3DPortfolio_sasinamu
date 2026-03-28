using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public ItemData _data;
    public Item(ItemData data)
    {
        _data = data;
    }

    public GameObject Object => _data.ObjectPrefab;

    public virtual List<(string statName, string statValue)> GetItemStats()
    {
        return new List<(string, string)>();
    }
}
