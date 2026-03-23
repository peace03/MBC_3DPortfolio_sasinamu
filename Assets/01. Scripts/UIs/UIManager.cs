using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IGamePauseHandler, IPlayerDeadHandler, IBoxHandler, IInventoryHandler
{
    [Header("정보")]
    [SerializeField] private UIType curOpenUIType = UIType.None;        // 현재 열린 UI 종류

    [Header("UI")]
    [SerializeField] private GameObject gamePauseUI;                    // 일시정지 UI
    [SerializeField] private GameOverUI gameOverUI;                     // 게임 오버 UI
    [SerializeField] private GameObject boxUI;                          // 상자 UI
    [SerializeField] private GameObject inventoryUI;                    // 가방 UI
    [SerializeField] private MapUI mapUI;                               // 맵 UI

    private Stack<GameObject> openedUIStack = new();                    // 열려있는 UI 스택

    private void OnEnable()
    {
        // 일시정지 이벤트 구독
        Subject<IGamePauseHandler>.Attach(this);
        // 플레이어 죽음 이벤트 구독
        Subject<IPlayerDeadHandler>.Attach(this);
        // 상자 이벤트 구독
        Subject<IBoxHandler>.Attach(this);
        // 가방 이벤트 구독
        Subject<IInventoryHandler>.Attach(this);
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
        // 가방 이벤트 구독 해제
        Subject<IInventoryHandler>.Detach(this);
        //// 맵 이벤트 구독 해제
        //EventBus<MapEvent>.OnEvent -= HandleMapUI;
    }

    // 일시정지 함수
    public void OnGamePause()
    {
        // UI가 닫힌 상태라면
        if (curOpenUIType == UIType.None)
            // 일시정지 UI 열기
            OpenUI(UIType.GamePause);
        // UI가 열린 상태라면
        else
            // UI 닫기
            CloseAllUI();

        // 적 멈춤 상태 이벤트 발행
        Subject<IEnemyPauseHandler>.Publish(h => h.OnEnemyPause(curOpenUIType != UIType.None));
    }

    // 플레이어 죽음 함수
    public void OnPlayerDead(string killer)
    {
        // UI가 열린 상태라면
        if (curOpenUIType != UIType.None)
            // UI 닫기
            CloseAllUI();

        // 게임오버 UI 꾸미기(플레이어를 죽인 원인)
        Debug.Log($"사망 원인 : [{killer}]");
        // 게임오버 UI 열기
        OpenUI(UIType.GameOver);
        // 적 멈춤 상태 이벤트 발행
        Subject<IEnemyPauseHandler>.Publish(h => h.OnEnemyPause(true));
    }

    // 상자 함수
    public void OnBox(List<GameObject> items)
    {
        // 아이템들의 수만큼
        foreach (var item in items)
            Debug.Log($"[Item] {item.name}");

        // 상자 UI 열기
        OpenUI(UIType.Box);
    }

    // 가방 함수
    public void OnInventory()
    {
        // UI가 닫힌 상태라면
        if (curOpenUIType == UIType.None)
            // 가방 UI 열기
            OpenUI(UIType.Inventory);
        // 현재 열린 UI가 일시정지가 아니라면
        else if (curOpenUIType != UIType.GamePause)
            // UI 닫기
            CloseAllUI();
    }

    // UI 열기 함수
    private void OpenUI(UIType type)
    {
        // UI 종류에 따라서
        switch (type)
        {
            // 일시정지라면
            case UIType.GamePause:
                // UI 열기
                gamePauseUI.SetActive(true);
                // 현재 열린 UI 종류 바꾸기
                curOpenUIType = UIType.GamePause;
                // 열려있는 UI 스택에 추가
                openedUIStack.Push(gamePauseUI);
                break;
            // 게임 오버라면
            case UIType.GameOver:
                // UI 열기
                gameOverUI.gameObject.SetActive(true);
                // 현재 열린 UI 종류 바꾸기
                curOpenUIType = UIType.GameOver;
                // 열려있는 UI 스택에 추가
                openedUIStack.Push(gameOverUI.gameObject);
                break;
            // 상자라면
            case UIType.Box:
                // 상자 UI 열기
                boxUI.SetActive(true);
                // 열려있는 UI 스택에 추가
                openedUIStack.Push(boxUI);
                // 가방 UI 열기
                OpenInventoryUI();
                break;
            // 가방이라면
            case UIType.Inventory:
                // 가방 UI 열기
                OpenInventoryUI();
                break;
        }

        // UI 상태 이벤트 발생
        PublishUIState();
    }

    // 가방 UI 열기 함수
    private void OpenInventoryUI()
    {
        // 가방 UI 열기
        inventoryUI.SetActive(true);
        // 현재 열린 UI 종류 바꾸기
        curOpenUIType = UIType.Inventory;
        // 열려있는 UI 스택에 추가
        openedUIStack.Push(inventoryUI);
    }

    // 모든 UI 닫기 함수
    private void CloseAllUI()
    {
        // 열려있는 UI의 개수만큼
        while (openedUIStack.Count > 0)
        {
            // 열려있는 UI 받아오기
            var openUI = openedUIStack.Pop();
            // UI 닫기
            openUI.SetActive(false);
            Debug.Log($"{openUI.name} 닫힘");
        }

        // 현재 열린 UI 종류 바꾸기
        curOpenUIType = UIType.None;
        // UI 상태 이벤트 발생
        PublishUIState();
    }

    // UI 열림 상태 이벤트 발행 함수
    private void PublishUIState()
    {
        // 현재 열린 UI 종류에 따라서
        switch (curOpenUIType)
        {
            // UI가 닫힌 상태라면
            case UIType.None:
                // UI 닫힘 상태 이벤트 발행
                Subject<IUIStateHandler>.Publish(h => h.OnUIState(false));
                break;
            // UI가 열린 상태라면
            case UIType.GamePause:
            case UIType.GameOver:
            case UIType.Box:
            case UIType.Inventory:
                // UI 열림 상태 이벤트 발행
                Subject<IUIStateHandler>.Publish(h => h.OnUIState(true));
                break;
        }
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