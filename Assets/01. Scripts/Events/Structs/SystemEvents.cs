using System.Collections.Generic;

public struct UIStateEvent : IEvent
{
    public bool isOpenUI;
    public UIStateEvent(bool value) => isOpenUI = value;
}

public struct LootBoxEvent : IEvent
{
    public string boxName;
    public LootBoxEvent(string name)
    {
        boxName = name;
    }
}