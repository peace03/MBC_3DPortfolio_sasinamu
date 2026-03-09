using UnityEngine;

/* UI 테스트를 위한 매니저 */
public class T_UIManager : MonoBehaviour
{
    [SerializeField] private bool isOpenUI;     // UI 활성화 여부

    private void OnEnable()
    {
        // 전리품 상자 이벤트 구독
        EventBus<LootBoxEvent>.OnEvent += T_OpenUI;
        // ESC 이벤트 구독
        EventBus<EscEvent>.OnEvent += T_ESCEvent;
        // 플레이어 죽음 이벤트 구독
        EventBus<DeadEvent>.OnEvent += T_PlayerDead;
    }

    private void OnDisable()
    {
        // 전리품 상자 이벤트 구독 해제
        EventBus<LootBoxEvent>.OnEvent -= T_OpenUI;
        // ESC 이벤트 구독 해제
        EventBus<EscEvent>.OnEvent -= T_ESCEvent;
        // 플레이어 죽음 이벤트 구독 해제
        EventBus<DeadEvent>.OnEvent -= T_PlayerDead;
    }

    private void T_OpenUI(LootBoxEvent data)
    {
        Debug.Log($"{data.boxName} UI 열림");
        // UI 열린 상태
        isOpenUI = true;
    }

    private void T_ESCEvent(EscEvent data)
    {
        if (isOpenUI)
        {
            Debug.Log("UI 닫힘");
            // UI 닫힌 상태
            isOpenUI = false;
            // UI 비활성화 이벤트 발생
            EventBus<UIStateEvent>.Publish(new UIStateEvent(false));
        }
        else
        {
            Debug.Log("일시정지 UI 열림");
            // UI 열린 상태
            isOpenUI = true;
            // UI 활성화 이벤트 발생
            EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
        }
    }

    private void T_PlayerDead(DeadEvent data)
    {
        // 플레이어가 죽은 거라면
        if(data.isPlayer)
            Debug.Log("플레이어 죽음");
    }
}