using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.ShadowCascadeGUI;

public class EnemyBT : MonoBehaviour
{
    private BT_Node root;                                   // BT의 뿌리(시작) 노드

    [Header("정보")]
    [SerializeField] private Transform target;              // 쫓아갈 대상
    [SerializeField] private float lastAttackTime;          // 마지막 공격 시간

    [Header("스탯")]
    [SerializeField] private float moveSpeed;               // 이동 속도
    [SerializeField] private float maxChaseDistance;        // 최대 추적 거리
    [SerializeField] private float attackPower;             // 공격력
    [SerializeField] private float attackDelayTime;         // 공격 딜레이 시간
    [SerializeField] private float maxAttackDistance;       // 최대 공격 거리

    private CharacterController cc;                         // 캐릭터 컨트롤러

    private void Awake()
    {
        root = new BT_Selector(new List<BT_Node>
        {
            // 공격
            new BT_Sequence(new List<BT_Node>
            {
                // 공격 딜레이 계산
                new BT_Action(CheckAttackDelayTime),
                // 최대 공격 거리 안에 플레이어가 있는 지 확인
                new BT_Action(CheckAttackDistance),
                // 공격
                new BT_Action(AttackPlayer)
            })
            // 추적
            // 제자리로 돌아가기
            // 돌아다니기(패트롤)
            // 대기
        });

        cc = GetComponent<CharacterController>();
    }

    // 공격 딜레이 시간 확인 함수
    private BT_NodeStatus CheckAttackDelayTime()
    {
        // 마지막 공격 시간에서 공격 딜레이 시간만큼 지나지 않았다면 ? 진행 중 반환 : 성공 반환
        return Time.time - lastAttackTime < attackDelayTime ? BT_NodeStatus.Running : BT_NodeStatus.Success;
    }

    // 공격 거리 확인 함수
    private BT_NodeStatus CheckAttackDistance()
    {
        // 플레이어와의 거리가 최대 공격 거리보다 작으면 ? 성공 반환 : 실패 반환
        return CalculateDistance() < maxAttackDistance ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }

    // 플레이어와의 거리 반환 함수
    private float CalculateDistance()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    // 플레이어 공격 함수
    private BT_NodeStatus AttackPlayer()
    {
        Debug.Log("플레이어 공격!");
        // 회전
        HandleRotate();
        // 성공 반환
        return BT_NodeStatus.Success;
    }

    // 회전 관리 함수
    private void HandleRotate()
    {
        // 방향 받아오기
        Vector3 dir = (target.position - transform.position).normalized;
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
    private void HandleMove()
    {
        // 플레이어와의 거리가 최대 공격 거리 안이라면
        if (CalculateDistance() < maxAttackDistance)
            // 종료
            return;

        //cc.Move();
    }
}