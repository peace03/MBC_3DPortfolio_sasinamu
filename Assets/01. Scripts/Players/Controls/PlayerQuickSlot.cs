using System.Collections;
using UnityEngine;

public class PlayerQuickSlot : MonoBehaviour, IQuickSlotHandler, IPlayerQuickSlotHandler, IPlayerCancelHandler
{
    [SerializeField] private int quickSlotCount;
    [SerializeField] private Item[] quickSlots;
    [SerializeField] private bool isUsingItem = false;
    [SerializeField] private float useItemLoadingTime;

    private InventoryModel quickSlotModel;
    private Coroutine loadingCoroutine;

    private int curSelectedSlot = 0;
    public int CurSelectedSlot => curSelectedSlot;

    private float useItemStartTime;
    private float ProgressRatio
    {
        get
        {
            if (useItemLoadingTime == 0f)
                return 0f;

            return Mathf.Clamp01((Time.time - useItemStartTime) / useItemLoadingTime);
        }
    }

    private void OnEnable()
    {
        Subject<IQuickSlotHandler>.Attach(this);
        Subject<IPlayerQuickSlotHandler>.Attach(this);
        Subject<IPlayerCancelHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IQuickSlotHandler>.Detach(this);
        Subject<IPlayerQuickSlotHandler>.Detach(this);
        Subject<IPlayerCancelHandler>.Detach(this);
    }

    public void OnQuickSlot(InventoryModel model)
    {
        quickSlotModel = model;

        for (int i = 0; i < quickSlotCount; i++)
            quickSlots[i] = model.GetItem(i);

        UpdateHoldingItemVisual();
    }

    private void UpdateHoldingItemVisual()
    {
        if (curSelectedSlot < 3 || isUsingItem)
            return;

        Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerHoldingItem(quickSlots[curSelectedSlot - 3]));
    }

    public void OnPlayerQuickSlot(int slotNumber)
    {
        if (curSelectedSlot != slotNumber)
        {
            curSelectedSlot = slotNumber;
        }

        UpdateHoldingItemVisual();

        if (slotNumber >= 3 && quickSlots[slotNumber - 3] != null)
        {
            var cureItem = quickSlots[slotNumber - 3] as CureKitItem;
            var foodItem = quickSlots[slotNumber - 3] as FoodItem;

            if ((cureItem == null && foodItem == null) || loadingCoroutine != null || isUsingItem)
                return;

            useItemStartTime = Time.time;
            isUsingItem = true;
            loadingCoroutine = StartCoroutine(UseItemLoadingCoroutine());
        }
    }

    private IEnumerator UseItemLoadingCoroutine()
    {
        while (Time.time - useItemStartTime < useItemLoadingTime)
        {
            if (Time.timeScale == 0)
            {
                yield return null;
                continue;
            }

            Subject<IProgressUIHandler>.Publish(h => h.OnStartProgress(ProgressType.UseItem, ProgressRatio));
            yield return null;
        }

        Subject<IProgressUIHandler>.Publish(h => h.OnStartProgress(ProgressType.UseItem, 1f));
        loadingCoroutine = null;
        isUsingItem = false;
        quickSlotModel.UseItem(SlotType.Quick, curSelectedSlot - 3);
    }

    public void OnCancel()
    {
        if (loadingCoroutine == null)
            return;

        StopCoroutine(loadingCoroutine);
        loadingCoroutine = null;
        isUsingItem = false;
        Subject<IProgressUIHandler>.Publish(h => h.OnCancelProgress());
    }
}