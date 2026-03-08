using System.Collections.Generic;
using UnityEngine;

public class EnemyBT : MonoBehaviour, IDamageable
{
    private BT_Node root;                                   // BT의 뿌리(시작) 노드

    [Header("정보")]
    [SerializeField] private bool isDamaged;                // 피격 여부
    [SerializeField] private float lastAttackTime;          // 마지막 공격 시간
    [SerializeField] private Transform player;              // 플레이어
    [SerializeField] private bool needToReturn;             // 복귀 필요 여부
    [SerializeField] private Transform returnPoint;         // 복귀 지점
    [SerializeField] private bool isArrivalPatrolPoint;     // 순찰 지점 도착 여부
    [SerializeField] private Transform patrolPoint;         // 순찰 지점

    [Header("스탯")]
    [SerializeField] private float curHp;                   // 현재 Hp
    [SerializeField] private float maxHp;                   // 최대 Hp
    [SerializeField] private float moveSpeed;               // 이동 속도
    [SerializeField] private float maxChaseDistance;        // 최대 추적 거리
    [SerializeField] private float attackPower;             // 공격력
    [SerializeField] private float attackDelayTime;         // 공격 딜레이 시간
    [SerializeField] private float maxAttackDistance;       // 최대 공격 거리
    [SerializeField] private float maxReturnDistance;       // 최대 복귀 거리
    [SerializeField] private float maxPatrolDistance;       // 최대 순찰 거리

    private CharacterController cc;                         // 적 캐릭터 컨트롤러
    private Animator anim;                                  // 적 애니메이터

    private void Awake()
    {
        // BT 생성
        root = new BT_Selector(new List<BT_Node>
        {
            // 죽음
            new BT_Sequence(new List<BT_Node>
            {
                // HP가 0인지 확인
                new BT_Action(CheckDead),
                // 죽음
                new BT_Action(IsDead)
            }),
            // 피격
            new BT_Sequence(new List<BT_Node>
            {
                // 피격 상태 확인
                new BT_Action(CheckDamaged),
                // 피격 애니메이션 재생
                new BT_Action(PlayDamagedAnimation)
            }),
            // 공격
            new BT_Sequence(new List<BT_Node>
            {
                // 공격 딜레이 확인
                new BT_Action(CheckAttackDelayTime),
                // 공격 거리 확인
                new BT_Action(CheckAttackDistance),
                // 공격
                new BT_Action(AttackPlayer)
            }),
            // 추격
            new BT_Sequence(new List<BT_Node>
            {
                // 추격 거리 확인
                new BT_Action(CheckChaseDistance),
                // 추격
                new BT_Action(ChasePlayer)
            }),
            // 복귀
            new BT_Sequence(new List<BT_Node>
            {
                // 복귀 거리 확인
                new BT_Action(CheckReturnDistance),
                // 복귀
                new BT_Action(MoveToReturnPoint)
            }),
            // 대기
            new BT_Action(Idle),
            // 순찰(패트롤)
            new BT_Sequence(new List<BT_Node>
            {
                // 순찰 지점 설정
                new BT_Action(SetRandomPatrolPoint),
                // 순찰 거리 확인
                new BT_Action(CheckPatrolDistance),
                // 순찰
                new BT_Action(MoveToPatrolPoint)
            })
        });

        // 초기화
        isArrivalPatrolPoint = true;
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        // Hp 설정
        curHp = maxHp;
    }

    private void Update()
    {
        // BT 실행
        root.Evaluate();
    }

