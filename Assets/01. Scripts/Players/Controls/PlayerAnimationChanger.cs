using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationChanger : MonoBehaviour, IPlayerDeadHandler
{
    [Header("정보")]
    [SerializeField] private PlayerAnimState curAnimState;                      // 현재 애니메이션 상태

    private Animator anim;                                                      // 플레이어 애니메이터

    private Dictionary<PlayerAnimState, int> animHashDictionary = new();        // 플레이어 애니메이션 해시 값 딕셔너리

    private void Awake()
    {
        // 초기화
        anim = GetComponentInChildren<Animator>();
        InitAnimationHashDictionary();
    }

    // 플레이어 애니메이션 해시 값 초기화 함수
    private void InitAnimationHashDictionary()
    {
        // 대기
        animHashDictionary.Add(PlayerAnimState.Idle, Animator.StringToHash("Idle"));
        // 걷기
        animHashDictionary.Add(PlayerAnimState.Walk, Animator.StringToHash("Walk"));
        animHashDictionary.Add(PlayerAnimState.WalkWithWeapon, Animator.StringToHash("WalkWithWeapon"));
        animHashDictionary.Add(PlayerAnimState.Run, Animator.StringToHash("Run"));
        animHashDictionary.Add(PlayerAnimState.Roll, Animator.StringToHash("Roll"));
        animHashDictionary.Add(PlayerAnimState.Dead, Animator.StringToHash("Dead"));
    }

    // 플레이어 애니메이션 변경 함수
    public void ChangePlayerAnimation(PlayerAnimState state)
    {
        // 현재 애니메이션과 같다면
        if (curAnimState == state)
            // 종료
            return;

        // 애니메이션 해시 값이 있다면
        if (animHashDictionary.TryGetValue(state, out var newAnim))
        {
            // 애니메이션 변경
            anim.SetTrigger(newAnim);
            // 현재 애니메이션 상태 바꾸기
            curAnimState = state;
        }
    }

    public void OnPlayerDead(string killer)
    {
        ChangePlayerAnimation(PlayerAnimState.Dead);
        StartCoroutine(WaitForDeadAnimation(killer));
    }

    private IEnumerator WaitForDeadAnimation(string killer)
    {
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        Subject<IDeadUIHandler>.Publish(h => h.OnDeadUI(killer));
    }
}