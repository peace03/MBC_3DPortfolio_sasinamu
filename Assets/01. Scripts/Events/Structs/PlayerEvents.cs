using UnityEngine;

public struct MoveEvent : IEvent
{
    public Vector2 moveInput;
    public MoveEvent(Vector2 input) => moveInput = input;
}

public struct RunEvent : IEvent
{
    public bool isPressed;
    public RunEvent(bool value) => isPressed = value;
}

public struct MousePosEvent : IEvent
{
    public Vector2 mousePos;
    public MousePosEvent(Vector2 pos) => mousePos = pos;
}

public struct RollEvent : IEvent {}

public struct InteractEvent : IEvent {}

public struct EscEvent : IEvent {}

public struct FireEvent : IEvent
{
    public bool isPressed;
    public FireEvent(bool value) => isPressed = value;
}