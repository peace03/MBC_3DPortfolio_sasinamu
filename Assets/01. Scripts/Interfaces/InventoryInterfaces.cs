using System.Collections.Generic;
using UnityEngine;

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