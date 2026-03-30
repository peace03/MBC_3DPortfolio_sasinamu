using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class EnemyBT : MonoBehaviour, IEnemyPauseHandler
{
    [Header("위치")]
    [SerializeField] private Transform returnPoint;                     // 복귀 지점
    [SerializeField] private Transform patrolPoint;                     // 순찰 지점
    [SerializeField] private Transform firePoint;                       // 사격 위치

    [Header("정보")]
    [SerializeField] private Slider hpUI;                               // 체력 UI
    [SerializeField] private bool isGamePause;                          // 일시정지 여부
    [SerializeField] private float lastAttackTime;                      // 마지막 공격 시간
    [SerializeField] private float lastDamagedTime;                     // 마지막 피격 시간
    [SerializeField] private bool needToReturn;                         // 복귀 필요 여부
    [SerializeField] private bool needToSetPatrolPoint = true;          // 순찰 지점 설정 필요 여부
    [SerializeField] private bool isIdle = false;                       // 대기 여부
    [SerializeField] private float startIdleTime;                       // 대기 시작 시간

    private IObjectPool<EnemyBT> poolRef;                               // 적 오브젝트 풀 주소

    private BT_Node root;                                               // BT의 뿌리(시작) 노드
    private BulletFactory bulletFactory;                                // 총알 공장
    private Transform player;                                           // 플레이어

    private EnemyStat stat;                                             // 스탯
    private EnemyRotate rot;                                            // 회전
    private NavMeshAgent agent;                                         // 이동 AI
    private NavMeshPath path;                                           // 이동 경로
    private EnemyFire fire;                                             // 사격
    private EnemyAnimationChanger anim;                                 // 애니메이션

    private void Start()
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
                // 공격 지연 시간 확인
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
    public void Init(Transform player, BulletFactory factory, IObjectPool<EnemyBT> pool)
    {
        // 초기화
        this.player = player;
        hpUI.gameObject.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        stat = GetComponent<EnemyStat>();
        stat.Init(agent);
        rot = GetComponent<EnemyRotate>();
        fire = GetComponent<EnemyFire>();
        anim = GetComponent<EnemyAnimationChanger>();
        path = new();
        bulletFactory = factory;
        fire.Init(bulletFactory, player, firePoint, stat.MaxSpreadAngle);
        poolRef = pool;
    }

    public void SetPointPosition(Vector3 pos) => returnPoint.parent.position = pos;

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
        poolRef.Release(this);
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
            // 체력 UI가 비활성화 중이라면
            if (!hpUI.gameObject.activeSelf)
                // 체력 UI 활성화
                hpUI.gameObject.SetActive(true);

            // 체력 UI 값 반영
            hpUI.value = stat.HpRatio;
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

    // 공격 지연 시간 확인 함수
    private BT_NodeStatus CheckAttackDelayTime()
    {
        // 초기 상태가 아니고, 마지막 공격 시간에서 공격 지연 시간만큼 지나지 않았다면 ? 진행 중 반환 : 성공 반환
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

        // 멈춘 상태가 아니라면
        if (!agent.isStopped)
        {
            // 멈추기
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // 회전
        rot.RotateToTarget(player);
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

        // 멈춘 상태라면
        if (agent.isStopped)
            // 멈춘 상태 풀기
            agent.isStopped = false;

        // 회전
        rot.RotateToTarget(player);
        // 이동
        agent.SetDestination(player.position);
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
        rot.RotateToTarget(returnPoint);
        // 이동
        agent.SetDestination(returnPoint.position);
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

        // 순찰 지점 저장할 변수 선언
        Vector3 targetPoint;
        // 순찰 지점 찾음 여부
        bool isFound = SetPatrolPoint(out targetPoint);

        // 순찰 지점을 못찾았다면
        if (!isFound)
            // 실패 반환
            return BT_NodeStatus.Failure;

        // 순찰 지점의 위치를 복귀 지점의 위치에 랜덤 위치를 더한 값으로 설정
        patrolPoint.position = targetPoint;
        // 순찰 지점 설정 필요 없음
        needToSetPatrolPoint = false;
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 순찰 지점 정하는 함수
    private bool SetPatrolPoint(out Vector3 target)
    {
        // 최대 10번까지 랜덤으로 정하기
        for (int i = 0; i < 10; i++)
        {
            // 지름이 1인 원에서 뽑은 랜덤 위치에 최대 순찰 거리를 곱해서 저장
            Vector3 rand = Random.insideUnitSphere * stat.MaxPatrolDistance;

            // 근처 바닥을 찾았다면
            if (NavMesh.SamplePosition(returnPoint.position + rand, out var hit, 2f, NavMesh.AllAreas))
            {
                // 경로 계산 시도가 성공했고 갈 수 있는 길이라면
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    // 경로의 거리가 지름의 0.6배보다 크다면
                    if (CalculatePathDistance(path) > stat.MaxPatrolDistance * 0.6f)
                        // 다시 경로 찾기
                        continue;

                    // 순찰 지점 저장
                    target = hit.position;
                    // 순찰 지점 찾음
                    return true;
                }
            }
        }

        // 초기화
        target = Vector3.zero;
        // 순찰 지점 못찾음
        return false;
    }

    // 경로 거리 계산 함수
    private float CalculatePathDistance(NavMeshPath path)
    {
        // 경로 계산에 실패했거나, 너무 근처에 있다면
        if (path.corners.Length < 2)
            // 계산 종료
            return 0f;

        // 거리를 저장할 변수 선언
        float distance = 0f;

        // 꺾이는 지점 개수만큼
        for (int i = 0; i < path.corners.Length - 1; i++)
            // 거리 더하기
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);

        // 거리 반환
        return distance;
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
        rot.RotateToTarget(patrolPoint);
        // 이동
        agent.SetDestination(patrolPoint.position);
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