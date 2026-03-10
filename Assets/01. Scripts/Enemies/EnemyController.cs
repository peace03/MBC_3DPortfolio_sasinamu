using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private Transform target;                  // 목표
    [SerializeField] private Transform firePoint;               // 발사 위치

    [Header("스탯")]
    [SerializeField] private float moveSpeed;                   // 이동 속도

    private Dictionary<AnimState, int> animHashDictionary       // 애니메이션 해시 값
                        = new Dictionary<AnimState, int>();

    private CharacterController cc;                             // 적 캐릭터 컨트롤러
    private Animator anim;                                      // 적 애니메이터

    private AnimState curAnimState;                             // 현재 애니메이션 상태
    public AnimState CurAnimState => curAnimState;
    public AnimatorStateInfo AnimInfo                           // 애니메이터 정보
        => anim.GetCurrentAnimatorStateInfo(0);

    private void Awake()
    {
        // 초기화
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        animHashDictionary.Add(AnimState.Idle, Animator.StringToHash("Idle"));
        animHashDictionary.Add(AnimState.Move, Animator.StringToHash("Move"));
        animHashDictionary.Add(AnimState.Damaged, Animator.StringToHash("Damaged"));
        curAnimState = AnimState.Idle;
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
        cc.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    // 애니메이션 변경 함수
    public void ChangeAnimation(AnimState state)
    {
        // 현재 애니메이션이랑 같다면
        if (curAnimState == state)
            // 종료
            return;

        // 현재 애니메이션 상태의 해시 값이 있다면
        if(animHashDictionary.TryGetValue(curAnimState, out var curAnim))
            // 애니메이션 종료
            anim.SetBool(curAnim, false);

        // 변경 애니메이션 상태의 해시 값이 있다면
        if(animHashDictionary.TryGetValue(state, out var newAnim))
        {
            // 애니메이션 실행 상태
            anim.SetBool(newAnim, true);
            // 애니메이션 0초부터 실행
            anim.Play(newAnim, 0, 0f);
            // 현재 애니메이션 상태 변경
            curAnimState = state;
        }
    }

    // 피격 애니메이션 재생 함수
    public void PlayDamagedAnimation()
    {
        // 피격 상태임
        anim.SetBool("Damaged", true);
        // 피격 애니메이션 0초부터 실행
        anim.Play("Damaged", 0, 0f);
        // 현재 애니메이션 상태를 피격 상태로 변경
        curAnimState = AnimState.Damaged;
    }

    // 총알 발사 함수
    public void FireBullet(float attackPower, int angle)
    {
        // 방향 저장
        Vector3 dir = (target.position - firePoint.position).normalized;
        // 플레이어 방향에 대한 각도 저장
        Quaternion playerRot = Quaternion.LookRotation(dir);
        // 탄 퍼짐 값 저장
        float randomAngle = Random.Range(-angle, angle);
        // 탄 퍼짐 각도 저장
        Quaternion spreadRot = Quaternion.Euler(0, randomAngle, 0);
        // 플레이어 방향 각도에 탄 퍼짐 각도 더하기
        Quaternion finalRot = playerRot * spreadRot;

        Bullet bullet = SystemFacade.instance.BulletFactory.LargeBulletPool.Get();
        bullet.transform.SetPositionAndRotation(firePoint.position, finalRot);
        bullet.GetComponent<Bullet>().SetBulletInfo(new DamagedEvent(gameObject, attackPower));
    }
}