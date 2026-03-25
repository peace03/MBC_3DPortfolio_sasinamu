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

public class FacadeView : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private EquipInventoryView _equipView; //장비 View
    [SerializeField] private BagInventoryView   _bagView;   //가방 View

    [Header("Etc")]
    [SerializeField] private VirtualSlot _virtualSlot;                  //가상 슬롯

    private void Awake()
    {
        InitViews();
    }

    public void InitViews()
    {
        _equipView.Init(_virtualSlot);
        _bagView.Init(_virtualSlot);
    }

    public void UpdateSingleSlot_Bag(int index, Item item) =>
        _bagView.UpdateBagSlot_Single(index, item);
    public void UpdateSingleSlot_Equip(int index, Item item) =>
        _equipView.UpdateEquipSlot_Single(index, item);
    
}
