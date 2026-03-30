using System;
using TMPro;
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
    [SerializeField] private EquipInventoryView _equipView;     //장비    View
    [SerializeField] private BagInventoryView   _bagView;       //가방    View
    [SerializeField] private StorageView        _storageView;   //창고    View
    [SerializeField] private QuickInventoryView _quickView;     //퀵슬롯  View
    [SerializeField] private BoxInventoryView   _boxView;       //상자    View
    [SerializeField] private WorkStationView    _workStaionView;//작업대  View

    [Header("Etc")]
    [SerializeField] private VirtualSlot _virtualSlot;          //가상 슬롯
    [SerializeField] private ItemStatPanel _itemStatPanel;       //아이템 스탯창
    [SerializeField] private InteractionButton _interactButton; //상호작용 버튼
    [SerializeField] private Transform background;              //UI 배경화면

    private int slotPointerIndex;
    private Transform preTransform;

    public void InitViews(int bagCapacity, int quickCapacity, int storageCapacity, int boxCapacity)
    {
        _equipView.Init(_virtualSlot);
        _bagView.Init(_virtualSlot, bagCapacity);
        _quickView.Init(_virtualSlot, quickCapacity);
        _storageView.Init(_virtualSlot, storageCapacity);
        _boxView.Init(_virtualSlot, boxCapacity);
        _workStaionView.Init();

        _interactButton.Init();
    }

    //슬롯 업데이트
    public void UpdateSingleSlot_Equip(int index, Item item) =>
        _equipView.UpdateEquipSlot_Single(index, item);
    public void UpdateSingleSlot_Bag(int index, Item item) =>
        _bagView.UpdateBagSlot_Single(index, item);
    public void UpdateSingleSlot_Storage(int index, Item item) =>
        _storageView.UpdateStorageSlot_Single(index, item);
    public void UpdateSingleSlot_Quick(int index, Item item) =>
        _quickView.UpdateQuickSlot_Single(index, item);
    public void UpdateSingleSlot_Box(int index, Item item) =>
        _boxView.UpdateBoxSlot_Single(index, item);

    //아이템 스탯창
    public void SetItemStatPanel(Item item)
    {
        if (item == null) _itemStatPanel.InActiveToggle();
        else
        {
            _itemStatPanel.ActiveToggle();
            _itemStatPanel.SetStatPanel(item);
        }
    }
    public void StatPanelInActive()
    {
        _itemStatPanel.InActiveToggle();
    }

    //슬롯에서 마우스 우클릭 했을 때
    public void OnSlotClickRight(Transform transform)
    {
        if (transform == background)
        {
            _interactButton.gameObject.SetActive(false);
            Debug.Log("실행1");
        }
        else if (preTransform == transform)
        {
            _interactButton.gameObject.SetActive(!_interactButton.gameObject.activeSelf);
            Debug.Log("실행2");
        }
        else _interactButton.gameObject.SetActive(true);
        preTransform = transform;

        _interactButton.SlotClickRight(transform);
    }

    public void OnAllBtnSetActive(bool setActive)
    {
        _interactButton.gameObject.SetActive(setActive);
    }

    public void OnUseBtnSetActive(bool setActive)
    {
        //Debug.Log($"{_interactButton.UseButton.GetChild(0).GetComponent<TextMeshProUGUI>().text}");
        _interactButton.UseButton.gameObject.SetActive(setActive);
    }
}
