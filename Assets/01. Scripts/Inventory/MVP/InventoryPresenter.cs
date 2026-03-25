using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public enum SlotType
{
    Equip,      //장비
    Bag,        //가방
    Box,        //상자
    Storage,    //창고
    Quick       //퀵슬롯
}

public class InventoryPresenter : ISlotExchangeHandler
{
    private FacadeView _view;
    private InventoryModel _bagModel;
    private InventoryModel _equipModel;
    private ItemManager _itemManager;

    public InventoryPresenter(FacadeView view,
        InventoryModel bagModel, InventoryModel equipModel,
        ItemManager manager)
    {
        _view = view;
        _bagModel = bagModel;
        _equipModel = equipModel;
        _itemManager = manager;
    }

    //초기화
    public void InitializePresenter()
    {
        //_bagInvenView.SetEvent(OnSlotDragDrop, UpdateStatPanel); //구독시키기
        //Debug.Log("실행1");
        Test();
        //Debug.Log("실행2");
        UpdateAllSlot(_bagModel.GetAllItem());
        //Debug.Log("실행3");
    }

    public void UpdateAllSlot(List<Item> items)
    {
        for (int i = 0; i < 20; i++)
        {
            _view.UpdateSingleSlot_Bag(i, items[i]);
        }
    }

    //public void UpdateStatPanel(SlotSource slotSource, int index)
    //{
    //    if (slotSource == SlotSource.Bag)
    //    {
    //        _bagInvenView.SetItemStatPanel(_userInvenModel.GetBagItem(index));
            
    //    }
    //    else if (slotSource == SlotSource.Equipment)
    //    {
    //        _bagInvenView.SetItemStatPanel(_userInvenModel.GetEquipItem(index));
    //    }
    //    else //상자에서 호출
    //    {

    //    }
    //}

    void Test()
    {
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
        return _bagModel.AddItem(item);
    }

    //아이템 교체
    public void onExchangeSlot(SlotType fromSlotType, int fromIndex, SlotType toSlotType, int toIndex)
    {
        Item fromItem;
        Item toItem;
        //아이템 교체
        fromItem = GetModelItem(fromSlotType, fromIndex);
        toItem = GetModelItem(toSlotType, toIndex);
        if (fromSlotType == SlotType.Equip || toSlotType == SlotType.Equip)
            if (!CanExchange(fromItem, toItem)) return; //교환 가능한지 검사
        Debug.Log("슬롯 교환 호출!!");
        PutModelItem(fromSlotType, fromIndex, toItem);
        PutModelItem(toSlotType, toIndex, fromItem);
        //UI 업데이트
        UpdateSingleSlot(fromSlotType, fromIndex);
        UpdateSingleSlot(toSlotType, toIndex);
    }
    //모델의 해당 인덱스 아이템 가져오기
    public Item GetModelItem(SlotType slotType, int index)
    {
        switch (slotType)
        {
            case SlotType.Equip:
                return _equipModel.GetItem(index);
            case SlotType.Bag:
                return _bagModel.GetItem(index);
            case SlotType.Box:
                break;
            case SlotType.Storage:
                break;
            case SlotType.Quick:
                break;
            default:
                break;
        }
        return null;
    }
    //교환 가능한지 검사
    public bool CanExchange(Item fromItem, Item toItem)
    {
        if (fromItem is GunItem && toItem is GunItem) return true;
        if (fromItem is BagItem && toItem is BagItem) return true;
        if (fromItem is ConsumableItem from && toItem is ConsumableItem to
            && from._data.GetComponent<ConsumableData>().consumableType == to._data.GetComponent<ConsumableData>().consumableType) return true;
        return false;
    }
    //모델의 해당 인덱스 아이템 넣어주기
    public void PutModelItem(SlotType slotType, int index, Item item)
    {
        switch (slotType)
        {
            case SlotType.Equip:
                _equipModel.PutItem(index, item);
                break;
            case SlotType.Bag:
                _bagModel.PutItem(index, item);
                break;
            case SlotType.Box:
                break;
            case SlotType.Storage:
                break;
            case SlotType.Quick:
                break;
            default:
                break;
        }
    }
    public void UpdateSingleSlot(SlotType slotType, int index)
    {
        Item item;
        switch (slotType)
        {
            case SlotType.Equip:
                item = _equipModel.GetItem(index);
                _view.UpdateSingleSlot_Equip(index, item);
                break;
            case SlotType.Bag:
                item = _bagModel.GetItem(index);
                _view.UpdateSingleSlot_Bag(index, item);
                break;
            case SlotType.Box:
                break;
            case SlotType.Storage:
                break;
            case SlotType.Quick:
                break;
            default:
                break;
        }
    }
}

//인벤토리 push
//아이템 사용 - 아이템 E키 누르면 데이터 반환
//아이템 버리기
//상자 창고
//장비 착용했을 때 총 오브젝트 반환
//장비