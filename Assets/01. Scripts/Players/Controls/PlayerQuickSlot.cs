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
    }

    private void OnDisable()
    {
        Subject<IQuickSlotHandler>.Detach(this);
    }

    public void OnQuickSlot(InventoryModel model)
    {
        quickSlotModel = model;

        for (int i = 0; i < quickSlotCount; i++)
            quickSlots[i] = model.GetItem(i);
    }

    public void OnQuickSlot(int slotNumber)
    {
        curSelectedSlot = slotNumber;

        if (slotNumber > 2)
            quickSlotModel.UseItem(SlotType.Quick, slotNumber - 3);
    }
}