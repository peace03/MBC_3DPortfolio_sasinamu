using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour,
    IGamePauseHandler, IPlayerDeadHandler, IControlManualHandler, IInventoryHandler, IBoxHandler, IMapHandler
{
    [Header("정보")]
    [SerializeField] private UIType curOpenUIType = UIType.None;        // 현재 열린 UI 종류
    [SerializeField] private float startOpenInvTime;                    // 가방 열기 시작 시간

    [Header("UI")]
    [SerializeField] private GameObject gamePauseUI;                    // 일시정지 UI
    [SerializeField] private GameObject gameOverUI;                     // 게임 오버 UI
    [SerializeField] private GameObject controlManualUI;                // 조작 설명 UI
    [SerializeField] private GameObject inventoryUI;                    // 가방 UI
    [SerializeField] private GameObject boxUI;                          // 상자 UI
    [SerializeField] private GameObject mapUI;                          // 지도 UI

    [Header("스탯")]
    [SerializeField] private float openInvDelayTime;                    // 가방 열기 지연 시간

    private Stack<GameObject> openedUIStack = new();                    // 열려있는 UI 스택

    private Coroutine openInvCoroutine;                                 // 가방 열기 코루틴

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
        // 상자 이벤트 구독
        Subject<IBoxHandler>.Attach(this);
        // 지도 이벤트 구독
        Subject<IMapHandler>.Attach(this);
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
        // 상자 이벤트 구독 해제
        Subject<IBoxHandler>.Detach(this);
        // 지도 이벤트 구독 해제
        Subject<IMapHandler>.Detach(this);
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
    public void OnControlManual() => controlManualUI.SetActive(!controlManualUI.activeSelf);

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

    // 상자 함수
    public void OnBox(List<GameObject> items)
    {
        // 아이템들의 수만큼
        foreach (var item in items)
            Debug.Log($"[Item] {item.name}");

        // 상자 UI 열기
        OpenUI(UIType.Box);
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
                gameOverUI.SetActive(true);
                // 현재 열린 UI 종류 바꾸기
                curOpenUIType = UIType.GameOver;
                // 열려있는 UI 스택에 추가
                openedUIStack.Push(gameOverUI);
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
            // 스탯이라면
            case UIType.Stat:
                Debug.Log("스탯");
                break;
            // 지도라면
            case UIType.Map:
                Debug.Log("지도");
                break;
            // 설정이라면
            case UIType.Settings:
                Debug.Log("설정");
                break;
        }

        // UI 상태 이벤트 발생
        PublishUIState();
    }

    // 가방 UI 열기 함수
    private void OpenInventoryUI()
    {
        // 가방 열기 코루틴이 비어있지 않다면
        if (openInvCoroutine != null)
            // 종료
            return;

        // 가방 열기 시작 시간 설정
        startOpenInvTime = Time.time;
        // 가방 열기 코루틴 시작
        openInvCoroutine = StartCoroutine(OpenInventoryCoroutine());
    }

    // 가방 열기 코루틴
    private IEnumerator OpenInventoryCoroutine()
    {
        // 가방 열기 시작 시간에서 가방 열기 지연 시간이 지나지 않았을 때
        while (Time.time - startOpenInvTime <= openInvDelayTime)
        {
            Debug.Log($"진행 시간 : {(Time.time - startOpenInvTime):F1}초");
            // 여기서 슬라이더에 값 보내주기

            // 프레임 단위로 기다리기
            yield return null;
        }

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
            case UIType.Stat:
            case UIType.Map:
            case UIType.Settings:
                // UI 열림 상태 이벤트 발행
                Subject<IUIStateHandler>.Publish(h => h.OnUIState(true));
                break;
        }
    }
}