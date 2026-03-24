using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationChanger : MonoBehaviour
{
    private Dictionary<EnemyAnimState, int> animHashDictionary = new();     // 애니메이션 해시 값 딕셔너리

    private Animator anim;                                                  // 적 애니메이터

    private EnemyAnimState curAnimState = EnemyAnimState.Idle;              // 현재 애니메이션 상태

    private void Awake()
    {
        // 초기화
        anim = GetComponentInChildren<Animator>();
        InitAnimationHashDictionary();
    }

    // 애니메이션 해시 값 딕셔너리 초기화 함수
    private void InitAnimationHashDictionary()
    {
        // 대기
        animHashDictionary.Add(EnemyAnimState.Idle, Animator.StringToHash("Idle"));
        // 이동
        animHashDictionary.Add(EnemyAnimState.Move, Animator.StringToHash("Move"));
        // 공격
        animHashDictionary.Add(EnemyAnimState.Attack, Animator.StringToHash("Attack"));
        // 피격
        animHashDictionary.Add(EnemyAnimState.Damaged, Animator.StringToHash("Damaged"));
        // 죽음
        animHashDictionary.Add(EnemyAnimState.Dead, Animator.StringToHash("Dead"));
    }

    // 애니메이션 변경 함수
    public void ChangeAnimation(EnemyAnimState state)
    {
        // 현재 애니메이션이랑 같다면
        if (curAnimState == state)
            // 종료
            return;

        // 변경 애니메이션 상태의 해시 값이 있다면
        if (animHashDictionary.TryGetValue(state, out var newAnim))
        {
            // 애니메이션 실행 상태
            anim.SetTrigger(newAnim);
            // 현재 애니메이션 상태 변경
            curAnimState = state;
        }
    }

    // 피격 애니메이션 재생 함수
    public void PlayDamagedAnimation()
    {
        // 피격 애니메이션 실행
        anim.SetTrigger("Damaged");
        // 현재 애니메이션 상태를 피격으로 변경
        curAnimState = EnemyAnimState.Damaged;
    }
}