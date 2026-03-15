using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private bool isOpenUI;                 // UI 활성화 여부

    [Header("UI")]
    [SerializeField] private GameOverUI gameOverUI;         // 게임 오버 UI
    [SerializeField] private MapUI mapUI;                   // 맵 UI

    private Stack<GameObject> openedUIStack = new();        // 열려있는 UI 스택

    private void OnEnable()
    {
        // 플레이어 죽음 이벤트 구독
        EventBus<DeadEvent>.OnEvent += HandlePlayerDeadUI;
        // 상자 이벤트 구독
        EventBus<BoxEvent>.OnEvent += OpenUI;
        // 취소 이벤트 구독
        EventBus<CancelEvent>.OnEvent += CancelEvent;
        //EventBus<InventoryEvent>.OnEvent
        // 맵 이벤트 구독
        EventBus<MapEvent>.OnEvent += HandleMapUI;
    }

    private void OnDisable()
    {
        // 플레이어 죽음 이벤트 구독 해제
        EventBus<DeadEvent>.OnEvent -= HandlePlayerDeadUI;
        // 상자 이벤트 구독 해제
        EventBus<BoxEvent>.OnEvent -= OpenUI;
        // 취소 이벤트 구독 해제
        EventBus<CancelEvent>.OnEvent -= CancelEvent;
        // 맵 이벤트 구독 해제
        EventBus<MapEvent>.OnEvent -= HandleMapUI;
    }

    // 플레이어 죽음 UI 관리 함수
    private void HandlePlayerDeadUI(DeadEvent data)
    {
        // 플레이어가 죽은 거라면
        if (data.isPlayer)
        {
            // UI 열린 상태
            isOpenUI = true;
            // 게임 오버 UI 켜기
            gameOverUI.gameObject.SetActive(true);
        }
    }

    // UI 열림 함수
    private void OpenUI(BoxEvent data)
    {
        Debug.Log($"{data.boxName} UI 열림");
        // UI 열린 상태
        isOpenUI = true;
    }

    // ESC 눌림 함수
    private void HandleCancel(CancelEvent data)
    {
        // UI가 열려있는 상태라면
        if (isOpenUI)
        {
            // UI 닫힌 상태
            isOpenUI = false;
            // 모든 UI 닫기
            CloseAllUI();
            // 종료
            return;
        }

        // UI 열린 상태
        isOpenUI = true;
        // 일시 정지 UI 열기
        Debug.Log("일시정지 UI 열림");

    }

    // 맵 UI 관리 함수
    private void HandleMapUI(MapEvent data)
    {
        // UI가 열려있는 상태라면
        if (isOpenUI)
        {
            // 모든 UI 닫기
            CloseAllUI();
            // 종료
            return;
        }

        // UI 열린 상태
        isOpenUI = true;
        // UI 활성화 이벤트 발생
        EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
        // 맵 UI 활성화
        mapUI.gameObject.SetActive(true);
        // 열려있는 UI 스택에 추가
        openedUIStack.Push(mapUI.gameObject);
    }

    // 모든 UI를 닫는 함수
    private void CloseAllUI()
    {
        // 열려있는 UI의 개수만큼
        while (openedUIStack.Count > 0)
            // UI 비활성화
            openedUIStack.Pop().SetActive(false);

        // UI 닫힌 상태
        isOpenUI = false;
        // UI 비활성화 이벤트 발생
        EventBus<UIStateEvent>.Publish(new UIStateEvent(false));
    }
}