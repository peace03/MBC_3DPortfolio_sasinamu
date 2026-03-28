using System.Collections.Generic;
using UnityEngine;

//슬롯 (좌, 우)클릭시 P에 슬롯 위치 저장 인터페이스
public interface ISlotClickHandler
{
    public void OnSlotLeftClick(SlotType slotType, int index);
    public void OnSlotRightClick(SlotType slotType, int index);
}
//슬록 우클릭 패널 제어 인터페이스
public interface ISlotClickRightHandler
{
    public void OnSlotClickRight(Transform transform); //우클릭시 발행
    public void OnAllBtnSetActive(bool setActive); //드랍 버튼 (비)활성
    public void OnUseBtnSetActive(bool setActive);  //사용 버튼 (비)활성
}
//사용하기, 버리기 버튼 누름 인터페이스
public interface IButtonHandler
{
    public void OnUseButtonDown();
    public void OnDropButtenDown();
}

public interface ICusorPointerHandler
{
    public void OnCusorSlotIn(SlotType slotType, int index);
    public void OnCusorSlotExit();
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

//아이템 착용 인터페이스
public interface IEquipWear
{
    public void OnGunSwap(int index1, Item item1, int index2, Item item2); //무기끼리 교환할 때
    public void OnEquipWear(int index, Item item); //장비 한개 장착될 때
}

//아이템 UI 업데이트 인터페이스
public interface ISlotChanged
{
    public void OnUpdateSingleSlot(SlotType slotType, int index);
}