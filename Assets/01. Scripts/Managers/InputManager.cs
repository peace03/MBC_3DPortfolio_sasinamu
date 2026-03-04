using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private bool canInput;             // 조작 가능 여부

    private List<InputAction> startedActions;           // 단발 액션들
    private List<InputAction> performedActions;         // 지속 액션들
    private DuckovInputActions inputActions;            // 인풋 시스템
    private InputAction moveAction;                     // 이동 액션
    private InputAction runAction;                      // 달리기 액션
    private InputAction mousePosAction;                 // 마우스 위치 액션
    private InputAction rollAction;                     // 구르기 액션
    private InputAction interactAction;                 // 상호작용 액션

    private Vector2 curMousePos;                        // 현재 마우스 위치
    public Vector2 CurMousePos => curMousePos;

    private void Awake()
    {
        // 초기화
        canInput = true;
        startedActions = new List<InputAction>();
        performedActions = new List<InputAction>();
        inputActions = new DuckovInputActions();
        // 이동
        moveAction = inputActions.Player.Move;
        performedActions.Add(moveAction);
        // 달리기
        runAction = inputActions.Player.Run;
        performedActions.Add(runAction);
        // 마우스 위치
        mousePosAction = inputActions.Camera.MousePosition;
        // 구르기
        rollAction = inputActions.Player.Roll;
        startedActions.Add(rollAction);
        // 상호작용
        interactAction = inputActions.Player.Interact;
        startedActions.Add(interactAction);
    }

    private void OnEnable()
    {
        // 입력 이벤트 가능
        inputActions.Player.Enable();
        inputActions.Camera.Enable();

        // 단발 액션들 수만큼
        foreach (var action in startedActions)
        {
            // 키를 눌렀을 때
            action.started += ChangeInputActions;
            // 키에서 손을 뗄 때
            action.canceled += ChangeInputActions;
        }

        // 지속 액션들 수만큼
        foreach (var action in performedActions)
        {
            // 키를 누르고 있을 때
            action.performed += ChangeInputActions;
            // 키에서 손을 뗄 때
            action.canceled += ChangeInputActions;
        }

        // UI 상태 이벤트 구독
        EventBus<UIStateEvent>.OnEvent += SetCanInput;
    }

    private void Update()
    {
        curMousePos = mousePosAction.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        // UI 상태 이벤트 구독 해제
        EventBus<UIStateEvent>.OnEvent -= SetCanInput;

        // 지속 액션들 수만큼
        foreach (var action in performedActions)
        {
            // 키를 누르고 있을 때
            action.performed -= ChangeInputActions;
            // 키에서 손을 뗄 때
            action.canceled -= ChangeInputActions;
        }

        // 단발 액션들 수만큼
        foreach (var action in startedActions)
        {
            // 키를 눌렀을 때
            action.started -= ChangeInputActions;
            // 키에서 손을 뗄 때
            action.canceled -= ChangeInputActions;
        }

        // 입력 이벤트 불가능
        inputActions.Player.Disable();
        inputActions.Camera.Disable();
    }

    // 조작 가능 여부 설정하는 함수
    private void SetCanInput(UIStateEvent data) => canInput = !data.isUIOpen;

    // 입력 액션들 변경 함수
    public void ChangeInputActions(InputAction.CallbackContext context)
    {
        // 조작이 불가능한 상태라면
        if (!canInput)
            // 종료
            return;

        // 이동
        if (context.action == moveAction)
            // 이벤트 발생
            EventBus<MoveEvent>.Publish(new MoveEvent(context.ReadValue<Vector2>()));
        // 달리기
        else if (context.action == runAction)
            // 이벤트 발생
            EventBus<RunEvent>.Publish(new RunEvent(context.ReadValueAsButton()));
        // 구르기
        else if (context.action == rollAction)
            // 이벤트 발생
            EventBus<RollEvent>.Publish(new RollEvent(context.ReadValueAsButton()));
        // 상호작용
        else if (context.action == interactAction)
            // 이벤트 발생
            EventBus<InteractEvent>.Publish(new InteractEvent());
        // 그 외
        else
            Debug.Log("없는 입력 액션");
    }
}