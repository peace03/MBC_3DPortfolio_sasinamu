using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum SlotSource //슬롯 출처
{
    Bag,        //가방 슬롯
    Equipment,  //장비 슬롯
    Box         //상자 슬롯
}

public class BagInventoryView : MonoBehaviour
{

    [Header("가방")]
    [SerializeField] private GameObject slotPrefab;         //가방 인벤토리에 배치될 슬롯
    [SerializeField] private VirtualSlot virtualSlot;       //가상 슬롯(드래그앤드롭에 사용)
    [SerializeField] private ItemStatPanel itemStatPanel;   //아이템 스탯 패널
    public int capacity = 20;

    private List<SlotPrefab> _bagSlots;    //가방 프리펩 슬롯 리스트

    public void Init(VirtualSlot virtualSlot)
    {
        //가방 인벤토리 슬롯 생성
        _bagSlots = new List<SlotPrefab>();
        //슬롯들 생성
        for(int i = 0; i < capacity; i++)
        {
            _bagSlots.Add(Instantiate(slotPrefab, transform).GetComponent<SlotPrefab>());
            _bagSlots[i].Initialize(i, virtualSlot);
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
        if (item is CountableItem countableItem)
        {
            _bagSlots[index].SetSlot(countableItem._data.Sprite, countableItem._data.Name,
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
            _bagSlots[index].SetSlot(item._data.Sprite, item._data.Name);
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
}
