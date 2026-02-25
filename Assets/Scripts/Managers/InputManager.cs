using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private DuckovInputActions inputActions;            // 인풋 시스템
    private InputAction moveAction;                     // 이동 액션
    private InputAction runAction;                      // 달리기 액션
    private InputAction rollAction;                     // 구르기 액션

    private void Awake()
    {
        inputActions = new DuckovInputActions();
        moveAction = inputActions.Player.Move;
        runAction = inputActions.Player.Run;
        rollAction = inputActions.Player.Roll;
    }

    private void OnEnable() => inputActions.Player.Enable();

    private void OnDisable() => inputActions.Player.Disable();

    // 이동 입력 반환 함수
    public Vector2 GetMoveInput()
    {
        return moveAction.ReadValue<Vector2>();
    }
}