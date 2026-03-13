public struct DeadEvent : IEvent
{
    public bool isPlayer;
    public DeadEvent(bool isPlayer)
    {
        this.isPlayer = isPlayer;
    }
}

public struct DamagedEvent : IEvent
{
    public IDamageable attacker;
    public IDamageable target;
    public float amount;
    public DamagedEvent(IDamageable attacker, float amount)
    {
        this.attacker = attacker;
        target = null;
        this.amount = amount;
    }
}

public struct UIStateEvent : IEvent
{
    public bool isOpenUI;
    public UIStateEvent(bool value) => isOpenUI = value;
}

public struct BoxEvent : IEvent
{
    public string boxName;
    public BoxEvent(string name)
    {
        boxName = name;
    }
}

public struct InventoryEvent : IEvent {}

public struct MapEvent : IEvent {}

public struct ShowControlsEvent : IEvent {}