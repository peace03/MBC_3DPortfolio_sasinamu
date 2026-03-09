using System.Collections.Generic;
using UnityEngine;

public class EnemyBT : MonoBehaviour
{
    private BT_Node root;                                   // BT의 뿌리(시작) 노드

    [Header("정보")]
    [SerializeField] private Transform player;              // 플레이어
    [SerializeField] private bool isPlayDamAnimation;       // 피격 애니메이션 재생 여부
    [SerializeField] private float lastAttackTime;          // 마지막 공격 시간
    [SerializeField] private bool needToReturn;             // 복귀 필요 여부
    [SerializeField] private Transform returnPoint;         // 복귀 지점
    [SerializeField] private bool isArrivalPoint;           // 지점 도착 여부
    [SerializeField] private Transform patrolPoint;         // 순찰 지점

    private EnemyController controller;                     // 적 컨트롤러(이동, 회전, 애니메이션 등)
    private EnemyStat stat;                                 // 적 스탯(체력, 공격력 등)

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
            new BT_Action(DamagedEnemy),
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
        isArrivalPoint = true;
        controller = GetComponent<EnemyController>();
        stat = GetComponent<EnemyStat>();
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
        return stat.IsDead ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 죽음 함수
    private BT_NodeStatus IsDead()
    {
        Debug.Log("죽음...");
        // 타겟 초기화
        controller.ClearTarget();
        // 애니메이션 변경

        // 적 오브젝트 비활성화
        gameObject.SetActive(false);
        // 죽음 이벤트 발생
        EventBus<DeadEvent>.Publish(new DeadEvent(false));
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 적 피격 함수
    private BT_NodeStatus DamagedEnemy()
    {
        // 피격 재생 중이 아니고 피격 상태가 아니라면
        if (!isPlayDamAnimation && !stat.IsDamaged)
            // 실패 반환
            return BT_NodeStatus.Failure;

        // 현재 애니메이션이 피격 애니메이션이 아니라면
        if (controller.CurAnimState != AnimState.Damaged)
        {
            // 애니메이션 변경
            controller.ChangeAnimation(AnimState.Damaged);
            // 피격 상태 초기화
            stat.ResetDamagedState();
            // 피격 애니메이션 재생 중
            isPlayDamAnimation = true;
            // 진행 중 반환
            return BT_NodeStatus.Running;
        }
        // 현재 애니메이션이 피격 애니메이션인데, 피격 상태가 되었다면
        else if (stat.IsDamaged)
        {
            // 피격 애니메이션 다시 실행
            controller.PlayDamagedAnimation();
            // 피격 상태 초기화
            stat.ResetDamagedState();
        }

        // 현재 애니메이션이 Damaged이고, 애니메이션 진행 시간이 끝나지 않았다면
        if (controller.AnimInfo.IsName("Damaged") && controller.AnimInfo.normalizedTime <= 1f)
            // 진행 중 반환
            return BT_NodeStatus.Running;

        // 피격 애니메이션 재생 끝
        isPlayDamAnimation = false;
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
        if (lastAttackTime != 0 && Time.time - lastAttackTime <= stat.AttackDelayTime)
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
        return CheckDistance(player.position, stat.MaxAttackDistance);
    }

    // 플레이어 공격 함수
    private BT_NodeStatus AttackPlayer()
    {
        // 회전
        controller.RotateToTarget();
        // 애니메이션 변경

        // 총알 발사(공격력, 퍼짐 각도)
        controller.FireBullet(stat.AttackPower, stat.MaxSpreadAngle);
        // 공격한 시간 저장
        lastAttackTime = Time.time;
        // 여기에 if문으로 애니메이션이 끝났을 때, 성공을 반환해야함

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 추격 거리 확인 함수
    private BT_NodeStatus CheckChaseDistance()
    {
        return CheckDistance(player.position, stat.MaxChaseDistance);
    }

    // 플레이어 추격 함수
    private BT_NodeStatus ChasePlayer()
    {
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

        // 플레이어와의 거리가 공격 거리보다 작거나 같다면
        if (CalculateTargetDistance(player.position) <= stat.MaxAttackDistance)
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
        return (distance >= stat.MaxReturnDistance && needToReturn) ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
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
        if (CalculateTargetDistance(returnPoint.position) <= stat.MaxReturnDistance)
        {
            // 복귀가 필요없는 상태로 변환
            needToReturn = false;
            // 복귀 지점 도착한 상태
            isArrivalPoint = true;
            // 타겟 초기화
            controller.ClearTarget();
            // 성공 반환
            return BT_NodeStatus.Success;
        }

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }

    // 대기 함수
    private BT_NodeStatus Idle()
    {
        // 복귀 혹은 순찰 지점에 도착한 상태라면
        if (isArrivalPoint)
        {
            // 현재 애니메이션 상태가 Idle이 아니라면
            if (controller.CurAnimState != AnimState.Idle)
            {
                // 애니메이션 재생
                controller.ChangeAnimation(AnimState.Idle);
                // 진행 중 반환
                return BT_NodeStatus.Running;
            }

            // 현재 애니메이션이 Idle이고, 애니메이션 진행 시간이 끝나지 않았다면
            if (controller.AnimInfo.IsName("Idle") && controller.AnimInfo.normalizedTime <= 1f)
                // 진행 중 반환
                return BT_NodeStatus.Running;
        }

        // 애니메이션 진행이 끝났다면 실패 반환
        return BT_NodeStatus.Failure;
    }

    // 순찰 지점 랜덤으로 정하는 함수
    private BT_NodeStatus SetRandomPatrolPoint()
    {
        // 순찰 지점에 도착했다면
        if (isArrivalPoint)
        {
            // 지름이 1인 원에서 뽑은 랜덤 위치에 최대 순찰 거리를 곱해서 저장
            Vector3 rand = Random.insideUnitSphere * stat.MaxPatrolDistance;
            // 높이 제거
            rand.y = 0f;
            // 순찰 지점의 위치를 복귀 지점의 위치에 랜덤 위치를 더한 값으로 설정
            patrolPoint.position = returnPoint.position + rand;
            // 순찰 지점에 도착하지 않은 상태
            isArrivalPoint = false;
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
            // 순찰 지점 도착 상태
            isArrivalPoint = true;
            // 타겟 초기화
            controller.ClearTarget();
            // 성공 반환
            return BT_NodeStatus.Success;
        }

        // 진행 중 반환
        return BT_NodeStatus.Running;
    }
}