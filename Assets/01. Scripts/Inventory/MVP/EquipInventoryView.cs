using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipInventoryView : MonoBehaviour
{
    [Header("장비 슬롯")]
    [SerializeField] private List<EquipmentSlot> _equipSlots;   //장비 슬롯 리스트
    [SerializeField] private ItemStatPanel itemStatPanel;   //아이템 스탯 패널

    public void Init(VirtualSlot virtualSlot)
    {
        //장비 인벤토리 슬롯 생성
        _equipSlots = new List<EquipmentSlot>();
        //슬롯들 추가
        for (int i = 0; i < 5; i++)
        {
            _equipSlots.Add(transform.GetChild(i).GetComponent<EquipmentSlot>());
            _equipSlots[i].Initialize(i, virtualSlot);
        }
    }

    //장비 인벤토리 슬롯 한개만 업데이트
    public void UpdateEquipSlot_Single(int index, Item item)
    {
        if (item == null) _equipSlots[index].SetSlot();
        else _equipSlots[index].SetSlot(item._data.Sprite, item._data.Name);
    }
}
