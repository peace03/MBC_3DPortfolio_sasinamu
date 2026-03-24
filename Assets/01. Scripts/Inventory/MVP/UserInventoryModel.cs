using UnityEngine;
using System.Collections.Generic;
using System;

public class UserInventoryModel
{
    private List<Item> _equipSlots; //장비 아이템 슬롯들
    private List<Item> _bagSlots; //인벤토리 내 아이템

    [SerializeField] private int capacity = 20; //인벤토리 최대 슬롯
    private int equipCapacity = 5; //장비 아이템 최대 슬롯
    private bool fullInventory; //인벤토리가 다 찼는가?

    public UserInventoryModel()
    {
        //비어있는 인벤토리 생성
        _equipSlots = new List<Item>();
        _bagSlots = new List<Item>();
        fullInventory = false;
        for (int i = 0; i < equipCapacity; i++) _equipSlots.Add(null);
        for (int i = 0; i < capacity; i++) _bagSlots.Add(null);
    }

    #region ExchangeSlot
    public bool ExchangeSlot_BagToBag(int fromIndex, int toIndex)
    {
        Item temp = _bagSlots[fromIndex];
        _bagSlots[fromIndex] = _bagSlots[toIndex];
        _bagSlots[toIndex] = temp;
        return true;
    }
    public bool ExchangeSlot_EquipToEquip(int fromIndex, int toIndex)
    {
        //무기만 swap가능
        if (fromIndex > 1 || toIndex > 1) return false;
        else
        {
            // Debug.Log("EquipToEquip 실행");
            Item temp = _equipSlots[fromIndex];
            _equipSlots[fromIndex] = _equipSlots[toIndex];
            _equipSlots[toIndex] = temp;
        }
        return true;
    }
    public bool ExchangeSlot_BagToEquip(int fromIndex, int toIndex)
    {
        if (toIndex <= 1 && _bagSlots[fromIndex] is GunItem) //무기 슬롯으로 옮길 때
        {
            SwapBag_Equip(fromIndex, toIndex);
            return true;
        }
        else if (toIndex == 2 && _bagSlots[fromIndex] is BagItem) //가방 슬롯으로 옮길 때
        {
            SwapBag_Equip(fromIndex, toIndex);
            return true;
        }
        else if (toIndex == 3 && _bagSlots[fromIndex] is ConsumableItem consumableItem && consumableItem.Type == ConsumableType.Helmat) //방탄모 슬롯으로 옮길 때
        {
            SwapBag_Equip(fromIndex, toIndex);
            return true;
        }
        else if (toIndex == 4 && _bagSlots[fromIndex] is ConsumableItem consumableItem2 && consumableItem2.Type == ConsumableType.Vest) //방탄복 슬롯으로 옮길 때
        {
            SwapBag_Equip(fromIndex, toIndex);
            return true;
        }
        return false;
    }
    public bool ExchangeSlot_EquipToBag(int fromIndex, int toIndex)
    {
        if (_bagSlots[toIndex] == null) { SwapBag_Equip(toIndex, fromIndex); return true; }
        return false;
    }
    //Equip -> bag Swap
    //반대방향으로 하려면 인덱스만 바꾸어서 넣어주면 됨
    private void SwapBag_Equip(int firstIndex, int SecondIndex)
    {
        Item temp = _equipSlots[SecondIndex];
        _equipSlots[SecondIndex] = _bagSlots[firstIndex];
        _bagSlots[firstIndex] = temp;
    }
    #endregion

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
        foreach(var item in _bagSlots) //linq 사용 피드백
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
            for (int i = 0; i < capacity; i++)
            {
                if (_bagSlots[i] == null)
                {
                    _bagSlots[i] = newItem;
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

    public Item GetEquipItem(int index)
    {
        return _equipSlots[index];
    }
    public Item GetBagItem(int index)
    {
        return _bagSlots[index];
    }
    public List<Item> GetAllItem()
    {
        return _bagSlots;
    }

    //인벤토리 다 찼는지 확인
    //? countable이 다 안찼을 땐 어캄?
    public void CheckFullInventory()
    {
        fullInventory = true;
        foreach(var item in _bagSlots)
        {
            if (item == null) fullInventory = false;
        }
        Debug.Log($"인벤토리 다 꽉찼는지: {fullInventory}");
    }
}
