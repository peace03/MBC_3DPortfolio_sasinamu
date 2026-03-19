using UnityEngine;

[System.Serializable]
public class Item
{
    public ItemData _data;
    public Item(ItemData data)
    {
        _data = data;
    }
}
