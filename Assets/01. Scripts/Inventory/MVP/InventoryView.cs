using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
public enum SwapType
{
    BagToBag,
    EquipToEquip,
    BagToEquip,
    EquipToBag
}
public enum SlotSource //슬롯 출처
{
    Bag,        //가방 슬롯
    Equipment,  //장비 슬롯
    Box         //상자 슬롯
}

public class InventoryView : MonoBehaviour
{
    [Header("인벤토리")]
    [SerializeField] private List<EquipmentSlot> _equipSlots;
    public Transform bagSlots;          //가방 인벤토리 위치

    [Header("가방")]
    [SerializeField] private GameObject slotPrefab;         //가방 인벤토리에 배치될 슬롯
    [SerializeField] private VirtualSlot virtualSlot;       //가상 슬롯(드래그앤드롭에 사용)
    [SerializeField] private ItemStatPanel itemStatPanel;   //아이템 스탯 패널
    public int capacity = 20;

    private List<SlotPrefab> _bagSlots;    //가방 프리펩 슬롯 리스트
    private Action<SwapType, int, int> onDragDrop;  //슬롯에 아이템 드롭할 때
    private Action<SlotSource, int> onSlotCusor;               //슬롯위에 커서있을 때

    private void Awake()
    {
        //장비 인벤토리 슬롯 초기화
        _equipSlots = new List<EquipmentSlot>();
        for (int i = 0; i < 5; i++)
        {
            _equipSlots.Add(transform.GetChild(i + 1).GetComponent<EquipmentSlot>());
            _equipSlots[i].Initialize(i, virtualSlot,
                (swapType, fromIndex, toIndex) => onDragDrop(swapType, fromIndex, toIndex),
                (slotSource, index) => onSlotCusor(slotSource, index),
                () => itemStatPanel.InActiveToggle());
        }

        //가방 인벤토리 슬롯 초기화
        _bagSlots = new List<SlotPrefab>();
        for(int i = 0; i < capacity; i++)
        {
            _bagSlots.Add(Instantiate(slotPrefab, bagSlots).GetComponent<SlotPrefab>());
            _bagSlots[i].Initialize(i, virtualSlot,
                (swapType, fromIndex, toIndex) => onDragDrop(swapType, fromIndex, toIndex),
                (source, index) => onSlotCusor(source, index),
                () => itemStatPanel.InActiveToggle());
            //Debug.Log($"index: {_slots[i].Index} 슬롯 생성");
        }
    }

    //인벤토리 껏다 키기
    public void InventoryToggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }


    //현재 안씀
    public void UpdataAllSlot(List<Item> slots)
    {
        for(int i = 0; i < capacity; i++)
        {;
            if (slots[i] == null) //비어있는 슬롯일 때
            {
                _bagSlots[i].SetSlot();
            }
            else if(slots[i] is CountableItem countableSlot) //countable 아이템일 때
            {
                _bagSlots[i].SetSlot(countableSlot._data.sprite, countableSlot._data._name,
                    countableSlot._curAmount.ToString());
            }
            else //UnCountable 아이템일 때
            {
                _bagSlots[i].SetSlot(slots[i]._data.sprite, slots[i]._data._name);
            }
        }
    }

    //장비 인벤토리 슬롯 한개만 업데이트
    public void UpdateEquipSlot_Single(int index, Item item)
    {
        if (item == null) _equipSlots[index].SetSlot();
        else _equipSlots[index].SetSlot(item._data.sprite, item._data._name);
    }
    //가방 인벤토리 슬롯 한개만 업데이트
    public void UpdateBagSlot_Single(int index, Item item)
    {
        if (item is CountableItem countableItem)
        {
            _bagSlots[index].SetSlot(countableItem._data.sprite, countableItem._data._name,
                countableItem._curAmount.ToString());
            //Debug.Log("카운터블 뷰 업뎃");
        }
        else if (item == null)
        {
            _bagSlots[index].SetSlot();
            //Debug.Log("비어있는 뷰 업뎃");
        }
        else
        {
            //Debug.Log("넌카운터블 뷰 업뎃");
            _bagSlots[index].SetSlot(item._data.sprite, item._data._name);
        }
    }

    public void SetItemStatPanel(Item item)
    {
        if (item == null) itemStatPanel.InActiveToggle();
        else
        {
            itemStatPanel.ActiveToggle();
            itemStatPanel.SetStatPanel(item);
        }
    }

    //드롭 끝날 때 호출할 이벤트 설정
    public void SetEvent(Action<SwapType, int, int> callback1, Action<SlotSource, int> callback2)
    {
        onDragDrop = callback1;
        onSlotCusor = callback2;
    }
}
