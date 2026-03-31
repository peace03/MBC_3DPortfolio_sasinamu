using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class InventoryModel
{
    private List<Item> _Slots; //인벤토리 내 아이템

    private int _capacity; //인벤토리 최대 슬롯
    private bool fullInventory; //인벤토리가 다 찼는가?

    public int Capacity => _capacity;

    public InventoryModel()
    {
        //비어있는 인벤토리 생성
        _Slots = new List<Item>();
        fullInventory = false;
    }

    public void Init(int capacity)
    {
        _capacity = capacity;
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
                totalCount = Citem.CurAmount + newCountableItem.CurAmount;
                //최대 적재량보다 작으면
                if (totalCount <= Citem.MaxAmount)
                {
                    Citem.SetCurAmount(totalCount);
                    //Debug.Log("적재 성공");
                    return true;
                }
                else
                {
                    Citem.SetCurAmount(Citem.MaxAmount);
                    newCountableItem.SetCurAmount(totalCount - Citem.CurAmount);
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
            if (cureItem.DecreaseDurability() <= 0f) 
            {
                PutItem(slotType, index, null);
                Subject<ISlotClickRightHandler>.Publish(h => h.OnAllBtnSetActive(false));
            }
            else Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(slotType, index));
            Debug.Log(cureItem.CurDurability);
        }
        else if (item is GunItem gunItem)
        {
            if (gunItem.DecreaseDurability() <= 0f)
            {
                PutGunItem(index, null);
                Subject<ISlotClickRightHandler>.Publish(h => h.OnAllBtnSetActive(false));
            }
            Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(slotType, index));
        }
        else if (item is VestItem vest)
        {

        }
        else if (item is FoodItem foodItem)
        {
            Debug.Log(foodItem.Energy + " " + foodItem.Thirst);
            Subject<IUseItemHandler>.Publish(h => h.OnUseFoodItem(foodItem.Energy, foodItem.Thirst));
            Subject<ISlotClickRightHandler>.Publish(h => h.OnAllBtnSetActive(false));
            PutItem(slotType, index, null);
        }
    }

    //총 아이템 파괴
    public void PutGunItem(int index, Item item)
    {
        _Slots[index] = item;
        if (item == null)
        {
            Subject<IEquipWear>.Publish(h => h.OnGunDestroy(index, item));
        }
    }
    //적재 아이템 넣어주기
    public int PutCountToItem(SlotType slotType, int index, int fromItemCount)
    {
        CountableItem item = _Slots[index] as CountableItem;
        int totalCount = item.CurAmount + fromItemCount;
        if (totalCount <= item.MaxAmount) item.SetCurAmount(totalCount);
        else item.SetCurAmount(item.MaxAmount);

        Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(slotType, index));
        return totalCount - item.MaxAmount;
    }
    public void PutCountFromItem(SlotType slotType, int index, int itemCount)
    {
        CountableItem item = _Slots[index] as CountableItem;
        if (itemCount <= 0) _Slots[index] = null;
        else item.SetCurAmount(itemCount);

        Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(slotType, index));
    }
    //아이템 넣어주기
    public void PutItem(SlotType slotType, int index, Item item)
    {
        _Slots[index] = item;
        Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(slotType, index));
    }
    //아이템 보내주기
    public Item GetItem(int index)
    {
        return _Slots[index];
    }
    public GameObject GetItemObject (int index)
    {
        GameObject gameObject = _Slots[index]._data.ObjectPrefab;
        return gameObject;
    }
    //모든 아이템 보내주기
    public List<Item> GetAllItem()
    {
        return _Slots;
    }
    //특정 아이템의 총 보유량을 메모리에서 긁어오는 함수
    public int GetTotalItemCount(int itemID)
    {
        int total = 0;
        for (int i = 0; i < _capacity; i++)
        {
            if (_Slots[i] is CountableItem cItem && cItem._data.ID == itemID)
            {
                total += cItem.CurAmount;
            }
        }
        return total;
    }
    //특정 아이템을 지정된 수량만큼 차감하고, '남은 차감량'을 반환하는 함수 (로우 레벨 제어)
    public int ConsumeItem(SlotType slotType, int itemID, int amountToConsume)
    {
        for (int i = 0; i < _capacity; i++)
        {
            if (amountToConsume <= 0) break; // 모두 차감했으면 루프 종료

            if (_Slots[i] is CountableItem cItem && cItem._data.ID == itemID)
            {
                if (cItem.CurAmount <= amountToConsume)
                {
                    // 슬롯에 있는 양보다 빼야 할 양이 같거나 많으면: 슬롯을 파괴(null)하고 남은 차감량 계산
                    amountToConsume -= cItem.CurAmount;
                    _Slots[i] = null;
                }
                else
                {
                    // 슬롯에 여유가 있다면: 수량만 깎고 차감 완료(0) 처리
                    cItem.SetCurAmount(cItem.CurAmount - amountToConsume);
                    amountToConsume = 0;
                }
                // 수량이 변했으므로 View에게 픽셀을 다시 그리라고 브로드캐스팅
                Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(slotType, i));
            }
        }
        return amountToConsume;
    }

    public float GetTotalWeight()
    {
        float totalWeight = 0f;

        foreach (var slot in _Slots)
            if (slot != null)
                totalWeight += slot._data.Weight;

        return totalWeight;
    }
}
