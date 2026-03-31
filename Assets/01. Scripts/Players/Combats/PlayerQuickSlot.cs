using UnityEngine;

public class PlayerQuickSlot : MonoBehaviour, IPlayerQuickSlotHandler
{
    [SerializeField] private int curQuickSlotIndex;

    private void OnEnable()
    {
        Subject<IPlayerQuickSlotHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IPlayerQuickSlotHandler>.Detach(this);
    }

    public void OnQuickSlot(int slotNumber)
    {
        curQuickSlotIndex = slotNumber;

    }
}