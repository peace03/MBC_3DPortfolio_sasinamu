using System.Collections.Generic;
using UnityEngine;

public class StorageView : MonoBehaviour
{
    [Header("창고")]
    [SerializeField] private GameObject panel;              //슬롯 배치될 패널
    [SerializeField] private GameObject slotPrefab;         //가방 인벤토리에 배치될 슬롯
    [SerializeField] private VirtualSlot virtualSlot;       //가상 슬롯(드래그앤드롭에 사용)

    private List<SlotPrefab> _storageSlots;     //창고 프리펩 슬롯 리스트
    private int capacity;                   //슬롯 용량

    public void Init(VirtualSlot virtualSlot, int storageCapacity)
    {
        capacity = storageCapacity;
        //가방 인벤토리 슬롯 리스트 생성
        _storageSlots = new List<SlotPrefab>();
        //슬롯들 생성
        for (int i = 0; i < capacity; i++)
        {
            _storageSlots.Add(Instantiate(slotPrefab, panel.transform).GetComponent<SlotPrefab>());
            _storageSlots[i].Initialize(i, SlotType.Storage, virtualSlot);
        }
    }

    public void UpdateStorageSlot_Single(int index, Item item)
    {
        if (item is CountableItem countableItem) //적재가능 아이템
        {
            _storageSlots[index].SetSlot(countableItem._data.Sprite, countableItem._data.Name,
                countableItem.CurAmount.ToString());
            //Debug.Log("카운터블 뷰 업뎃");
        }
        else if (item == null) //비어있는 슬롯
        {
            _storageSlots[index].SetSlot();
            //Debug.Log("비어있는 뷰 업뎃");
        }
        else if (item is ConsumableItem consumableItem) //consumable 아이템
        {
            _storageSlots[index].SetSlot(consumableItem._data.Sprite, consumableItem._data.Name,
                consumableItem.CurDurability, consumableItem.MaxDruability);
        }
        else //unCountable 아이템
        {
            //Debug.Log("넌카운터블 뷰 업뎃");
            _storageSlots[index].SetSlot(item._data.Sprite, item._data.Name);
        }
    }
}
