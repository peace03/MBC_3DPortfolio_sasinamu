using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class UserInventoryPresenter
{
    private UserInventoryView _userInvenView;
    private UserInventoryModel _userInvenModel;
    private ItemManager _itemManager;

    public UserInventoryPresenter(UserInventoryView view, UserInventoryModel model, ItemManager manager)
    {
        _userInvenView = view;
        _userInvenModel = model;
        _itemManager = manager;
    }

    //초기화
    public void InitializePresenter()
    {
        _userInvenView.SetEvent(OnSlotDragDrop, UpdateStatPanel); //구독시키기
        Test();
        UpdateAllSlot(_userInvenModel.GetAllItem());
    }

    #region User Inventory UI Update
    public void UpdateAllSlot(List<Item> items)
    {
        for (int i = 0; i < 20; i++)
        {
            _userInvenView.UpdateBagSlot_Single(i, items[i]);
        }
    }
    //가방 슬롯 UI 업데이트
    public void UpdateSingleSlot_Bag(int index)
    {
        Item item = _userInvenModel.GetBagItem(index);
        _userInvenView.UpdateBagSlot_Single(index, item);
    }
    //장비 슬롯 UI 업데이트
    public void UpdateSingleSlot_Equip(int index)
    {
        Item item = _userInvenModel.GetEquipItem(index);
        _userInvenView.UpdateEquipSlot_Single(index, item);
    }

    //Slot 스왑 이벤트 발생시 스왑로직 실행
    public void OnSlotDragDrop(SwapType swapType, int fromIndex, int toIndex)
    {
        switch (swapType)
        {
            case SwapType.BagToBag:
                if (_userInvenModel.ExchangeSlot_BagToBag(fromIndex, toIndex))
                {
                    UpdateSingleSlot_Bag(fromIndex);
                    UpdateSingleSlot_Bag(toIndex);
                }
                break;
            case SwapType.EquipToEquip:
                if (_userInvenModel.ExchangeSlot_EquipToEquip(fromIndex, toIndex))
                {
                    UpdateSingleSlot_Equip(fromIndex);
                    UpdateSingleSlot_Equip(toIndex);
                }
                break;
            case SwapType.BagToEquip:
                if (_userInvenModel.ExchangeSlot_BagToEquip(fromIndex, toIndex))
                {
                    UpdateSingleSlot_Bag(fromIndex);
                    UpdateSingleSlot_Equip(toIndex);
                }
                break;
            case SwapType.EquipToBag:
                if (_userInvenModel.ExchangeSlot_EquipToBag(fromIndex, toIndex))
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
            _userInvenView.SetItemStatPanel(_userInvenModel.GetBagItem(index));
            
        }
        else if (slotSource == SlotSource.Equipment)
        {
            _userInvenView.SetItemStatPanel(_userInvenModel.GetEquipItem(index));
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
        Item item = _itemManager.CreateItemInstance(id); //인스턴스 내부에서 호출 피드백
        //Debug.Log($"P - 생성된 인스턴스 Type: {item.GetType()}");
        //Debug.Log($"item: {item}");
        return _userInvenModel.AddItem(item);
    }
}