    // 죽음 확인 함수
    private BT_NodeStatus CheckDead()
    {
        // 현재 체력이 0보다 작거나 같다면 ? 성공 반환 : 실패 반환
        return curHp <= 0f ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 죽음 함수
    private BT_NodeStatus IsDead()
    {
        Debug.Log("죽음...");
        // 이전 애니메이션 종료 ★ 나중에 고치기 ★
        anim.SetBool("Idle", false);
        // 애니메이션 재생 ★ 나중에 고치기 ★
        anim.Play("None", 0, 0f);
        // 적 오브젝트 비활성화
        gameObject.SetActive(false);
        // 죽음 이벤트 발생
        EventBus<DeadEvent>.Publish(new DeadEvent());
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 피격 확인 함수
    private BT_NodeStatus CheckDamaged()
    {
        // 피격 상태라면 ? 성공 반환 : 실패 반환
        return isDamaged ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 피격 애니메이션 재생 함수
    private BT_NodeStatus PlayDamagedAnimation()
    {
        Debug.Log("피격 상태");
        // 이전 애니메이션 종료 ★ 나중에 고치기 ★
        anim.SetBool("Idle", false);
        // 애니메이션 재생 ★ 나중에 고치기 ★
        anim.Play("None", 0, 0f);
        // 피격 상태 아님
        isDamaged = false;
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 거리 확인 함수
    private BT_NodeStatus CheckDistance(Vector3 pos, float range)
    {
        // 플레이어와 가까운 타겟의 표면 좌표 저장
        Vector3 trgClosestPos = player.GetComponent<Collider>().ClosestPoint(transform.position);
        // 타겟과의 거리 저장
        float distance = CalculateTargetDistance(trgClosestPos);
        // 타겟과의 거리가 매개변수의 거리보다 작거나 같으면 ? 성공 반환 : 실패 반환
        return distance <= range ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 매개변수 타겟와의 거리 계산 함수
    private float CalculateTargetDistance(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos);
    }

    // 공격 딜레이 시간 확인 함수
    private BT_NodeStatus CheckAttackDelayTime()
    {
        // 공격을 했고, 마지막 공격 시간에서 공격 딜레이 시간만큼 지나지 않았다면
        if (lastAttackTime != 0 && Time.time - lastAttackTime <= attackDelayTime)
            // 진행 중 반환
            return BT_NodeStatus.Running;
        // 공격을 안했거나, 마지막 공격 시간에서 공격 딜레이 시간만큼 지났다면
        else
            // 성공 반환
            return BT_NodeStatus.Success;
    }

    // 공격 거리 확인 함수
    private BT_NodeStatus CheckAttackDistance()
    {
        return CheckDistance(player.position, maxAttackDistance);
    }

    // 플레이어 공격 함수
    private BT_NodeStatus AttackPlayer()
    {
        // 회전
        HandleRotate(player.position);
        // 이동
        HandleMove(player.position);
        // 이전 애니메이션 종료 ★ 나중에 고치기 ★
        anim.SetBool("Idle", false);
        // 애니메이션 재생 ★ 나중에 고치기 ★
        anim.Play("None", 0, 0f);
        // 플레이어 체력 계산

        // 공격한 시간 저장
        lastAttackTime = Time.time;
        // 여기에 if문으로 애니메이션이 끝났을 때, 성공을 반환해야함

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 추격 거리 확인 함수
    private BT_NodeStatus CheckChaseDistance()
    {
        return CheckDistance(player.position, maxChaseDistance);
    }

    // 플레이어 추격 함수
    private BT_NodeStatus ChasePlayer()
    {
        // 복귀가 필요없는 상태였다면
        if (!needToReturn)
            // 복귀가 필요한 상태로 변경
            needToReturn = true;

        // 회전
        HandleRotate(player.position);
        // 이동
        HandleMove(player.position);
        // 이전 애니메이션 종료 ★ 나중에 고치기 ★
        anim.SetBool("Idle", false);
        // 애니메이션 재생 ★ 나중에 고치기 ★
        anim.Play("None", 0, 0f);
        // 플레이어와의 거리가 공격 거리보다 작거나 같다면
        if (CalculateTargetDistance(player.position) <= maxAttackDistance)
            // 성공 반환
            return BT_NodeStatus.Success;

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 복귀 거리 확인 함수
    private BT_NodeStatus CheckReturnDistance()
    {
        // 복귀 지점과의 거리 저장
        float distance = CalculateTargetDistance(returnPoint.position);
        // 복귀 지점과의 거리가 최대 복귀 거리보다 크거나 같고 && 복귀가 필요한 상태라면 ? 성공 반환 : 실패 반환
        return (distance >= maxReturnDistance && needToReturn) ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 복귀 함수
    private BT_NodeStatus MoveToReturnPoint()
    {
        // 복귀가 필요없는 상태라면
        if (!needToReturn)
            // 실패 반환
            return BT_NodeStatus.Failure;

        // 회전
        HandleRotate(returnPoint.position);
        // 이동
        HandleMove(returnPoint.position);
        // 이전 애니메이션 종료 ★ 나중에 고치기 ★
        anim.SetBool("Idle", false);
        // 애니메이션 재생 ★ 나중에 고치기 ★
        anim.Play("None", 0, 0f);
        // 복귀 지점과의 거리가 최대 복귀 거리보다 작거나 같다면
        if (CalculateTargetDistance(returnPoint.position) <= maxReturnDistance)
        {
            // 복귀가 필요없는 상태로 변환
            needToReturn = false;
            // 성공 반환
            return BT_NodeStatus.Success;
        }

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 대기 함수
    private BT_NodeStatus Idle()
    {
        // 순찰 지점에 도착한 상태라면
        if (isArrivalPatrolPoint)
        {
            // Idle 애니메이션 시작 상태가 아니라면
            if (!anim.GetBool("Idle"))
            {
                // Idle 애니메이션 시작 상태
                anim.SetBool("Idle", true);
                // 애니메이션 0초로 시작
                anim.Play("Idle", 0, 0f);
                // 진행 중 반환
                return BT_NodeStatus.Running;
            }

            // 현재 0번 레이어 애니메이터 정보 저장
            var animInfo = anim.GetCurrentAnimatorStateInfo(0);

            // 현재 애니메이션이 Idle이고, 애니메이션 진행 시간이 끝나지 않았다면
            if (animInfo.IsName("Idle") && animInfo.normalizedTime <= 1f)
                // 진행 중 반환
                return BT_NodeStatus.Running;
        }

        // Idle 애니메이션 시작 상태 아님
        anim.SetBool("Idle", false);
        // 실패 반환
        return BT_NodeStatus.Failure;
    }

    // 순찰 지점 랜덤으로 정하는 함수
    private BT_NodeStatus SetRandomPatrolPoint()
    {
        // 순찰 지점에 도착했다면
        if (isArrivalPatrolPoint)
        {
            // 지름이 1인 원에서 뽑은 랜덤 위치에 최대 순찰 거리를 곱해서 저장
            Vector3 rand = Random.insideUnitSphere * maxPatrolDistance;
            // 높이 제거
            rand.y = 0f;
            // 순찰 지점의 위치를 복귀 지점의 위치에 랜덤 위치를 더한 값으로 설정
            patrolPoint.position = returnPoint.position + rand;
            // 순찰 지점에 도착하지 않은 상태
            isArrivalPatrolPoint = false;
        }

        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 순찰 거리 확인 함수
    private BT_NodeStatus CheckPatrolDistance()
    {
        // 순찰 지점과의 거리 저장
        float dis = CalculateTargetDistance(patrolPoint.position);
        // 순찰 지점과의 거리랑 멀다면 ? 성공 반환 : 실패 반환
        return dis >= 0.1f ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 순찰 함수
    private BT_NodeStatus MoveToPatrolPoint()
    {
        // 회전
        HandleRotate(patrolPoint.position);
        // 이동
        HandleMove(patrolPoint.position);
        // 애니메이션 재생

        // 순찰 지점과의 거리가 가깝다면
        if (CalculateTargetDistance(patrolPoint.position) <= 0.1f)
        {
            // 순찰 지점 도착 상태
            isArrivalPatrolPoint = true;
            // 성공 반환
            return BT_NodeStatus.Success;
        }

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 회전 관리 함수
    private void HandleRotate(Vector3 pos)
    {
        // 방향 저장
        Vector3 dir = (pos - transform.position).normalized;
        // 높이 제거
        dir.y = 0f;

        // 방향이 똑같다면
        if (dir == Vector3.zero)
            // 종료
            return;

        // 회전
        transform.forward = dir;
    }

    // 이동 관리 함수
    private void HandleMove(Vector3 pos)
    {
        // 방향 저장
        Vector3 dir = (pos - transform.position).normalized;
        // 높이 제거
        dir.y = 0f;
        // 이동
        cc.Move(dir * moveSpeed * Time.deltaTime);
    }

    // 피격 함수
    public void Damaged(DamagedEvent data)
    {
        // Hp 감소
        curHp -= data.amount;
        // 피격 상태임
        isDamaged = true;
        // 피격 이벤트 발생
        EventBus<DamagedEvent>.Publish(data);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
    }
}