using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private Transform target;                      // 목표
    [SerializeField] private Transform firePoint;                   // 발사 위치

    private CharacterController cc;                                 // 적 캐릭터 컨트롤러
    private Animator anim;                                          // 적 애니메이터
    private EnemyStat stat;                                         // 적 스탯(체력, 공격력 등)
    public EnemyStat Stat => stat;

    private Dictionary<AnimState, int> animHashDictionary           // 애니메이션 해시 값 딕셔너리
                                                    = new();

    private AnimState curAnimState;                                 // 현재 애니메이션 상태
    public AnimState CurAnimState => curAnimState;

    public bool IsDead => stat.IsDead;                              // 죽음 여부
    public bool IsDamaged => stat.IsDamaged;                        // 피격 여부

    private void Awake()
    {
        // 초기화
        stat = GetComponent<EnemyStat>();
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        InitAnimationHashDictionary();
        curAnimState = AnimState.Idle;
    }

    // 애니메이션 해시 값 딕셔너리 초기화 함수
    private void InitAnimationHashDictionary()
    {
        // 대기
        animHashDictionary.Add(AnimState.Idle, Animator.StringToHash("Idle"));
        // 이동
        animHashDictionary.Add(AnimState.Move, Animator.StringToHash("Move"));
        // 피격
        animHashDictionary.Add(AnimState.Damaged, Animator.StringToHash("Damaged"));
        // 죽음
        animHashDictionary.Add(AnimState.Dead, Animator.StringToHash("Dead"));
    }

    // 타겟 설정 함수
    public void SetTarget(Transform trg)
    {
        // 같은 타겟이라면
        if(trg == target)
            // 종료
            return;

        // 타겟 설정
        target = trg;
    }

    // 타겟 초기화 함수
    public void ClearTarget() => target = null;

    // 회전 함수
    public void RotateToTarget()
    {
        // 방향 저장
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

    // 이동 함수
    public void MoveToTarget()
    {
        // 애니메이션 변경
        ChangeAnimation(AnimState.Move);
        // 이동
        cc.Move(transform.forward * stat.MoveSpeed * Time.deltaTime);
    }

    // 애니메이션 변경 함수
    public void ChangeAnimation(AnimState state)
    {
        // 현재 애니메이션이랑 같다면
        if (curAnimState == state)
            // 종료
            return;

        // 변경 애니메이션 상태의 해시 값이 있다면
        if(animHashDictionary.TryGetValue(state, out var newAnim))
        {
            // 애니메이션 실행 상태
            anim.SetTrigger(newAnim);
            // 애니메이션 0초부터 실행
            anim.Play(newAnim, 0, 0f);
            // 현재 애니메이션 상태 변경
            curAnimState = state;
        }
    }

    // 피격 애니메이션 재생 함수
    public void ReplayDamagedAnimation()
    {
        // 피격 상태임
        anim.SetTrigger("Damaged");
        // 피격 애니메이션 0초부터 실행
        anim.Play("Damaged", 0, 0f);
    }

    // 총알 발사 함수
    public void FireBullet()
    {
        // 방향 저장
        Vector3 dir = (target.position - firePoint.position).normalized;
        // 플레이어 방향에 대한 각도 저장
        Quaternion playerRot = Quaternion.LookRotation(dir);
        // 탄 퍼짐 값 저장
        float randomAngle = Random.Range(-stat.MaxSpreadAngle, stat.MaxSpreadAngle);
        // 탄 퍼짐 각도 저장
        Quaternion spreadRot = Quaternion.Euler(0, randomAngle, 0);
        // 플레이어 방향 각도에 탄 퍼짐 각도 더하기
        Quaternion finalRot = playerRot * spreadRot;
        // L형 총알 받아오기
        Bullet bullet = SystemFacade.instance.BulletFactory.LargeBulletPool.Get();
        // 총알 정보, 위치, 각도 설정 후 총알 발사
        bullet.FireBullet(new DamagedEvent(stat, stat.AttackPower), firePoint.position, finalRot);
    }
}