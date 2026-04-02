using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour,
    IGamePauseHandler, IPopupUIClosedHandler, IPlayerDeadHandler, IControlManualHandler, IInventoryHandler,
    IMapHandler, IBoxHandler, IQuickSlotStateHandler, ICraftingHandler, IKeyMakerHandler, IGameEndingHandler
{
    [Header("정보")]
    [SerializeField] private UIType curOpenUIType = UIType.None;        // 현재 열린 UI 종류

    [Header("UI")]
    [SerializeField] private GameObject hudUI;                          // 상시 표시 정보 UI
    [SerializeField] private GameObject controlManualDetailUI;          // 조작 설명 상세 UI
    [SerializeField] private GameObject quickSlotUI;                    // 퀵슬롯 UI
    [SerializeField] private GameObject gamePauseUI;                    // 일시정지 UI
    [SerializeField] private GameObject settingsUI;                     // 설정 UI
    [SerializeField] private GameObject gameQuitUI;                     // 게임 종료 UI
    [SerializeField] private GameObject gameOverUI;                     // 게임 오버 UI
    [SerializeField] private GameObject gameEndingUI;                   // 게임 엔딩 UI
    [SerializeField] private GameObject menuBarUI;                      // 메뉴 바 UI
    [SerializeField] private GameObject boxUI;                          // 상자 UI
    [SerializeField] private GameObject craftingUI;                     // 작업대 UI
    [SerializeField] private GameObject keyMakerUI;                     // 열쇠 가공기 UI

    private Stack<GameObject> openedUIStack = new();                    // 열려있는 UI 스택

    private void OnEnable()
    {
        // 일시정지 이벤트 구독
        Subject<IGamePauseHandler>.Attach(this);
        // 팝업 UI 닫기 이벤트 구독
        Subject<IPopupUIClosedHandler>.Attach(this);
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
        Subject<IQuickSlotStateHandler>.Attach(this);
        // 작업대 이벤트 구독
        Subject<ICraftingHandler>.Attach(this);
        // 열쇠 가공기 이벤트 구독
        Subject<IKeyMakerHandler>.Attach(this);
        // 게임 엔딩 이벤트 구독
        Subject<IGameEndingHandler>.Attach(this);
    }

    private void OnDisable()
    {
        // 일시정지 이벤트 구독 해제
        Subject<IGamePauseHandler>.Detach(this);
        // 팝업 UI 닫기 이벤트 구독 해제
        Subject<IPopupUIClosedHandler>.Detach(this);
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
        Subject<IQuickSlotStateHandler>.Detach(this);
        // 작업대 이벤트 해제
        Subject<ICraftingHandler>.Detach(this);
        // 열쇠 가공기 이벤트 해제
        Subject<IKeyMakerHandler>.Detach(this);
        //게임 엔딩 이벤트 해제
        Subject<IGameEndingHandler>.Detach(this);
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

    // 설정 함수
    public void OnSettings() => OpenUI(UIType.Settings);

    // 게임 종료 함수
    public void OnGameQuit() => OpenUI(UIType.GameQuit);

    // 팝업 UI 닫기 함수
    public void OnClosedPopupUI() => CloseAllUI();

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
        {
            // UI 닫기
            CloseAllUI();
            Subject<ICameraHandler>.Publish(h => h.OnCamera(false));
        }
    }

    // 상자 함수
    public void OnBox(InventoryModel model)
    {
        // 상자 UI 열기
        OpenUI(UIType.Box);
    }

    public void OnQuickSlotState(bool state) => quickSlotUI.SetActive(state);

    //작업대, 열쇠가공기 호출
    public void OnCraftingTable() => OpenUI(UIType.CraftingTable);
    public void OnKeyMaker() => OpenUI(UIType.KeyMaker);

    // 엔딩씬 넘어가는 함수
    public void OnGameEnding()
    {
        // 기존에 UIManager 내부에 있던 UIType.GameEnding 활성화 로직 호출
        OpenUI(UIType.GameEnding);

        // 필요에 따라 게임 플레이 시간을 정지하거나, 마우스 커서를 활성화하는 로직 추가
        Time.timeScale = 0f;
    }

    // UI 열기 함수
    private void OpenUI(UIType type)
    {
        // UI 종류에 따라서
        switch (type)
        {
            // 일시정지라면
            case UIType.GamePause:
                // 게임 시간 정지
                Time.timeScale = 0f;
                // UI 변경
                ChangeUI(gamePauseUI, type);
                // 퀵슬롯 UI 닫기
                OnQuickSlotState(false);
                break;
            // 설정이라면
            case UIType.Settings:
                ChangeUI(settingsUI, type);
                break;
            // 게임 종료라면
            case UIType.GameQuit:
                ChangeUI(gameQuitUI, type);
                break;
            // 게임 오버라면
            case UIType.GameOver:
                Time.timeScale = 0f;
                ChangeUI(gameOverUI, type);
                break;
            case UIType.GameEnding:
                Time.timeScale = 0f;
                ChangeUI(gameEndingUI, type);
                break;
            // 가방이라면
            case UIType.Inventory:
                ChangeUI(menuBarUI, type);
                // 퀵슬롯 UI 열기
                OnQuickSlotState(true);
                // 메뉴 바 UI 업데이트 함수
                menuBarUI.GetComponent<MenuBarUI>().UpdateMenuBarUI(type);
                break;
            // 지도라면
            case UIType.Map:
                ChangeUI(menuBarUI, type);
                OnQuickSlotState(false);
                // 메뉴 바 UI 업데이트 함수
                menuBarUI.GetComponent<MenuBarUI>().UpdateMenuBarUI(type);
                break;
            // 상자라면
            case UIType.Box:
                ChangeUI(menuBarUI, UIType.Inventory);
                OnQuickSlotState(true);
                menuBarUI.GetComponent<MenuBarUI>().UpdateMenuBarUI(UIType.Inventory);
                // 상자 UI 열기
                OpenUI(boxUI, type);
                break;
            case UIType.CraftingTable:
                ChangeUI(menuBarUI, UIType.Inventory); // 필요하다면 메뉴바 연동
                OpenUI(craftingUI, type);
                break;
            case UIType.KeyMaker:
                ChangeUI(menuBarUI, UIType.Inventory);
                OpenUI(keyMakerUI, type);
                break;
        }

        // 상시 표시 정보 UI 닫기
        hudUI.SetActive(false);
        // UI 상태 이벤트 발생
        PublishUIState();
    }

    // UI 변경 함수
    private void ChangeUI(GameObject nextUI, UIType type)
    {
        // 열려있는 UI가 있다면
        if (openedUIStack.Count > 0)
            // 최근 UI 닫기
            openedUIStack.Pop().SetActive(false);

        // 다음 UI 열기
        nextUI.SetActive(true);
        // 현재 UI 종류 변경
        curOpenUIType = type;
        // 열려있는 UI 스택에 추가
        openedUIStack.Push(nextUI);
    }

    // UI 열기 함수
    private void OpenUI(GameObject nextUI, UIType type)
    {
        // 다음 UI 열기
        nextUI.SetActive(true);
        // 현재 UI 종류 변경
        curOpenUIType = type;
        // 열려있는 UI 스택에 추가
        openedUIStack.Push(nextUI);
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
            case UIType.Settings:
            case UIType.GameQuit:
            case UIType.GameOver:
            case UIType.Inventory:
            case UIType.Map:
                // UI 열림 상태 이벤트 발행
                Subject<IUIStateHandler>.Publish(h => h.OnUIState(true));
                break;
        }
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
        }

        // 게임 시간이 멈춰있다면
        if (Time.timeScale == 0)
            // 게임 시간 시작
            Time.timeScale = 1f;

        // 상시 표시 정보 UI 열기
        hudUI.SetActive(true);
        // 퀵슬롯 UI 열기
        OnQuickSlotState(true);
        // 현재 열린 UI 종류 바꾸기
        curOpenUIType = UIType.None;
        // UI 상태 이벤트 발생
        PublishUIState();
    }
}