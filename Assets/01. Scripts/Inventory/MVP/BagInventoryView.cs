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

    private int maxAllocatedSlots = 16; // 💡 확정적 오버할당할 최대 슬롯 개수 (10 + 6)

    public void Init(VirtualSlot virtualSlot, int currentCapacity)
    {
        capacity = currentCapacity;
        //가방 인벤토리 슬롯 리스트 생성
        _bagSlots = new List<SlotPrefab>();

        //// 1. 최대치인 19개의 프리팹을 게임 시작과 동시에 미리 메모리에 모두 올려둡니다. (Instantiate)
        for (int i = 0; i < maxAllocatedSlots; i++)
        {
            _bagSlots.Add(Instantiate(slotPrefab, transform).GetComponent<SlotPrefab>());
            _bagSlots[i].Initialize(i, SlotType.Bag, virtualSlot);

            // 2. 현재 모델의 용량(capacity) 범위를 벗어나는 인덱스의 슬롯은 꺼둡니다.
            _bagSlots[i].gameObject.SetActive(i < capacity);
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

    // Presenter의 지시에 따라 미리 만들어둔 UI 슬롯을 켜고 끄기만 합니다. (Instantiate 발생 안함 -> 렉 0%)
    public void UpdateCapacityUI(int newCapacity)
    {
        capacity = newCapacity;
        for (int i = 0; i < _bagSlots.Count; i++)
        {
            // 자신의 인덱스가 새로운 용량보다 작으면 활성화, 크거나 같으면 비활성화
            _bagSlots[i].gameObject.SetActive(i < capacity);
        }
    }
}
