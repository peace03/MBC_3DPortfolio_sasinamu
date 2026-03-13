using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private bool canInput;         // 조작 가능 여부

    private List<InputAction> allActionsList;       // 모든 액션들 리스트

    /* 액션 맵 - 플레이어 */
    private DuckovInputActions inputActions;        // 인풋 시스템
    private InputAction moveAction;                 // 이동 액션(WASD)
    private InputAction runAction;                  // 달리기 액션(Shift)
    private InputAction rollAction;                 // 구르기 액션(Space)
    private InputAction interactAction;             // 상호작용 액션(E)
    private InputAction cancelAction;               // 취소 액션(ESC)
    private InputAction fireAction;                 // 사격 액션
    private InputAction fireModeAction;             // 사격 모드 변경 액션
    private InputAction reloadAction;               // 장전 액션
    private InputAction inventoryAction;            // 가방 액션
    private InputAction mapAction;                  // 지도 액션
    private InputAction showControlsAction;         // 조작 설명 액션

    /* 액션 맵 - 카메라 */
    private InputAction mousePosAction;             // 마우스 위치 액션

    private Vector2 curMousePos;                    // 현재 마우스 위치
    public Vector2 CurMousePos => curMousePos;

    private void Awake()
    {
        // 초기화
        canInput = true;
        allActionsList = new List<InputAction>();
        inputActions = new DuckovInputActions();
        // 이동
        moveAction = inputActions.Player.Move;
        allActionsList.Add(moveAction);
        // 달리기
        runAction = inputActions.Player.Run;
        allActionsList.Add(runAction);
        // 구르기
        rollAction = inputActions.Player.Roll;
        allActionsList.Add(rollAction);
        // 상호작용
        interactAction = inputActions.Player.Interact;
        allActionsList.Add(interactAction);
        // ESC
        cancelAction = inputActions.Player.Cancel;
        allActionsList.Add(cancelAction);
        // 사격
        fireAction = inputActions.Player.Fire;
        allActionsList.Add(fireAction);
        // 사격 모드 변경
        fireModeAction = inputActions.Player.FireMode;
        allActionsList.Add(fireModeAction);
        // 장전
        reloadAction = inputActions.Player.Reload;
        allActionsList.Add(reloadAction);
        // 가방
        inventoryAction = inputActions.Player.Inventory;
        allActionsList.Add(inventoryAction);
        // 지도
        mapAction = inputActions.Player.Map;
        allActionsList.Add(mapAction);
        // 조작 설명
        showControlsAction = inputActions.Player.ShowControls;
        allActionsList.Add(showControlsAction);
        // 마우스 위치
        mousePosAction = inputActions.Camera.MousePosition;
    }

    private void OnEnable()
    {
        // 입력 이벤트 가능
        inputActions.Player.Enable();
        inputActions.Camera.Enable();

        // 액션 리스트의 수만큼
        foreach (var action in allActionsList)
        {
            // 키 입력이 시작됐을 때
            action.started += PublishInputActions;
            // 키 입력이 지속될 때
            action.performed += PublishInputActions;
            // 키 입력이 끝났을 때
            action.canceled += PublishInputActions;
        }

        // UI 상태 이벤트 구독
        EventBus<UIStateEvent>.OnEvent += SetCanInput;
    }

    private void Update()
    {
        // 조작이 불가능한 상태라면
        if (!canInput)
            // 종료
            return;

        // 현재 마우스 위치 저장
        curMousePos = mousePosAction.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        // UI 상태 이벤트 구독 해제
        EventBus<UIStateEvent>.OnEvent -= SetCanInput;

        // 액션 리스트의 수만큼
        foreach (var action in allActionsList)
        {
            // 입력 값이 0에서 벗어났을 때
            action.started -= PublishInputActions;
            // 입력 값이 유효한 값으로 확정되거나 변했다면
            action.performed -= PublishInputActions;
            // 입력 값이 0이 됐을 때
            action.canceled -= PublishInputActions;
        }

        // 입력 이벤트 불가능
        inputActions.Player.Disable();
        inputActions.Camera.Disable();
    }

    // 조작 가능 여부 설정하는 함수
    private void SetCanInput(UIStateEvent data) => canInput = !data.isOpenUI;

    // 입력 액션들에 대한 이벤트 발행 함수
    public void PublishInputActions(InputAction.CallbackContext context)
    {
        // 입력된 액션이 취소 액션일 때
        if (context.action == cancelAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                EventBus<CancelEvent>.Publish(new CancelEvent());

            // 종료
            return;
        }

        // 조작이 불가능한 상태라면
        if (!canInput)
            // 종료
            return;

        // 입력된 액션이 이동 액션일 때
        if (context.action == moveAction)
            // 이벤트 발생
            EventBus<MoveEvent>.Publish(new MoveEvent(context.ReadValue<Vector2>()));
        // 입력된 액션이 달리기 액션일 때
        else if (context.action == runAction)
            // 이벤트 발생
            EventBus<RunEvent>.Publish(new RunEvent(context.ReadValueAsButton()));
        // 입력된 액션이 구르기 액션일 때
        else if (context.action == rollAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                EventBus<RollEvent>.Publish(new RollEvent());
        }
        // 입력된 액션이 상호작용 액션일 때
        else if (context.action == interactAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                EventBus<InteractEvent>.Publish(new InteractEvent());
        }
        // 입력된 액션이 사격 액션일 때
        else if (context.action == fireAction)
            // 이벤트 발생
            EventBus<FireEvent>.Publish(new FireEvent(context.ReadValueAsButton()));
        // 입력된 액션이 사격 모드 변경 액션일 때
        else if (context.action == fireModeAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                EventBus<FireModeEvent>.Publish(new FireModeEvent());
        }
        // 입력된 액션이 장전 액션일 때
        else if (context.action == reloadAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                EventBus<ReloadEvent>.Publish(new ReloadEvent());
        }
        // 입력된 액션이 가방 액션일 때
        else if (context.action == inventoryAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
            {
                // UI 상태 이벤트 발생
                EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
                // 가방 이벤트 발생
                EventBus<InventoryEvent>.Publish(new InventoryEvent());
            }
        }
        // 입력된 액션이 지도 액션일 때
        else if (context.action == mapAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
            {
                // UI 상태 이벤트 발생
                EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
                // 맵 이벤트 발생
                EventBus<MapEvent>.Publish(new MapEvent());
            }
        }
        // 입력된 액션이 조작 설명 액션일 때
        else if(context.action == showControlsAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                EventBus<ShowControlsEvent>.Publish(new ShowControlsEvent());
        }
    }
}