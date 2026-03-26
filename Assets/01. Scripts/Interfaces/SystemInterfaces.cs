using System.Collections.Generic;
using UnityEngine;

// 팝업 UI 닫기 관리 인터페이스
public interface IPopupUIClosedHandler
{
    public void OnClosedPopupUI();
}

// 상호작용이 가능한 대상 인터페이스
public interface IInteractable
{
    // 상호작용 함수
    public void Interact();
}

// 데미지 입을 수 있는 대상 인터페이스
public interface IDamageable
{
    // 데미지 반영 함수
    public void Damaged(string name, float amount);
}

// 적 죽음 관리 인터페이스
public interface IEnemyDeadHandler
{
    public void OnEnemyDead(Vector3 position);
}

// UI 상태 관리 인터페이스
public interface IUIStateHandler
{
    public void OnUIState(bool state);
}

// 적 일시정지 관리 인터페이스
public interface IEnemyPauseHandler
{
    public void OnEnemyPause(bool state);
}

// 일시정지 관리 인터페이스
public interface IGamePauseHandler
{
    public void OnGamePause();
}

// 조작설명 관리 인터페이스
public interface IControlManualHandler
{
    public void OnControlManual();
}

// 상자 관리 인터페이스
public interface IBoxHandler
{
    public void OnBox(List<GameObject> items);
}

// 가방 관리 인터페이스
public interface IInventoryHandler
{
    public void OnInventory();
}

// 지도 관리 인터페이스
public interface IMapHandler
{
    public void OnMap();
}

// 아이템 사용 관리 인터페이스
public interface T_IUseItemHandler
{
    public void OnUseCureItem(float cureAmount);
    public void OnUseFoodItem(float hunger, float thirst);
}