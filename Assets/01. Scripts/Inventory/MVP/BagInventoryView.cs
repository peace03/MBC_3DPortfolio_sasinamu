using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class BagInventoryView : MonoBehaviour
{

    [Header("가방")]
    [SerializeField] private GameObject slotPrefab;         //가방 인벤토리에 배치될 슬롯
    [SerializeField] private VirtualSlot virtualSlot;       //가상 슬롯(드래그앤드롭에 사용)
    private int capacity;

    private List<SlotPrefab> _bagSlots;    //가방 프리펩 슬롯 리스트

    public void Init(VirtualSlot virtualSlot, int bagCapacity)
    {
        capacity = bagCapacity;
        //가방 인벤토리 슬롯 리스트 생성
        _bagSlots = new List<SlotPrefab>();
        //슬롯들 생성
        for(int i = 0; i < capacity; i++)
        {
            _bagSlots.Add(Instantiate(slotPrefab, transform).GetComponent<SlotPrefab>());
            _bagSlots[i].Initialize(i, SlotType.Bag, virtualSlot);
        }
    }

    //인벤토리 껏다 키기 - 이건 파사드에서 제어
    public void InventoryToggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    
    //가방 인벤토리 슬롯 한개만 업데이트
    public void UpdateBagSlot_Single(int index, Item item)
    {
        if (item is CountableItem countableItem) //적재가능 아이템
        {
            _bagSlots[index].SetSlot(countableItem._data.Sprite, countableItem._data.Name,
                countableItem.CurAmount.ToString());
        }
        else if (item == null) //비어있는 슬롯
        {
            _bagSlots[index].SetSlot();
        }
        else if (item is ConsumableItem consumableItem) //consumable 아이템
        {
            _bagSlots[index].SetSlot(consumableItem._data.Sprite, consumableItem._data.Name,
                consumableItem.CurDurability, consumableItem.MaxDruability);
        }
        else //unCountable 아이템
        {
            //Debug.Log("넌카운터블 뷰 업뎃");
            _bagSlots[index].SetSlot(item._data.Sprite, item._data.Name);
        }
    }

    
}
