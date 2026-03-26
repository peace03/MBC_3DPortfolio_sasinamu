using System;
using UnityEngine;

//public enum SwapType
//{
//    BagToBag,
//    EquipToEquip,
//    BoxToBox,
//    BagToEquip,
//    EquipToBag
//}

public class FacadeView : MonoBehaviour, ISlotClickRightHandler
{
    [Header("View")]
    [SerializeField] private EquipInventoryView _equipView;     //장비 View
    [SerializeField] private BagInventoryView   _bagView;       //가방 View
    //상자 View
    //창고 View
    [SerializeField] private InteractionButton _interactButton; //상호작용 버튼

    [Header("Etc")]
    [SerializeField] private VirtualSlot _virtualSlot;          //가상 슬롯

    private int slotPointerIndex;

    private void Awake()
    {
        InitViews();
    }

    public void InitViews()
    {
        _equipView.Init(_virtualSlot);
        _bagView.Init(_virtualSlot);

        _interactButton.Init();
    }

    public void UpdateSingleSlot_Bag(int index, Item item) =>
        _bagView.UpdateBagSlot_Single(index, item);
    public void UpdateSingleSlot_Equip(int index, Item item) =>
        _equipView.UpdateEquipSlot_Single(index, item);

    //슬롯에서 마우스 우클릭 했을 때
    public void OnSlotClickRight(Transform transform)
    {
        _interactButton.SlotClickRight(transform);
    }
}
