using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private List<InputAction> allActions;               // 액션들 리스트
    private DuckovInputActions inputActions;            // 인풋 시스템
    private InputAction moveAction;                     // 이동 액션
    private InputAction runAction;                      // 달리기 액션
    private InputAction mousePosAction;                 // 마우스 위치 액션
    private InputAction rollAction;                     // 구르기 액션

    private Vector2 curMousePos;                        // 현재 마우스 위치
    public Vector2 CurMousePos => curMousePos;

    private void Awake()
    {
        // 초기화
        allActions = new List<InputAction>();
        inputActions = new DuckovInputActions();
        moveAction = inputActions.Player.Move;
        allActions.Add(moveAction);
        runAction = inputActions.Player.Run;
        allActions.Add(runAction);
        rollAction = inputActions.Player.Roll;
        allActions.Add(rollAction);
        mousePosAction = inputActions.Camera.MousePosition;
    }

    private void OnEnable()
    {
        // 입력 이벤트 가능
        inputActions.Player.Enable();
        inputActions.Camera.Enable();

        // 이벤트 수만큼
        foreach (var action in allActions)
        {
            // 키 눌렀을 때
            action.performed += ChangeInputAction;
            // 키에서 손을 뗄 때
            action.canceled += ChangeInputAction;
        }
    }

    private void Update()
    {
        curMousePos = mousePosAction.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        // 이벤트 수만큼
        foreach (var action in allActions)
        {
            // 키 눌렀을 때
            action.performed -= ChangeInputAction;
            // 키에서 손을 뗄 때
            action.canceled -= ChangeInputAction;
        }

        // 입력 이벤트 불가능
        inputActions.Player.Disable();
        inputActions.Camera.Disable();
    }

    // 입력 이벤트 변경 함수
    public void ChangeInputAction(InputAction.CallbackContext context)
    {
        // 이동 이벤트
        if (context.action == moveAction)
            EventBus<MoveEvent>.Publish(new MoveEvent { moveInput = context.ReadValue<Vector2>() });
        // 달리기 이벤트
        else if (context.action == runAction)
            EventBus<RunEvent>.Publish(new RunEvent { isPressed = context.ReadValueAsButton() });
        // 구르기 이벤트
        else if (context.action == rollAction)
            EventBus<RollEvent>.Publish(new RollEvent { isPressed = context.ReadValueAsButton() });
        // 그 외
        else
            Debug.Log("없는 입력 이벤트");
    }
}