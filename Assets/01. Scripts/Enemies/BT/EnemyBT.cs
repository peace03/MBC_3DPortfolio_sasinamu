using System.Collections.Generic;
using UnityEngine;

public class EnemyBT : MonoBehaviour, IEnemyPauseHandler
{
    private BT_Node root;                                               // BT의 뿌리(시작) 노드

    [Header("총알")]
    [SerializeField] private BulletFactory bulletFactory;               // 총알 공장

    [Header("위치")]
    [SerializeField] private Transform player;                          // 플레이어
    [SerializeField] private Transform returnPoint;                     // 복귀 지점
    [SerializeField] private Transform patrolPoint;                     // 순찰 지점
    [SerializeField] private Transform firePoint;                       // 사격 위치

    [Header("정보")]
    [SerializeField] private bool isGamePause;                          // 일시정지 여부
    [SerializeField] private float lastAttackTime;                      // 마지막 공격 시간
    [SerializeField] private float lastDamagedTime;                     // 마지막 피격 시간
    [SerializeField] private bool needToReturn;                         // 복귀 필요 여부
    [SerializeField] private bool needToSetPatrolPoint = true;          // 순찰 지점 설정 필요 여부
    [SerializeField] private bool isIdle = false;                       // 대기 여부
    [SerializeField] private float startIdleTime;                       // 대기 시작 시간

