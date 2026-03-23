using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationChanger : MonoBehaviour
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

    private void InitAnimationHashDictionary()
    {
        // 대기
        animHashDictionary.Add(PlayerAnimState.Idle, Animator.StringToHash("Idle"));
        // 걷기
        animHashDictionary.Add(PlayerAnimState.Walk, Animator.StringToHash("Walk"));
    }

    public void ChangePlayerAnimation(PlayerAnimState state)
    {
        if (curAnimState == state)
            return;

        if (animHashDictionary.TryGetValue(state, out var newAnim))
        {
            anim.SetTrigger(newAnim);
            curAnimState = state;
        }
    }
}