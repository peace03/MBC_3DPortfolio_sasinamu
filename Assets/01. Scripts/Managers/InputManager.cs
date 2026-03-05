using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private bool canInput;         // 조작 가능 여부

    private List<InputAction> allActionsList;       // 모든 액션들 리스트
    private DuckovInputActions inputActions;        // 인풋 시스템
    private InputAction moveAction;                 // 이동 액션
    private InputAction runAction;                  // 달리기 액션
    private InputAction rollAction;                 // 구르기 액션
    private InputAction interactAction;             // 상호작용 액션
    private InputAction escAction;                  // ESC 액션
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
        escAction = inputActions.Player.Esc;
        allActionsList.Add(escAction);
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
        // 입력된 액션이 ESC 액션일 때
        if (context.action == escAction)
        {
            // 키를 눌렀을 때
            if (context.performed)
                // 이벤트 발생
                EventBus<EscEvent>.Publish(new EscEvent());

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
    }
}