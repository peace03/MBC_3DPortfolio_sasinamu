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
    // 객체 구분을 위한 레이어 번호(플레이어/적)
    public int LayerNumber { get; }
    // HP 비율
    public float HpRatio { get; }
    // 데미지 반영 함수
    public void Damaged(float amount);
}

// 마우스 위치 관리 인터페이스
public interface IMouseMoveHandler
{
    public void OnMouseMove(Vector2 pos);
}

// 공격, 피격 이벤트 관리 인터페이스
public interface IDamageEventHandler
{
    public void OnDamage(IDamageable target, float amount, float hpRatio);
}

// 플레이어 죽음 관리 인터페이스
public interface IPlayerDeadHandler
{
    public void OnPlayerDead(GameObject killer);
}

// 적 죽음 관리 인터페이스
public interface IEnemyDeadHandler
{
    public void OnEnemyDead(IDamageable enemy, GameObject killer);
}

// UI 상태 관리 인터페이스
public interface IUIStateHandler
{
    public void OnOpenUI(UIType type);
    public void OnCloseUI(UIType type);
}

// 상자 UI 관리 인터페이스
public interface IBoxDataHandler
{
    public void OnBoxOpened();
    public void OnBoxClosed();
}

// 일시정지 관리 인터페이스
//public interface IGamePauseHandler
//{
//    public void OnPause();
//}
// 취소 관리 인터페이스
// 가방 관리 인터페이스
// 지도 관리 인터페이스
// 조작설명 관리 인터페이스