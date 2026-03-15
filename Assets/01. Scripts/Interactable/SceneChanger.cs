using System.Collections;
using UnityEngine;

public class SceneChanger : MonoBehaviour, IInteractable
{
    [Header("정보")]
    [SerializeField] private float startChangeTime;     // 전환 시작 시간
    [SerializeField] private float changeDelayTime;     // 전환 딜레이 시간
    [SerializeField] private SceneType nextScene;       // 다음 씬

    private Coroutine sceneChangeCoroutine;             // 씬 전환 코루틴

    private void OnEnable()
    {
        // 중지 이벤트 구독
        EventBus<BreakEvent>.OnEvent += CancelSceneChange;
    }

    private void OnDisable()
    {
        // 중지 이벤트 구독 해제
        EventBus<BreakEvent>.OnEvent -= CancelSceneChange;
    }

    // 상호작용 함수
    public void Interact()
    {
        // 씬 전환 코루틴이 비어있지 않다면
        if (sceneChangeCoroutine != null)
            // 종료
            return;

        // 전환 시작 시간 저장
        startChangeTime = Time.time;
        // 씬 전환 코루틴 시작
        sceneChangeCoroutine = StartCoroutine(SceneChangeCoroutine());
    }

    // 씬 전환 취소 함수
    public void CancelSceneChange(BreakEvent data)
    {
        // 씬 전환 코루틴이 비어있다면
        if (sceneChangeCoroutine == null)
            // 종료
            return;

        // 씬 전환 코루틴 종료
        StopCoroutine(sceneChangeCoroutine);
        // 씬 전환 코루틴 초기화
        sceneChangeCoroutine = null;
        // 여기서 UI의 슬라이더 값 초기화 알려주기

    }

    // 씬 전환 코루틴 함수
    private IEnumerator SceneChangeCoroutine()
    {
        // 전환 시작 시간에서 전환 딜레이 시간만큼 지날 때까지
        while(Time.time - startChangeTime < changeDelayTime)
        {
            Debug.Log($"진행 시간 : {(Time.time - startChangeTime):F1}초");
            // 여기서 UI의 슬라이더 값 전달하기

            // 프레임 단위로 기다리기
            yield return null;
        }

        // 씬 전환
        SystemFacade.instance.ChangeScene(nextScene);
    }
}