using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class InventoryModel
{
    private List<Item> _Slots; //인벤토리 내 아이템

    private int _capacity; //인벤토리 최대 슬롯
    private bool fullInventory; //인벤토리가 다 찼는가?

    public InventoryModel()
    {
        //비어있는 인벤토리 생성
        _Slots = new List<Item>();
        fullInventory = false;
    }

    public void Init()
    {
        for (int i = 0; i < _capacity; i++) _Slots.Add(null);
    }

    #region AddItem
    //아이템 추가 메서드
    public bool AddItem(Item newItem)
    {
        bool success;
        //countable일 때
        if(newItem is CountableItem newCountableItem)
        {
            //Debug.Log("countable 타입 슬롯 추가 메서드 실행");
            success = AddListCountableItem(newCountableItem);
        }
        //countable 아닐 때 && 비어있는 슬롯 있으면
        else
        {
            //Debug.Log("Uncountable 타입 슬롯 추가 메서드 실행");
            //그냥 넣기
            success = AddListItem(newItem);
        }
        return success;
    }

    //countable아이템 슬롯에 추가
    private bool AddListCountableItem(CountableItem newCountableItem)
    {
        int totalCount = 0;
        //Debug.Log("인벤토리 아이템 탐색");
        foreach(var item in _Slots) //linq 사용 피드백
        {
            //동일 아이템이 있다면
            if (item is CountableItem Citem && Citem._data.ID == newCountableItem._data.ID)
            {
                //Debug.Log("동일 아이템 발견");
                totalCount = Citem._curAmount + newCountableItem._curAmount;
                //최대 적재량보다 작으면
                if (totalCount <= Citem.MaxAmount)
                {
                    Citem._curAmount = totalCount;
                    //Debug.Log("적재 성공");
                    return true;
                }
                else
                {
                    Citem._curAmount = Citem.MaxAmount;
                    newCountableItem._curAmount = totalCount - Citem._curAmount;
                }
            }
        }
        //그래도 남으면 새로 생성
        //Debug.Log("새로 생성 시도");
        return AddListItem(newCountableItem);
    }

    //비어있는 슬롯에 추가
    //최대 개수로 생성 혹은 만들어주도록 테스틐 코드 작성
    private bool AddListItem(Item newItem)
    {
        if (fullInventory == true) //꽉차있을 때
        {
            //Debug.Log("! 인벤토리가 꽉 차있습니다 !");
            return false;
        }
        else //비어있을 때
        {
            for (int i = 0; i < _capacity; i++)
            {
                if (_Slots[i] == null)
                {
                    _Slots[i] = newItem;
                    //Debug.Log("새로 생성 성공");
                    return true;
                }
            }
        }
        fullInventory = true;
       // Debug.Log("새로 생성 실패");
        return false;
    }
    #endregion

    //아이템 사용 - 아이템 소멸되면 true 반환
    public void UseItem(SlotType slotType, int index)
    {
        Item item = _Slots[index];
        if (item is CureKitItem cureItem)
        {
            Subject<IUseItemHandler>.Publish(h => h.OnUseCureItem(cureItem.CureAmount));
            //내구도 0이하면 아이템 없애기
            if (cureItem.Use() <= 0f) 
            {
                _Slots[index] = null;
                Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(slotType, index));
            }
        }
        else if (item is FoodItem foodItem)
        {
            Subject<IUseItemHandler>.Publish(h => h.OnUseFoodItem(foodItem.Energy, foodItem.Thirst));
            _Slots[index] = null;
            Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(slotType, index));
            
        }
    }

    //아이템 넣어주기
    public void PutItem(int index, Item item)
    {
        _Slots[index] = item;
    }
    //아이템 보내주기
    public Item GetItem(int index)
    {
        return _Slots[index];
    }
    //모든 아이템 보내주기
    public List<Item> GetAllItem()
    {
        return _Slots;
    }
    //슬롯 용량 설정
    public void SetCapacity(int capacity)
    {
        _capacity = capacity;
    }

    //인벤토리 다 찼는지 확인
    //? countable이 다 안찼을 땐 어캄?
    public void CheckFullInventory()
    {
        fullInventory = true;
        foreach(var item in _Slots)
        {
            if (item == null) fullInventory = false;
        }
        Debug.Log($"인벤토리 다 꽉찼는지: {fullInventory}");
    }
}
