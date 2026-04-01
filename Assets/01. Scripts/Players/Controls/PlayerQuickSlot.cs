using UnityEngine;

public class PlayerQuickSlot : MonoBehaviour, IQuickSlotHandler, IPlayerQuickSlotHandler
{
    [SerializeField] private int quickSlotCount;
    [SerializeField] private Item[] quickSlots;

    private InventoryModel quickSlotModel;

    private int curSelectedSlot = 1;
    public int CurSelectedSlot => curSelectedSlot;

    private void Awake()
    {
        quickSlots = new Item[quickSlotCount];
    }

    private void OnEnable()
    {
        Subject<IQuickSlotHandler>.Attach(this);
        Subject<IPlayerQuickSlotHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IQuickSlotHandler>.Detach(this);
        Subject<IPlayerQuickSlotHandler>.Detach(this);
    }

    public void OnQuickSlot(InventoryModel model)
    {
        quickSlotModel = model;

        for (int i = 0; i < quickSlotCount; i++)
            quickSlots[i] = model.GetItem(i);

        OnQuickSlot(curSelectedSlot);
    }

    public void OnQuickSlot(int slotNumber)
    {
        if (curSelectedSlot != slotNumber)
            curSelectedSlot = slotNumber;

        if (slotNumber < 3)
            return;

        if (quickSlots[slotNumber - 3] != null)
            Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerHoldingItem(quickSlots[slotNumber - 3].Object));
        else
            Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerHoldingItem(null));
    }
}