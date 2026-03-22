using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IGamePauseHandler, IPlayerDeadHandler, IBoxHandler
{
    [Header("정보")]
    [SerializeField] private bool isOpenUI;                 // UI 활성화 여부

    [Header("UI")]
    [SerializeField] private GameOverUI gameOverUI;         // 게임 오버 UI
    [SerializeField] private GameObject boxUI;              // 상자 UI
    [SerializeField] private MapUI mapUI;                   // 맵 UI

    private Stack<GameObject> openedUIStack = new();        // 열려있는 UI 스택

    private void OnEnable()
    {
        // 일시정지 이벤트 구독
        Subject<IGamePauseHandler>.Attach(this);
        // 플레이어 죽음 이벤트 구독
        Subject<IPlayerDeadHandler>.Attach(this);
        // 상자 이벤트 구독
        Subject<IBoxHandler>.Attach(this);
        //// 맵 이벤트 구독
        //EventBus<MapEvent>.OnEvent += HandleMapUI;
    }

    private void OnDisable()
    {
        // 일시정지 이벤트 구독 해제
        Subject<IGamePauseHandler>.Detach(this);
        // 플레이어 죽음 이벤트 구독 해제
        Subject<IPlayerDeadHandler>.Detach(this);
        // 상자 이벤트 구독 해제
        Subject<IBoxHandler>.Detach(this);
        //// 맵 이벤트 구독 해제
        //EventBus<MapEvent>.OnEvent -= HandleMapUI;
    }

    // 일시정지 함수
    public void OnPause()
    {
        // UI가 닫힌 상태라면
        if (!isOpenUI)
            // 일시정지 UI 활성화
            OpenUI(UIType.Pause);
        // UI가 열린 상태라면
        else
            // UI 닫기
            CloseAllUI();

        // UI 상태 이벤트 발생
        Subject<IUIStateHandler>.Publish(h => h.OnUIState(isOpenUI));
    }

    // 플레이어 죽음 함수
    public void OnPlayerDead(GameObject killer)
    {
        // 게임오버 UI 꾸미기(플레이어를 죽인 원인)
        Debug.Log($"사망 원인 : [{killer.name}]");
        // 게임오버 UI 활성화
        OpenUI(UIType.GameOver);
    }

    // 상자 함수
    public void OnBox(List<GameObject> items)
    {
        // 아이템들의 수만큼
        foreach (var item in items)
            Debug.Log($"[Item] {item.name}");

        // 상자 UI 활성화
        OpenUI(UIType.Box);
    }

    // UI 활성화 함수
    private void OpenUI(UIType type)
    {
        // UI 종류에 따라서
        switch (type)
        {
            // 일시정지라면
            case UIType.Pause:
                Debug.Log("일시정지 UI 열림");
                break;
            // 게임 오버라면
            case UIType.GameOver:
                // UI 활성화
                gameOverUI.gameObject.SetActive(true);
                break;
            // 상자라면
            case UIType.Box:
                Debug.Log("상자 UI 열림");
                break;
        }

        // UI 열린 상태
        isOpenUI = true;
    }

    // 모든 UI 비활성화 함수
    private void CloseAllUI()
    {
        // 열려있는 UI의 개수만큼
        while (openedUIStack.Count > 0)
        {
            // 열려있는 UI 받아오기
            var openUI = openedUIStack.Pop();
            // UI 비활성화
            openUI.SetActive(false);
            Debug.Log($"{openUI.name} 닫힘");
        }

        // UI 닫힌 상태
        isOpenUI = false;
    }

    // 맵 UI 관리 함수
    //private void HandleMapUI(MapEvent data)
    //{
    //    // UI가 열려있는 상태라면
    //    if (isOpenUI)
    //    {
    //        // 모든 UI 닫기
    //        CloseAllUI();
    //        // 종료
    //        return;
    //    }

    //    // UI 열린 상태
    //    isOpenUI = true;
    //    // UI 활성화 이벤트 발생
    //    EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
    //    // 맵 UI 활성화
    //    mapUI.gameObject.SetActive(true);
    //    // 열려있는 UI 스택에 추가
    //    openedUIStack.Push(mapUI.gameObject);
    //}
}