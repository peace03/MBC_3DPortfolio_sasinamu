using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class BoxInventoryView : MonoBehaviour
{
    [Header("상자")]
    [SerializeField] private GameObject panel;          //슬롯 배치할 패널
    [SerializeField] private VirtualSlot virtualSlot;   //가상 슬롯(드래그앤드롭에 사용)

    private List<SlotPrefab> _boxSlots;     //상자 슬롯 리스트
    private int capacity;                   //슬롯 용량

    public void Init(VirtualSlot virtualSlot, int boxCapacity)
    {
        capacity = boxCapacity;
        //상자 인벤토리 슬롯 리스트 생성
        _boxSlots = new List<SlotPrefab>();
        //슬롯 생성
        for (int i = 0; i < capacity; i++)
        {
            _boxSlots.Add(panel.transform.GetChild(i).GetComponent<SlotPrefab>());
            _boxSlots[i].Initialize(i, SlotType.Box, virtualSlot);
        }
    }

    public void UpdateBoxSlot_Single(int index, Item item)
    {
        if (item is CountableItem countableItem)
        {
            _boxSlots[index].SetSlot(countableItem._data.Sprite, countableItem._data.Name,
                countableItem.CurAmount.ToString());
            //Debug.Log("카운터블 뷰 업뎃");
        }
        else if (item == null)
        {
            _boxSlots[index].SetSlot();
            //Debug.Log("비어있는 뷰 업뎃");
        }
        else
        {
            //Debug.Log("넌카운터블 뷰 업뎃");
            _boxSlots[index].SetSlot(item._data.Sprite, item._data.Name);
        }
    }
}
