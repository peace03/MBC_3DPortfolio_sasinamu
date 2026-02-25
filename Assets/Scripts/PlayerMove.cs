using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("상태")]
    [SerializeField] private bool pressRunKey;          // 달리기 키 누름 여부

    [Header("스탯")]
    [SerializeField] private float moveSpeed;           // 움직이는 속도
    [SerializeField] private float runSpeed;            // 달리는 속도
    [SerializeField] private float rollDistance;        // 구르는 거리
    [SerializeField] private float maxStamina;          // 최대 스테미나
    [SerializeField] private float curStamina;          // 현재 스테미나

    private CharacterController cc;                     // 캐릭터 컨트롤러

    private DuckovInputActions inputActions;            // 인풋 시스템
    private InputAction moveAction;                     // 이동 액션
    private InputAction runAction;                      // 달리기 액션
    private InputAction rollAction;                     // 구르기 액션

    private Vector2 moveInput;                          // 입력 값

    private void Awake()
    {
        // 초기화
        curStamina = maxStamina;

        cc = GetComponent<CharacterController>();

        inputActions = new DuckovInputActions();
        moveAction = inputActions.Player.Move;
        runAction = inputActions.Player.Run;
        rollAction = inputActions.Player.Roll;
    }

    private void OnEnable()
    {
        // 인풋 시스템 구독
        inputActions.Player.Enable();
    }

    private void Update()
    {
        if (runAction.WasPerformedThisFrame())
            pressRunKey = true;

        if (runAction.WasCompletedThisFrame())
            pressRunKey = false;

        // 스테미나
        HandleStamina();
    }

    private void FixedUpdate()
    {
        // 이동
        HandleMove();
    }

    private void OnDisable()
    {
        // 인풋 시스템 구독 해제
        inputActions.Player.Disable();
    }

    // 이동 관리 함수
    private void HandleMove()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        // 입력 값에 따른 방향 정하기
        Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        // 달리기 조건(키 눌림, 스테미나 있음)에 해당되면 달리기 속도, 아니라면 움직이는 속도
        float curSpeed = (pressRunKey && curStamina > 0) ? runSpeed : moveSpeed;
        cc.Move(dir * curSpeed * Time.fixedDeltaTime);
    }

    // 스테미나 관리 함수
    private void HandleStamina()
    {
        // 달리기 조건(키 눌림, 방향키 입력 중)에 해당되면 스테미나 감소, 아니라면 스테미나 증가
        curStamina += (pressRunKey && moveInput.magnitude > 0) ? -Time.deltaTime : Time.deltaTime;
        // 스테미나 최소값, 최대값에 맞춰서 반영하기
        curStamina = Mathf.Clamp(curStamina, 0, maxStamina);
    }
}