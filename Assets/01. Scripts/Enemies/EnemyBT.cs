using System.Collections.Generic;
using UnityEngine;

public class EnemyBT : MonoBehaviour
{
    private BT_Node root;                                       // BT의 뿌리(시작) 노드

    [Header("위치")]
    [SerializeField] private Transform player;                  // 플레이어
    [SerializeField] private Transform returnPoint;             // 복귀 지점
    [SerializeField] private Transform patrolPoint;             // 순찰 지점

    [Header("정보")]
    [SerializeField] private float lastAttackTime;              // 마지막 공격 시간
    [SerializeField] private float lastDamagedTime;             // 마지막 피격 시간
    [SerializeField] private float startIdleTime;               // 대기 시작 시간
    [SerializeField] private bool needToReturn;                 // 복귀 필요 여부
    [SerializeField] private bool needToResetPatrolPoint        // 순찰 지점 재설정 필요 여부
                                                    = true;

    private EnemyController controller;                         // 적 컨트롤러(이동, 회전, 애니메이션 등)

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
            // 순찰(패트롤)
            new BT_Sequence(new List<BT_Node>
            {
                // 순찰 지점 설정
                new BT_Action(SetRandomPatrolPoint),
                // 순찰 거리 확인
                new BT_Action(CheckPatrolDistance),
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
        controller = GetComponent<EnemyController>();
    }

    private void Update()
    {
        // BT 실행
        root.Evaluate();
    }

    // 죽음 확인 함수
    private BT_NodeStatus CheckDead()
    {
        // 죽었다면 ? 성공 반환 : 실패 반환
        return controller.IsDead ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 죽음 함수
    private BT_NodeStatus IsDead()
    {
        // 타겟 초기화
        controller.ClearTarget();
        // 애니메이션 변경
        controller.ChangeAnimation(AnimState.Dead);
        // 적 오브젝트 비활성화
        gameObject.SetActive(false);
        // 죽음 이벤트 발생
        EventBus<DeadEvent>.Publish(new DeadEvent(false));
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 피격 경직 시간 확인 함수
    private BT_NodeStatus CheckHitStunTime()
    {
        // 피격 상태이거나, 초기 상태가 아니고 마지막 피격 시간에서 피격 경직 시간만큼 지나지 않았다면 ? 성공 반환 : 실패 반환
        return (controller.IsDamaged ||
                (lastDamagedTime != 0 && Time.time - lastDamagedTime <= controller.Stat.HitStunTime)) ?
                                                            BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 적 피격 함수
    private BT_NodeStatus DamagedEnemy()
    {
        // 피격 상태가 되었다면
        if (controller.IsDamaged)
        {
            // 마지막 피격 시간 저장
            lastDamagedTime = Time.time;
            // 피격 상태 초기화
            controller.Stat.ResetDamagedState();
            // 피격 애니메이션 실행
            controller.PlayDamagedAnimation();
        }

        // 진행 중 반환
        return BT_NodeStatus.Running;
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
        // 초기 상태가 아니고, 마지막 공격 시간에서 공격 딜레이 시간만큼 지나지 않았다면 ? 진행 중 반환 : 성공 반환
        return (lastAttackTime != 0 && Time.time - lastAttackTime <= controller.Stat.AttackDelayTime) ?
                                                        BT_NodeStatus.Running : BT_NodeStatus.Success;
    }

    // 공격 거리 확인 함수
    private BT_NodeStatus CheckAttackDistance()
    {
        return CheckDistance(player.position, controller.Stat.MaxAttackDistance);
    }

    // 플레이어 공격 함수
    private BT_NodeStatus AttackPlayer()
    {
        // 회전
        controller.RotateToTarget();
        // 총알 발사
        controller.FireBullet();
        // 공격한 시간 저장
        lastAttackTime = Time.time;
        // 애니메이션 변경
        controller.ChangeAnimation(AnimState.Attack);
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 추격 거리 확인 함수
    private BT_NodeStatus CheckChaseDistance()
    {
        return CheckDistance(player.position, controller.Stat.MaxChaseDistance);
    }

    // 플레이어 추격 함수
    private BT_NodeStatus ChasePlayer()
    {
        // 플레이어와의 거리가 공격 거리보다 작거나 같다면
        if (CalculateTargetDistance(player.position) <= controller.Stat.MaxAttackDistance)
            // 성공 반환
            return BT_NodeStatus.Success;

        // 복귀가 필요없는 상태였다면
        if (!needToReturn)
            // 복귀가 필요한 상태로 변경
            needToReturn = true;

        // 타겟 설정
        controller.SetTarget(player);
        // 회전
        controller.RotateToTarget();
        // 이동
        controller.MoveToTarget();
        // 애니메이션 변경
        controller.ChangeAnimation(AnimState.Move);

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 복귀 거리 확인 함수
    private BT_NodeStatus CheckReturnDistance()
    {
        // 복귀 지점과의 거리 저장
        float distance = CalculateTargetDistance(returnPoint.position);
        // 복귀 지점과의 거리가 최대 복귀 거리보다 크거나 같고, 복귀가 필요한 상태라면 ? 성공 반환 : 실패 반환
        return (distance >= controller.Stat.MaxReturnDistance && needToReturn) ?
                                    BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 복귀 함수
    private BT_NodeStatus MoveToReturnPoint()
    {
        // 복귀가 필요없는 상태라면
        if (!needToReturn)
            // 실패 반환
            return BT_NodeStatus.Failure;

        // 타겟 설정
        controller.SetTarget(returnPoint);
        // 회전
        controller.RotateToTarget();
        // 이동
        controller.MoveToTarget();
        // 애니메이션 변경
        controller.ChangeAnimation(AnimState.Move);

        // 복귀 지점과의 거리가 최대 복귀 거리보다 작거나 같다면
        if (CalculateTargetDistance(returnPoint.position) <= controller.Stat.MaxReturnDistance)
        {
            // 복귀가 필요없는 상태로 변환
            needToReturn = false;
            // 순찰 지점 재설정 필요
            needToResetPatrolPoint = true;
            // 대기 시작 시간 저장
            startIdleTime = Time.time;
            // 타겟 초기화
            controller.ClearTarget();
            // 성공 반환
            return BT_NodeStatus.Success;
        }

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 순찰 지점 랜덤으로 정하는 함수
    private BT_NodeStatus SetRandomPatrolPoint()
    {
        // 순찰 지점 재설정이 필요한 상태라면
        if (needToResetPatrolPoint)
        {
            // 지름이 1인 원에서 뽑은 랜덤 위치에 최대 순찰 거리를 곱해서 저장
            Vector3 rand = Random.insideUnitSphere * controller.Stat.MaxPatrolDistance;
            // 높이 제거
            rand.y = 0f;
            // 순찰 지점의 위치를 복귀 지점의 위치에 랜덤 위치를 더한 값으로 설정
            patrolPoint.position = returnPoint.position + rand;
            // 순찰 지점 재설정 필요 없음
            needToResetPatrolPoint = false;
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
        // 타겟 지정
        controller.SetTarget(patrolPoint);
        // 회전
        controller.RotateToTarget();
        // 이동
        controller.MoveToTarget();
        // 애니메이션 변경
        controller.ChangeAnimation(AnimState.Move);

        // 순찰 지점과의 거리가 가깝다면
        if (CalculateTargetDistance(patrolPoint.position) <= 0.1f)
        {
            // 대기 시작 시간 저장
            startIdleTime = Time.time;
            // 타겟 초기화
            controller.ClearTarget();
            // 성공 반환
            return BT_NodeStatus.Success;
        }

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 도착 시간 확인 함수
    private BT_NodeStatus CheckIdleTime()
    {
        // 대기 시작 시간에서 대기 시간만큼 지나지 않았다면
        if (Time.time - startIdleTime <= controller.Stat.IdleTime)
            // 성공 반환
            return BT_NodeStatus.Success;
        // 대기 시간만큼 지났다면
        else
        {
            // 순찰 지점 재설정 필요
            needToResetPatrolPoint = true;
            // 실패 반환
            return BT_NodeStatus.Failure;
        }
    }

    // 대기 함수
    private BT_NodeStatus Idle()
    {
        // 애니메이션 재생
        controller.ChangeAnimation(AnimState.Idle);
        // 진행 중 반환
        return BT_NodeStatus.Running;
    }
}