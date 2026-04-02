using System.Collections.Generic;
using UnityEngine;

public class Bunker : SceneChanger
{
    [Header("정보")]
    [SerializeField] private BunkerAnimState curAnimState;                      // 현재 애니메이션 상태

    private Animator anim;                                                      // 플레이어 애니메이터

    private Dictionary<BunkerAnimState, int> animHashDictionary = new();        // 플레이어 애니메이션 해시 값 딕셔너리

    private void Awake()
    {
        // 초기화
        anim = GetComponent<Animator>();
        InitAnimationHashDictionary();
    }

    // 애니메이션 해시 값 초기화 함수
    private void InitAnimationHashDictionary()
    {
        // 열기
        animHashDictionary.Add(BunkerAnimState.Open, Animator.StringToHash("Open"));
        // 닫기
        animHashDictionary.Add(BunkerAnimState.Close, Animator.StringToHash("Close"));
    }

    // 플레이어 애니메이션 변경 함수
    private void ChangeBunkerAnimation(BunkerAnimState state)
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

    private void OnTriggerEnter(Collider other)
    {
        ChangeBunkerAnimation(BunkerAnimState.Open);
    }

    private void OnTriggerExit(Collider other)
    {
        ChangeBunkerAnimation(BunkerAnimState.Close);
    }
}