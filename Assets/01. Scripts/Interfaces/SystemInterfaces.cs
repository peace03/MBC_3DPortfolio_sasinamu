using System.Collections.Generic;
using UnityEngine;

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
    public void Damaged(float amount);
}

// 적 죽음 관리 인터페이스
public interface IEnemyDeadHandler
{
    public void OnEnemyDead(IDamageable enemy, GameObject killer);
}

// 일시정지 관리 인터페이스
public interface IGamePauseHandler
{
    public void OnPause();
}

// UI 상태 관리 인터페이스
public interface IUIStateHandler
{
    public void OnUIState(bool state);
}

// 상자 관리 인터페이스
public interface IBoxHandler
{
    public void OnBox(List<GameObject> items);
}

// 가방 관리 인터페이스
// 지도 관리 인터페이스
// 조작설명 관리 인터페이스