using System.Collections.Generic;

public struct UIStateEvent : IEvent
{
    public bool isUIOpen;
    public UIStateEvent(bool value) => isUIOpen = value;
}

public struct LootBoxEvent : IEvent
{
    public string boxName;
    public List<string> itemsList;
    public LootBoxEvent(string name, List<string> items)
    {
        boxName = name;
        itemsList = items;
    }
}