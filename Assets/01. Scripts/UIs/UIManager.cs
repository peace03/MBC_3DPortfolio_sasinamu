using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour,
    IGamePauseHandler, IPlayerDeadHandler, IControlManualHandler, IInventoryHandler, IMapHandler, IBoxHandler,
    IOpenUIByButtonHandler
{
    [Header("정보")]
    [SerializeField] private UIType curOpenUIType = UIType.None;        // 현재 열린 UI 종류

    [Header("UI")]
    #region UI GameObjects
    [SerializeField] private GameObject gamePauseUI;                    // 일시정지 UI
    [SerializeField] private GameObject gameOverUI;                     // 게임 오버 UI
    [SerializeField] private GameObject controlManualAllUI;             // 조작 설명 전체 UI
    [SerializeField] private GameObject controlManualDetailUI;          // 조작 설명 상세 UI
    [SerializeField] private GameObject menuBarUI;                      // 메뉴 바 UI
    [SerializeField] private GameObject inventoryUI;                    // 가방 UI
    [SerializeField] private GameObject statUI;                         // 스탯 UI
    [SerializeField] private GameObject mapUI;                          // 지도 UI
    [SerializeField] private GameObject settingsUI;                     // 설정 UI
    [SerializeField] private GameObject boxUI;                          // 상자 UI
    #endregion

    private Stack<GameObject> openedUIStack = new();                    // 열려있는 UI 스택

    private void OnEnable()
    {
        // 일시정지 이벤트 구독
        Subject<IGamePauseHandler>.Attach(this);
        // 플레이어 죽음 이벤트 구독
        Subject<IPlayerDeadHandler>.Attach(this);
        // 조작 설명 이벤트 구독
        Subject<IControlManualHandler>.Attach(this);
        // 가방 이벤트 구독
        Subject<IInventoryHandler>.Attach(this);
        // 지도 이벤트 구독
        Subject<IMapHandler>.Attach(this);
        // 상자 이벤트 구독
        Subject<IBoxHandler>.Attach(this);
        // UI 열기(버튼) 이벤트 구독
        Subject<IOpenUIByButtonHandler>.Attach(this);
    }

    private void OnDisable()
    {
        // 일시정지 이벤트 구독 해제
        Subject<IGamePauseHandler>.Detach(this);
        // 플레이어 죽음 이벤트 구독 해제
        Subject<IPlayerDeadHandler>.Detach(this);
        // 조작 설명 이벤트 구독 해제
        Subject<IControlManualHandler>.Detach(this);
        // 가방 이벤트 구독 해제
        Subject<IInventoryHandler>.Detach(this);
        // 지도 이벤트 구독 해제
        Subject<IMapHandler>.Detach(this);
        // 상자 이벤트 구독 해제
        Subject<IBoxHandler>.Detach(this);
        // UI 열기(버튼) 이벤트 구독 해제
        Subject<IOpenUIByButtonHandler>.Detach(this);
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

    // 조작 설명 함수
    public void OnControlManual() => controlManualDetailUI.SetActive(!controlManualDetailUI.activeSelf);

    // 가방 함수
    public void OnInventory()
    {
        // UI가 닫힌 상태라면
        if (curOpenUIType == UIType.None)
            // 가방 UI 열기
            OpenUI(UIType.Inventory);
        // 현재 열린 UI가 일시정지와 게임 오버가 아니라면
        else if (curOpenUIType != UIType.GamePause && curOpenUIType != UIType.GameOver)
            // UI 닫기
            CloseAllUI();
    }

    // 지도 함수
    public void OnMap()
    {
        // UI가 닫힌 상태라면
        if (curOpenUIType == UIType.None)
            // 지도 UI 열기
            OpenUI(UIType.Map);
        // 현재 열린 UI가 지도이라면
        else if (curOpenUIType == UIType.Map)
            // UI 닫기
            CloseAllUI();
    }

    // 버튼으로 열리는 UI 함수
    public void OnOpenUIByButton(UIType type)
    {
        // 열려있던 UI 닫기
        openedUIStack.Pop().SetActive(false);

        // UI 종류에 따라서
        switch (type)
        {
            // 가방이라면
            case UIType.Inventory:
                // 가방 UI 열기
                OpenInventoryUI();
                break;
            // 스탯이라면
            case UIType.Stat:
                // 스탯 UI 열기
                statUI.SetActive(true);
                curOpenUIType = UIType.Stat;
                openedUIStack.Push(statUI);
                break;
            // 지도라면
            case UIType.Map:
                // 지도 UI 열기
                OpenMapUI();
                break;
            // 설정이라면
            case UIType.Settings:
                // 설정 UI
                statUI.SetActive(true);
                curOpenUIType = UIType.Stat;
                openedUIStack.Push(statUI);
                break;
        }
    }

    // 상자 함수
    public void OnBox(InventoryModel model)
    {
        // 아이템들의 수만큼
        //foreach (var item in items)
        //    Debug.Log($"[Item] {item.name}");

        // 상자 UI 열기
        OpenUI(UIType.Box);
    }

    // UI 열기 함수
    private void OpenUI(UIType type)
    {
        // UI 종류에 따라서
        switch (type)
        {
            // 일시정지라면
            case UIType.GamePause:
                // 일시정지 UI 열기
                gamePauseUI.SetActive(true);
                // 게임 시간 정지
                Time.timeScale = 0f;
                // 현재 열린 UI 종류 바꾸기
                curOpenUIType = UIType.GamePause;
                // 열려있는 UI 스택에 추가
                openedUIStack.Push(gamePauseUI);
                break;
            // 게임 오버라면
            case UIType.GameOver:
                // 게임 오버 UI 열기
                gameOverUI.SetActive(true);
                Time.timeScale = 0f;
                curOpenUIType = UIType.GameOver;
                openedUIStack.Push(gameOverUI);
                break;
            // 가방이라면
            case UIType.Inventory:
                // 메뉴 바 UI 열기
                OpenMenuBarUI(UIType.Inventory);
                // 가방 UI 열기
                OpenInventoryUI();
                break;
            // 지도라면
            case UIType.Map:
                OpenMenuBarUI(UIType.Map);
                // 지도 UI 열기
                OpenMapUI();
                break;
            // 상자라면
            case UIType.Box:
                // 상자 UI 열기
                boxUI.SetActive(true);
                openedUIStack.Push(boxUI);
                OpenMenuBarUI(UIType.Inventory);
                OpenInventoryUI();
                break;
        }

        // 조작 설명 UI 닫기
        controlManualAllUI.SetActive(false);
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
            case UIType.Inventory:
            case UIType.Map:
            case UIType.Box:
                // UI 열림 상태 이벤트 발행
                Subject<IUIStateHandler>.Publish(h => h.OnUIState(true));
                break;
        }
    }

    // 메뉴 바 UI 열기 함수
    private void OpenMenuBarUI(UIType type)
    {
        // 메뉴 바 UI 열기
        menuBarUI.SetActive(true);
        // 열려있는 UI 스택에 추가
        openedUIStack.Push(menuBarUI);
        // 메뉴 바 버튼들 갱신
        menuBarUI.GetComponent<MenuBarUI>().UpdateMenuButtons(type);
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

    // 지도 UI 열기 함수
    private void OpenMapUI()
    {
        // 지도 UI 열기
        mapUI.SetActive(true);
        // 현재 열린 UI 종류 바꾸기
        curOpenUIType = UIType.Map;
        // 열려있는 UI에 추가
        openedUIStack.Push(mapUI);
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

        // 게임 시간이 멈춰있다면
        if (Time.timeScale == 0)
            // 게임 시간 시작
            Time.timeScale = 1f;

        // 조작 설명 UI 열기
        controlManualAllUI.SetActive(true);
        // 현재 열린 UI 종류 바꾸기
        curOpenUIType = UIType.None;
        // UI 상태 이벤트 발생
        PublishUIState();
    }
}