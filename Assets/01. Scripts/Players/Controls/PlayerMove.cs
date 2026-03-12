using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private bool pressedRunKey;            // 달리기 키 누름 여부
    [SerializeField] private bool isRolling;                // 구르는 중인지 여부

    [Header("스탯")]
    [SerializeField] private float moveSpeed;               // 이동 속도
    [SerializeField] private float runSpeedMultiplier;      // 달리기 속도 배율
    [SerializeField] private float runStamRate;             // 초당 달리기 스테미나 소모량

    [Space(10)]
    [SerializeField] private float rollTime;                // 구르는 시간(클수록 느리게 구름)
    [SerializeField] private float rollDistance;            // 구르는 거리
    [SerializeField] private float rollStamAmount;          // 구르기 스테미나 소모량

    private CharacterController cc;                         // 캐릭터 컨트롤러
    private Transform mainCamTransform;                     // 메인 카메라 트랜스폼
    private PlayerManager manager;                          // 플레이어 매니저

    private Vector2 moveInput;                              // 입력 값

    private void Awake()
    {
        // 초기화
        cc = transform.GetComponent<CharacterController>();
        mainCamTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        // 이동 이벤트 구독
        EventBus<MoveEvent>.OnEvent += SetMoveInput;
        // 달리기 이벤트 구독
        EventBus<RunEvent>.OnEvent += SetPressedRunKey;
        // 구르기 이벤트 구독
        EventBus<RollEvent>.OnEvent += PressedRollKey;
    }

    private void FixedUpdate()
    {
        // 입력 값이 있고, 구르는 중이 아니라면
        if (moveInput.magnitude > 0 && !isRolling)
            // 이동
            HandleMove();
    }

    private void OnDisable()
    {
        // 이동 이벤트 구독 해제
        EventBus<MoveEvent>.OnEvent -= SetMoveInput;
        // 달리기 이벤트 구독 해제
        EventBus<RunEvent>.OnEvent -= SetPressedRunKey;
        // 구르기 이벤트 구독 해제
        EventBus<RollEvent>.OnEvent -= PressedRollKey;
    }

    // 초기화 함수
    public void Init(PlayerManager manager) => this.manager = manager;

    // 이동 입력 값 설정 함수
    private void SetMoveInput(MoveEvent data) => moveInput = data.moveInput;

    // 달리기 키 입력 값 설정 함수
    private void SetPressedRunKey(RunEvent data) => pressedRunKey = data.isPressed;

    // 구르기 키 입력 함수
    private void PressedRollKey(RollEvent data)
    {
        // 구르는 중이거나, 현재 스테미나가 구르기 스테미나 소모량보다 적다면
        if (isRolling || !manager.Stat.UseStamina(rollStamAmount))
            // 종료
            return;

        // 방향키 입력 값이 있다면
        if (moveInput.magnitude > 0)
            // 움직이는 방향으로 구르기
            StartCoroutine(RollingCoroutine(CalculateMoveDirection()));
        // 방향키 입력 값이 없다면
        else
            // 플레이어가 바라보는 방향으로 구르기
            StartCoroutine(RollingCoroutine(transform.forward));
    }

    // 이동 관리 함수
    private void HandleMove()
    {
        // 움직일 방향 저장
        Vector3 dir = CalculateMoveDirection();
        // 달리기 키를 눌렀고 스테미나가 있다면 ? 달리기 속도 : 이동 속도
        float finalSpeed = (pressedRunKey && manager.Stat.UseStamina(runStamRate * Time.deltaTime)) ?
                                                                moveSpeed * runSpeedMultiplier : moveSpeed;
        // 무게에 따라 무게 배율도 곱해줘야 함
        // finalSpeed=
        // 캐릭터 컨트롤러는 중력이 없으니 y축 눌러주기
        float yVelocity = 0f;

        // 땅에 있다면
        if (cc.isGrounded)
            // 고정적으로 눌러주기
            yVelocity = -2f;

        // 방향에 값 반영
        dir.y = yVelocity;
        // 이동(방향 * 속도 * 시간)
        cc.Move(dir * finalSpeed * Time.fixedDeltaTime);

    }

    // 움직일 방향 계산해서 반환하는 함수
    private Vector3 CalculateMoveDirection()
    {
        // 메인 카메라의 정면 값 저장
        Vector3 camForward = mainCamTransform.forward;
        // 메인 카메라의 우측 값 저장
        Vector3 camRight = mainCamTransform.right;
        // 메인 카메라의 높이 값 초기화
        camForward.y = camRight.y = 0f;
        // 메인 카메라 정면 값 정리
        camForward.Normalize();
        // 메인 카메라 우측 값 정리
        camRight.Normalize();
        // 움직일 방향 반환
        return (camForward * moveInput.y + camRight * moveInput.x).normalized;
    }

    // 구르기 코루틴 함수
    private IEnumerator RollingCoroutine(Vector3 dir)
    {
        // 구르는 중임
        isRolling = true;
        // 스테미나 사용
        UseStamina(rollStamAmount);
        // 속도 저장
        float speed = rollDistance / rollTime;
        // 진행률
        float progress = 0f;

        while (true)
        {
            // 이동
            cc.Move(dir * speed * Time.deltaTime);
            // 진행률 증가
            progress += Time.deltaTime;
            // 프레임 기다리기
            yield return null;

            // 진행률이 차면
            if (progress >= rollTime)
                // 종료
                break;
        }

        // 구르기 종료
        isRolling = false;
    }

    // 스테미나 감소 함수
    private void UseStamina(float amount)
    {
        // 스테미나 감소 후 최소값, 최대값에 맞춰서 반영
        curStamina = Mathf.Clamp(curStamina - amount, 0, maxStamina);
        // 스테미나 사용한 시간 갱신
        lastUsedStamTime = Time.time;
    }
}