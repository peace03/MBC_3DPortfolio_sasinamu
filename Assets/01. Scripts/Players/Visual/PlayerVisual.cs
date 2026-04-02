using UnityEngine;

public class PlayerVisual : MonoBehaviour, IPlayerVisualHandler
{
    [Header("공장")]
    [SerializeField] private ItemFactory itemFactory;

    [Header("장비 프리팹")]
    [SerializeField] private GameObject glockRightArm;
    [SerializeField] private GameObject ak47Arms;
    [SerializeField] private GameObject vest;
    [SerializeField] private GameObject smallInv;
    [SerializeField] private GameObject largeInv;

    [Header("퀵슬롯 프리팹")]
    [SerializeField] private GameObject itemArms;
    [SerializeField] private Transform itemPosition;

    [Header("정보")]
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject rightArmTorus;
    [SerializeField] private GameObject inventory;

    private ItemWorldObject curHoldingItem = null;

    private PlayerVisualState armState;
    private PlayerVisualState vestState;
    private PlayerVisualState invState;

    private void OnEnable()
    {
        Subject<IPlayerVisualHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IPlayerVisualHandler>.Detach(this);
    }

    public void OnPlayerVisual(PlayerVisualType type, PlayerVisualState state)
    {
        switch (type)
        {
            case PlayerVisualType.Arm:
                if (state == armState)
                    return;

                armState = state;
                ChangeArm();
                break;
            case PlayerVisualType.Vest:
                if (state == vestState)
                    return;

                vestState = state;
                ChangeVest();
                break;
            case PlayerVisualType.Inv:
                if (state == invState)
                    return;

                invState = state;
                ChangeInventory();
                break;
        }
    }

    private void ChangeArm()
    {
        leftArm.SetActive(false);
        rightArm.SetActive(false);
        rightArmTorus.SetActive(false);
        glockRightArm.SetActive(false);
        ak47Arms.SetActive(false);
        itemArms.SetActive(false);

        if (curHoldingItem != null)
        {
            curHoldingItem.ReturnToPool();
            curHoldingItem = null;
        }

        switch (armState)
        {
            case PlayerVisualState.None:
                leftArm.SetActive(true);
                rightArm.SetActive(true);
                rightArmTorus.SetActive(true);
                break;
            case PlayerVisualState.Glock:
                glockRightArm.SetActive(true);
                break;
            case PlayerVisualState.AK47:
                ak47Arms.SetActive(true);
                break;
            case PlayerVisualState.Items:
                itemArms.SetActive(true);
                break;
        }
    }

    private void ChangeVest()
    {
        switch (vestState)
        {
            case PlayerVisualState.None:
                vest.SetActive(false);
                break;
            case PlayerVisualState.Vest:
                vest.SetActive(true);
                break;
        }
    }

    private void ChangeInventory()
    {
        inventory.SetActive(false);
        smallInv.SetActive(false);
        largeInv.SetActive(false);

        switch (invState)
        {
            case PlayerVisualState.None:
                inventory.SetActive(true);
                break;
            case PlayerVisualState.SmallInv:
                inventory.SetActive(true);
                smallInv.SetActive(true);
                break;
            case PlayerVisualState.LargeInv:
                largeInv.SetActive(true);
                break;
        }
    }

    public void OnPlayerHoldingItem(Item item)
    {
        if (armState == PlayerVisualState.Items)
        {
            if (item == null)
            {
                curHoldingItem.ReturnToPool();
                curHoldingItem = null;
                return;
            }
            else if (curHoldingItem != null && curHoldingItem.DropItem._data.ID == item._data.ID)
                return;
            else if (curHoldingItem != null)
            {
                curHoldingItem.ReturnToPool();
                curHoldingItem = null;
            }
        }
        else
        {
            armState = PlayerVisualState.Items;
            ChangeArm();

            if (item == null)
                return;
        }

        var holdingItem = itemFactory.GetWorldItem(item);
        holdingItem.gameObject.transform.SetParent(itemPosition, false);
        holdingItem.gameObject.SetActive(true);
        curHoldingItem = holdingItem;
    }
}