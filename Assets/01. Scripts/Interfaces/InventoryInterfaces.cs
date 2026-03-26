using System.Collections.Generic;
using UnityEngine;

//슬롯 (좌, 우)클릭 인터페이스
public interface ISlotClickHandler
{
    public void OnSlotLeftClick(SlotType slotType, int index);
    public void OnSlotRightClick(SlotType slotType, int index);
}
//슬록 우클릭 인터페이스
public interface ISlotClickRightHandler
{
    public void OnSlotClickRight(Transform transform);
}
//Drop 버튼 누름 인터페이스
public interface IDropButtonHandler
{
    public void OnDropButtenDown();
}


// 아이템 사용 인터페이스
public interface IUseItemHandler
{
    public void OnUseCureItem(float cureAmount);
    public void OnUseFoodItem(float energe, float thirst);
}

//아이템 교환 인터페이스
public interface ISlotExchangeHandler
{
    public void OnExchangeSlot(SlotType fromSlotType, int fromIndex,
        SlotType toSlotType, int toIndex);
}

//아이템 UI 업데이트 인터페이스
public interface ISlotChanged
{
    public void OnUpdateSingleSlot(SlotType slotType, int index);
}