    private EnemyStat stat;                                             // 스탯
    private EnemyMove move;                                             // 이동
    private EnemyFire fire;                                             // 사격
    private EnemyAnimationChanger anim;                                 // 애니메이션

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
                // 피격 경직 확인
                new BT_Action(CheckHitStunTime),
                // 피격
                new BT_Action(DamagedEnemy)
            }),
            // 공격
            new BT_Sequence(new List<BT_Node>
            {
                // 공격 딜레이 확인
                new BT_Action(CheckAttackDelayTime),
                // 공격
                new BT_Action(AttackPlayer)
            }),
            // 추격
            new BT_Action(ChasePlayer),
            // 복귀
            new BT_Action(MoveToReturnPoint),
            // 순찰(패트롤)
            new BT_Sequence(new List<BT_Node>
            {
                // 순찰 지점 설정
                new BT_Action(SetRandomPatrolPoint),
                // 순찰
                new BT_Action(MoveToPatrolPoint)
            }),
            // 대기
            new BT_Sequence(new List<BT_Node>
            {
                // 대기 시간 확인
                new BT_Action(CheckIdleTime),
                // 대기
                new BT_Action(Idle)
            })
        });

        // 초기화
        InitEnemy();
    }

    private void OnEnable()
    {
        // 적 멈춤 이벤트 구독
        Subject<IEnemyPauseHandler>.Attach(this);
    }

    private void Update()
    {
        // 일시정지 중이라면
        if (isGamePause)
            // 종료
            return;

        // BT 실행
        root.Evaluate();
    }

    private void OnDisable()
    {
        // 적 멈춤 이벤트 구독 해제
        Subject<IEnemyPauseHandler>.Detach(this);
    }

    // 적 초기화 함수
    private void InitEnemy()
    {
        // 적 컴포넌트 받아오기
        stat = GetComponent<EnemyStat>();
        move = GetComponent<EnemyMove>();
        fire = GetComponent<EnemyFire>();
        anim = GetComponent<EnemyAnimationChanger>();

        // 컴포넌트 초기화
        move.Init(stat.MoveSpeed);
        fire.Init(bulletFactory, player, firePoint, stat.MaxSpreadAngle);
    }

    // 적 멈춤 함수
    public void OnEnemyPause(bool state) => isGamePause = state;

    // 죽음 확인 함수
    private BT_NodeStatus CheckDead()
    {
        // 죽었다면 ? 성공 반환 : 실패 반환
        return stat.IsDead ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 죽음 함수
    private BT_NodeStatus IsDead()
    {
        // 애니메이션 변경
        anim.ChangeAnimation(EnemyAnimState.Dead);
        // 적 오브젝트 비활성화
        gameObject.SetActive(false);
        // 적 죽음 이벤트 발생 ★ 상자 만들어주기 ★
        Subject<IEnemyDeadHandler>.Publish(h => h.OnEnemyDead(transform.position));
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 피격 경직 시간 확인 함수
    private BT_NodeStatus CheckHitStunTime()
    {
        // 피격 상태이거나, 초기 상태가 아니고 마지막 피격 시간에서 피격 경직 시간만큼 지나지 않았다면 ? 성공 반환 : 실패 반환
        return (stat.IsDamaged ||
                (lastDamagedTime != 0 && Time.time - lastDamagedTime <= stat.HitStunTime)) ?
                                                    BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 적 피격 함수
    private BT_NodeStatus DamagedEnemy()
    {
        // 피격 상태가 되었다면
        if (stat.IsDamaged)
        {
            // 마지막 피격 시간 저장
            lastDamagedTime = Time.time;
            // 피격 상태 초기화
            stat.ResetDamagedState();
            // 피격 애니메이션 실행
            anim.PlayDamagedAnimation();
        }

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 공격 딜레이 시간 확인 함수
    private BT_NodeStatus CheckAttackDelayTime()
    {
        // 초기 상태가 아니고, 마지막 공격 시간에서 공격 딜레이 시간만큼 지나지 않았다면 ? 진행 중 반환 : 성공 반환
        return (lastAttackTime != 0 && Time.time - lastAttackTime <= stat.AttackDelayTime) ?
                                                    BT_NodeStatus.Running : BT_NodeStatus.Success;
    }

    // 플레이어 공격 함수
    private BT_NodeStatus AttackPlayer()
    {
        // 플레이어와의 거리가 공격 가능 거리보다 멀어졌다면
        if ((player.position - transform.position).magnitude >= stat.MaxAttackDistance)
            // 실패 반환
            return BT_NodeStatus.Failure;

        // 회전
        move.RotateToTarget(player);
        // 사격
        fire.FireBullet();
        // 공격한 시간 저장
        lastAttackTime = Time.time;
        // 애니메이션 변경
        anim.ChangeAnimation(EnemyAnimState.Attack);
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 플레이어 추격 함수
    private BT_NodeStatus ChasePlayer()
    {
        // 플레이어와의 방향 받아오기
        Vector3 dir = player.position - transform.position;

        // 플레이어와의 거리가 추격 가능 거리보다 멀어졌다면
        if (dir.magnitude >= stat.MaxChaseDistance)
            // 실패 반환
            return BT_NodeStatus.Failure;

        // 복귀가 필요없는 상태였다면
        if (!needToReturn)
            // 복귀가 필요한 상태로 변경
            needToReturn = true;

        // 회전
        move.RotateToTarget(player);
        // 이동
        move.MoveToTarget(player.position + dir.normalized * stat.MaxAttackDistance, 0.1f);
        // 애니메이션 변경
        anim.ChangeAnimation(EnemyAnimState.Move);
        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 복귀 함수
    private BT_NodeStatus MoveToReturnPoint()
    {
        // 복귀가 필요없는 상태라면
        if (!needToReturn)
            // 실패 반환
            return BT_NodeStatus.Failure;

        // 복귀 지점과의 거리가 가깝다면
        if ((returnPoint.position - transform.position).magnitude <= 0.1f)
        {
            // 복귀가 필요없는 상태로 변환
            needToReturn = false;
            // 순찰 지점 설정 필요
            needToSetPatrolPoint = true;
            // 대기 시작 시간 저장
            startIdleTime = Time.time;
            // 성공 반환
            return BT_NodeStatus.Success;
        }

        // 회전
        move.RotateToTarget(returnPoint);
        // 이동
        move.MoveToTarget(returnPoint.position, 0.1f);
        // 애니메이션 변경
        anim.ChangeAnimation(EnemyAnimState.Move);
        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 순찰 지점 랜덤으로 정하는 함수
    private BT_NodeStatus SetRandomPatrolPoint()
    {
        // 대기 중이라면
        if (isIdle)
            // 실패 반환
            return BT_NodeStatus.Failure;

        // 순찰 지점 설정이 필요하지 않다면
        if (!needToSetPatrolPoint)
            // 성공 반환
            return BT_NodeStatus.Success;

        // 지름이 1인 원에서 뽑은 랜덤 위치에 최대 순찰 거리를 곱해서 저장
        Vector3 rand = Random.insideUnitSphere * stat.MaxPatrolDistance;
        // 높이 제거
        rand.y = 0f;
        // 순찰 지점의 위치를 복귀 지점의 위치에 랜덤 위치를 더한 값으로 설정
        patrolPoint.position = returnPoint.position + rand;
        // 순찰 지점 설정 필요 없음
        needToSetPatrolPoint = false;
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 순찰 함수
    private BT_NodeStatus MoveToPatrolPoint()
    {
        // 순찰 지점과 가까워졌다면
        if ((patrolPoint.position - transform.position).magnitude <= 0.1f)
        {
            // 대기 시작
            isIdle = true;
            // 대기 시작 시간 저장
            startIdleTime = Time.time;
            // 실패 반환
            return BT_NodeStatus.Failure;
        }

        // 회전
        move.RotateToTarget(patrolPoint);
        // 이동
        move.MoveToTarget(patrolPoint.position, 0.1f);
        // 애니메이션 변경
        anim.ChangeAnimation(EnemyAnimState.Move);
        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 대기 시간 확인 함수
    private BT_NodeStatus CheckIdleTime()
    {
        // 대기 시작 시간에서 대기 시간만큼 지났다면
        if (Time.time - startIdleTime >= stat.IdleTime)
        {
            // 대기 중 아님
            isIdle = false;
            // 순찰 지점 설정 필요
            needToSetPatrolPoint = true;
        }

        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 대기 함수
    private BT_NodeStatus Idle()
    {
        // 애니메이션 재생
        anim.ChangeAnimation(EnemyAnimState.Idle);
        // 진행 중 반환
        return BT_NodeStatus.Running;
    }
}