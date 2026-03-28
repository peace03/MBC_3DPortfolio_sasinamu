using System.Collections.Generic;
using UnityEngine;

public class QuickInventoryView : MonoBehaviour
{
    [Header("퀵슬롯")]
    [SerializeField] private GameObject panel;              //슬롯 배치될 패널
    [SerializeField] private VirtualSlot virtualSlot;       //가상 슬롯(드래그앤드롭에 사용)

    private List<SlotPrefab> _quickSlots;     //퀵슬롯 프리펩 슬롯 리스트
    private int capacity;                   //슬롯 용량

    public void Init(VirtualSlot virtualSlot, int quickCapacity)
    {
        capacity = quickCapacity;
        //가방 인벤토리 슬롯 리스트 생성
        _quickSlots = new List<SlotPrefab>();
        //슬롯들 생성
        for (int i = 0; i < capacity; i++)
        {
            _quickSlots.Add(panel.transform.GetChild(i).GetComponent<SlotPrefab>());
            _quickSlots[i].Initialize(i, SlotType.Quick, virtualSlot);
        }
    }

    public void UpdateQuickSlot_Single(int index, Item item)
    {
        if (item is CountableItem countableItem)
        {
            _quickSlots[index].SetSlot(countableItem._data.Sprite, countableItem._data.Name,
                countableItem.CurAmount.ToString());
            //Debug.Log("카운터블 뷰 업뎃");
        }
        else if (item == null)
        {
            _quickSlots[index].SetSlot();
            //Debug.Log("비어있는 뷰 업뎃");
        }
        else
        {
            //Debug.Log("넌카운터블 뷰 업뎃");
            _quickSlots[index].SetSlot(item._data.Sprite, item._data.Name);
        }
    }
}
