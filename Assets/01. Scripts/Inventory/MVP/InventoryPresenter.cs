using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class InventoryPresenter
{
    private InventoryView _view;
    private InventoryModel _model;
    private ItemManager _manager;

    public InventoryPresenter(InventoryView view, InventoryModel model, ItemManager manager)
    {
        _view = view;
        _model = model;
        _manager = manager;
    }

    //초기화
    public void InitializePresenter(List<ItemData> itemDatas)
    {
        _view.SetEvent(OnSlotDragDrop, UpdateStatPanel); //구독시키기
        Test();
        UpdateAllSlot(_model.GetAllItem());
    }

    #region UI Update
    public void UpdateAllSlot(List<Item> items)
    {
        for (int i = 0; i < 20; i++)
        {
            _view.UpdateBagSlot_Single(i, items[i]);
        }
    }
    //가방 슬롯 UI 업데이트
    public void UpdateSingleSlot_Bag(int index)
    {
        Item item = _model.GetBagItem(index);
        _view.UpdateBagSlot_Single(index, item);
    }
    //장비 슬롯 UI 업데이트
    public void UpdateSingleSlot_Equip(int index)
    {
        Item item = _model.GetEquipItem(index);
        _view.UpdateEquipSlot_Single(index, item);
    }

    //Slot 스왑 이벤트 발생시 스왑로직 실행
    public void OnSlotDragDrop(SwapType swapType, int fromIndex, int toIndex)
    {
        switch (swapType)
        {
            case SwapType.BagToBag:
                if (_model.ExchangeSlot_BagToBag(fromIndex, toIndex))
                {
                    UpdateSingleSlot_Bag(fromIndex);
                    UpdateSingleSlot_Bag(toIndex);
                }
                break;
            case SwapType.EquipToEquip:
                if (_model.ExchangeSlot_EquipToEquip(fromIndex, toIndex))
                {
                    UpdateSingleSlot_Equip(fromIndex);
                    UpdateSingleSlot_Equip(toIndex);
                }
                break;
            case SwapType.BagToEquip:
                if (_model.ExchangeSlot_BagToEquip(fromIndex, toIndex))
                {
                    UpdateSingleSlot_Bag(fromIndex);
                    UpdateSingleSlot_Equip(toIndex);
                }
                break;
            case SwapType.EquipToBag:
                if (_model.ExchangeSlot_EquipToBag(fromIndex, toIndex))
                {
                    UpdateSingleSlot_Equip(fromIndex);
                    UpdateSingleSlot_Bag(toIndex);
                }
                break;
            default:
                break;
        }
        //Debug.Log($"From: {fromIndex}\nTo: {toIndex}");
    }
    #endregion

    public void UpdateStatPanel(SlotSource slotSource, int index)
    {
        if (slotSource == SlotSource.Bag)
        {
            _view.SetItemStatPanel(_model.GetBagItem(index));
            
        }
        else if (slotSource == SlotSource.Equipment)
        {
            _view.SetItemStatPanel(_model.GetEquipItem(index));
        }
        else //상자에서 호출
        {

        }
    }

    void Test()
    {
        //Debug.Log(_manager.ReturnItem_One(1).name);
        //bool success = CreateItem(1);
        //if (success == true) Debug.Log("아이템 추가 성공");
        //else Debug.Log("아이템 추가 실패");

        CreateItem(1);
        CreateItem(13);
        CreateItem(20);
        CreateItem(16);
        CreateItem(8);
        CreateItem(1);
        CreateItem(18);
        CreateItem(9);
    }

    //ItemData의 id로 객체 생성하도록 M에 요청
    private bool CreateItem(int id)
    {
        //Debug.Log("P - CreateItem 호출 완료");
        //_model.CheckFullInventory();
        Item item = _manager.CreateItemInstance(id); //인스턴스 내부에서 호출 피드백
        //Debug.Log($"P - 생성된 인스턴스 Type: {item.GetType()}");
        //Debug.Log($"item: {item}");
        return _model.AddItem(item);
    }

    //아이템 추가 메서드 -> M에게 시키기
}
