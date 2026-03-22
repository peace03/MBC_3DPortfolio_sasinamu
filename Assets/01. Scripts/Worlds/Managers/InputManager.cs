using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, IUIStateHandler
{
    [Header("정보")]
    [SerializeField] private bool canInput;         // 조작 가능 여부

    private List<InputAction> allActionsList;       // 모든 액션들 리스트

    #region 액션 맵 - 플레이어
    private DuckovInputActions inputActions;        // 인풋 시스템
    private InputAction moveAction;                 // 이동 액션(WASD)
    private InputAction runAction;                  // 달리기 액션(Shift)
    private InputAction rollAction;                 // 구르기 액션(Space)
    private InputAction interactAction;             // 상호작용 액션(E)
    private InputAction pauseAction;                // 일시정지 액션(ESC)
    private InputAction cancelAction;               // 취소 액션(X)
    private InputAction fireAction;                 // 사격 액션(마우스 좌클릭)
    private InputAction fireModeAction;             // 사격 모드 변경 액션(B)
    private InputAction reloadAction;               // 장전 액션(R)
    private InputAction inventoryAction;            // 가방 액션(Tab)
    private InputAction mapAction;                  // 지도 액션(M)
    private InputAction showControlsAction;         // 조작 설명 액션(O)
    private InputAction quickSlotAction;            // 퀵슬롯 액션(1 ~ 8)
    #endregion

    private InputAction mousePosAction;             // 마우스 위치 액션

    private Vector2 curMousePos;                    // 현재 마우스 위치
    public Vector2 CurMousePos => curMousePos;

    private Vector2 lastMoveInput;                  // 마지막 이동 입력 값
    public Vector2 LastMoveInput => lastMoveInput;

    private void Awake()
    {
        // 초기화
        canInput = true;
        allActionsList = new List<InputAction>();
        inputActions = new DuckovInputActions();
        InitAllActions();
    }

    private void OnEnable()
    {
        // 입력 이벤트 가능
        inputActions.Player.Enable();
        inputActions.Mouse.Enable();

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
        Subject<IUIStateHandler>.Attach(this);
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
        // 입력 이벤트 불가능
        inputActions.Player.Disable();
        inputActions.Mouse.Disable();

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

        // UI 상태 이벤트 구독 해제
        Subject<IUIStateHandler>.Detach(this);
    }

    // 모든 액션 초기화 함수
    private void InitAllActions()
    {
        // 초기화
        moveAction = inputActions.Player.Move;                      // 이동
        runAction = inputActions.Player.Run;                        // 달리기
        rollAction = inputActions.Player.Roll;                      // 구르기
        interactAction = inputActions.Player.Interact;              // 상호작용
        pauseAction = inputActions.Player.Pause;                    // 일시정지
        cancelAction = inputActions.Player.Cancel;                  // 취소     
        fireAction = inputActions.Player.Fire;                      // 사격
        fireModeAction = inputActions.Player.FireMode;              // 사격 모드 변경
        reloadAction = inputActions.Player.Reload;                  // 장전
        inventoryAction = inputActions.Player.Inventory;            // 가방
        mapAction = inputActions.Player.Map;                        // 지도
        showControlsAction = inputActions.Player.ShowControls;      // 조작 설명
        quickSlotAction = inputActions.Player.QuickSlot;            // 퀵슬롯
        mousePosAction = inputActions.Mouse.Position;               // 마우스 위치

        // 입력 종류가 필요한 액션들은 액션 리스트에 추가
        allActionsList.Add(moveAction);
        allActionsList.Add(runAction);
        allActionsList.Add(rollAction);
        allActionsList.Add(interactAction);
        allActionsList.Add(pauseAction);
        allActionsList.Add(cancelAction);
        allActionsList.Add(fireAction);
        allActionsList.Add(fireModeAction);
        allActionsList.Add(reloadAction);
        allActionsList.Add(inventoryAction);
        allActionsList.Add(mapAction);
        allActionsList.Add(showControlsAction);
        allActionsList.Add(quickSlotAction);
    }

    // 마우스 위치(Vector2)를 월드 좌표(Vector3)로 바꾸고 반환하는 함수
    public Vector3 GetMouseWorldPosition(float playerCenterHeight)
    {
        // 플레이어의 중간 높이에서 위를 바라보는 보이지 않는 평면 생성
        Plane groundPlane = new(Vector3.up, new Vector3(0, playerCenterHeight, 0));
        // 메인 카메라의 위치에서 카메라 렌즈(니어 플레인) 위의 마우스 위치를 통과하는 레이저 저장
        Ray ray = Camera.main.ScreenPointToRay(curMousePos);

        // 평면과 부딪히는 레이저가 있다면
        if (groundPlane.Raycast(ray, out float distance))
        {
            // 카메라와 레이저의 거리를 이용해 마우스의 위치 저장
            Vector3 hitPoint = ray.GetPoint(distance);
            // y 값을 플레이어의 중간 높이로 초기화
            hitPoint.y = playerCenterHeight;
            // 마우스 위치 반환
            return hitPoint;
        }

        // 평면과 부딪히는 레이저 없음
        return Vector3.zero;
    }

    // 화면 중앙과 현재 마우스 위치의 거리를 비율로 반환하는 함수
    public Vector2 GetMouseOffsetRatio()
    {
        // 현재 마우스 위치를 0 ~ 1 사이로 정규화
        float mouseX = curMousePos.x / Screen.width;
        float mouseY = curMousePos.y / Screen.height;
        // 범위를 -1 ~ 1로 바꾸기
        float xRatio = Mathf.Lerp(-1f, 1f, mouseX);
        float yRatio = Mathf.Lerp(-1f, 1f, mouseY);
        // 마우스가 화면 밖으로 나가도 반영하지 않게 제한
        float finalXRatio = Mathf.Clamp(xRatio, -1f, 1f);
        float finalYRatio = Mathf.Clamp(yRatio, -1f, 1f);
        // 마우스 위치 비율 반환
        return new Vector2(finalXRatio, finalYRatio);
    }

    // UI 상태 함수(UI가 열리면 조작 불가, 닫히면 조작 가능)
    public void OnUIState(bool state) => canInput = !state;

    // 입력 액션들에 대한 이벤트 발행 함수
    public void PublishInputActions(InputAction.CallbackContext context)
    {
        // 입력된 액션이 일시정지 액션일 때
        if (context.action == pauseAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                Subject<IGamePauseHandler>.Publish(h => h.OnPause());

            // 종료
            return;
        }

        // 조작이 불가능한 상태라면
        if (!canInput)
        {
            // 이동 입력 값이 있었다면
            if (lastMoveInput != Vector2.zero)
                // 초기화
                lastMoveInput = Vector2.zero;

            // 종료
            return;
        }

        // 입력된 액션이 이동 액션일 때
        if (context.action == moveAction)
        {
            // 현재 이동 입력 값 저장
            Vector2 curMoveInput = context.ReadValue<Vector2>();

            // 마지막 이동 입력 값과 현재 이동 입력 값이 같다면
            if (lastMoveInput == curMoveInput)
                // 종료
                return;

            // 마지막 이동 입력 값 저장
            lastMoveInput = curMoveInput;
        }
        // 입력된 액션이 달리기 액션일 때
        else if (context.action == runAction)
            // 이벤트 발생(현재 키 입력 값에 따른 달리기 함수 실행)
            Subject<IPlayerRunHandler>.Publish(h => h.OnRun(context.ReadValueAsButton()));
        // 입력된 액션이 구르기 액션일 때
        else if (context.action == rollAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생(구르기 함수 실행)
                Subject<IPlayerRollHandler>.Publish(h => h.OnRoll());
        }
        // 입력된 액션이 상호작용 액션일 때
        else if (context.action == interactAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생(상호작용 함수 실행)
                Subject<IPlayerInteractHandler>.Publish(h => h.OnInteract());
        }
        // 입력된 액션이 취소 액션일 때
        else if (context.action == cancelAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                Subject<IPlayerCancelHandler>.Publish(h => h.OnCancel());
        }
        // 입력된 액션이 사격 액션일 때
        else if (context.action == fireAction)
            // 이벤트 발생(사격 함수)
            Subject<IPlayerFireHandler>.Publish(h => h.OnFire(context.ReadValueAsButton()));
        // 입력된 액션이 사격 모드 변경 액션일 때
        else if (context.action == fireModeAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                Subject<IPlayerFireModeHandler>.Publish(h => h.OnFireMode());
        }
        // 입력된 액션이 장전 액션일 때
        else if (context.action == reloadAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                Subject<IPlayerReloadHandler>.Publish(h => h.OnReload());
        }
        // 입력된 액션이 가방 액션일 때
        //else if (context.action == inventoryAction)
        //{
        //    // 키를 눌렀을 때
        //    if (context.performed)
        //    {
        //        // UI 상태 이벤트 발생
        //        EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
        //        // 가방 이벤트 발생
        //        EventBus<InventoryEvent>.Publish(new InventoryEvent());
        //    }
        //}
        // 입력된 액션이 지도 액션일 때
        //else if (context.action == mapAction)
        //{
        //    // 키를 눌렀을 때
        //    if (context.performed)
        //    {
        //        // UI 상태 이벤트 발생
        //        EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
        //        // 맵 이벤트 발생
        //        EventBus<MapEvent>.Publish(new MapEvent());
        //    }
        //}
        // 입력된 액션이 조작 설명 액션일 때
        //else if (context.action == showControlsAction)
        //{
        //    // 키를 눌렀을 때
        //    if (context.performed)
        //        // 이벤트 발생
        //        EventBus<ShowControlsEvent>.Publish(new ShowControlsEvent());
        //}
        // 입력된 액션이 퀵슬롯 액션일 때
        //else if (context.action == quickSlotAction)
        //{
        //    // 키를 눌렀을 때
        //    if (context.performed)
        //        // 이벤트 발생
        //        EventBus<QuickSlotEvent>.Publish(new QuickSlotEvent(context.ReadValue<int>()));
        //}
    }
}