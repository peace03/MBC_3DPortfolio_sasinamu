using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour, IPlayerRunHandler, IPlayerRollHandler
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
    private PlayerStat stat;                                // 플레이어 스탯
    private PlayerAnimationChanger anim;                    // 플레이어 애니메이션
    private InputManager inputManager;                      // 입력 매니저

    private Vector2 curMoveInput;                           // 현재 이동 입력 값

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        // 달리기 이벤트 구독
        Subject<IPlayerRunHandler>.Attach(this);
        // 구르기 이벤트 구독
        Subject<IPlayerRollHandler>.Attach(this);
    }

    private void FixedUpdate()
    {
        // 현재 이동 입력 값 저장
        curMoveInput = inputManager.LastMoveInput;
        // 이동
        HandleMove();
    }

    private void OnDisable()
    {
        // 달리기 이벤트 구독 해제
        Subject<IPlayerRunHandler>.Detach(this);
        // 구르기 이벤트 구독 해제
        Subject<IPlayerRollHandler>.Detach(this);
    }

    // 초기화 함수
    public void Init(PlayerStat stat, PlayerAnimationChanger animation, InputManager manager)
    {
        this.stat = stat;
        anim = animation;
        inputManager = manager;
    }

    // 이동 관리 함수
    private void HandleMove()
    {
        // 구르는 중이라면
        if (isRolling)
            // 종료
            return;

        // 현재 이동 입력 값이 없다면 
        if (curMoveInput.magnitude <= 0)
        {
            // 대기 모습으로 변경
            anim.ChangePlayerAnimation(PlayerAnimState.Idle);
            // 종료
            return;
        }

        // 움직일 방향 저장
        Vector3 dir = CalculateMoveDirection();
        // 달리기 키를 눌렀고 스테미나가 있다면 ? 이동 속도 * 달리기 * 무게 : 이동 속도 * 무게
        float finalSpeed = (pressedRunKey && stat.UseStamina(runStamRate * Time.deltaTime)) ?
                                moveSpeed * runSpeedMultiplier * stat.SpeedMultiplierCurInvWeight
                                                    : moveSpeed * stat.SpeedMultiplierCurInvWeight;
        // 캐릭터 컨트롤러는 중력이 없으니 y축 눌러주기
        float yVelocity = 0f;

        // 땅에 있다면
        if (cc.isGrounded)
            // 고정적으로 눌러주기
            yVelocity = -2f;

        if (stat.AttackPower == 0)
        {
            if (pressedRunKey)
                anim.ChangePlayerAnimation(PlayerAnimState.Run);
            else
                // 걷는 모습(애니메이션)으로 변경
                anim.ChangePlayerAnimation(PlayerAnimState.Walk);
        }
        else if (stat.AttackPower > 8)
        {
            if (pressedRunKey)
                anim.ChangePlayerAnimation(PlayerAnimState.RunWithWeapon);
            else
                anim.ChangePlayerAnimation(PlayerAnimState.WalkWithWeapon);
        }

        // 방향에 값 반영
        dir.y = yVelocity;
        // 이동(방향 * 속도 * 시간)
        cc.Move(dir * finalSpeed * Time.fixedDeltaTime);
    }

    // 움직일 방향 계산해서 반환하는 함수
    private Vector3 CalculateMoveDirection()
    {
        // 메인 카메라의 정면 값 저장
        Vector3 camForward = Camera.main.transform.forward;
        // 메인 카메라의 우측 값 저장
        Vector3 camRight = Camera.main.transform.right;
        // 메인 카메라의 높이 값 초기화
        camForward.y = camRight.y = 0f;
        // 메인 카메라 정면 값 정리
        camForward.Normalize();
        // 메인 카메라 우측 값 정리
        camRight.Normalize();
        // 움직일 방향 반환
        return (camForward * curMoveInput.y + camRight * curMoveInput.x).normalized;
    }

    // 달리기 입력 값 설정 함수
    public void OnRun(bool isPressed) => pressedRunKey = isPressed;

    // 구르기 입력 함수
    public void OnRoll()
    {
        // 구르는 중이거나, 현재 스테미나가 구르기 스테미나 소모량보다 적다면
        if (isRolling || !stat.UseStamina(rollStamAmount))
            // 종료
            return;

        anim.ChangePlayerAnimation(PlayerAnimState.Roll);

        // 현재 이동 입력 값이 있다면
        if (curMoveInput.magnitude > 0)
            // 움직이는 방향으로 구르기
            StartCoroutine(RollingCoroutine(CalculateMoveDirection()));
        // 현재 이동 입력 값이 없다면
        else
            // 플레이어가 바라보는 방향으로 구르기
            StartCoroutine(RollingCoroutine(transform.forward));
    }

    // 구르기 코루틴 함수
    private IEnumerator RollingCoroutine(Vector3 dir)
    {
        // 구르는 중임
        isRolling = true;
        // 스테미나 사용
        stat.UseStamina(rollStamAmount);
        // 속도 저장
        float speed = rollDistance / rollTime;
        // 진행률
        float progress = 0f;

        // 구르기 시간이 지날 때까지
        while (progress < rollTime)
        {
            // 게임 시간이 멈춰있다면
            if (Time.timeScale == 0)
            {
                // 프레임 단위로 기다리기
                yield return null;
                // 건너뛰기
                continue;
            }

            // 이동
            cc.Move(dir * speed * Time.deltaTime);
            // 진행률 증가
            progress += Time.deltaTime;
            // 프레임 단위로 기다리기
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        // 구르기 종료
        isRolling = false;
    }
}