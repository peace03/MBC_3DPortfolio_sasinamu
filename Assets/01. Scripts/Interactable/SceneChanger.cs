using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour, IInteractable, IPlayerCancelHandler
{
    [Header("정보")]
    [SerializeField] private float startChangeTime;     // 전환 시작 시간
    [SerializeField] private float changeDelayTime;     // 전환 지연 시간
    [SerializeField] private SceneType nextScene;       // 다음 씬

    private float ProgressRatio
    {
        get
        {
            if (changeDelayTime <= 0)
                return 0f;

            return Mathf.Clamp01((Time.time - startChangeTime) / changeDelayTime);
        }
    }

    private Coroutine sceneChangeCoroutine;             // 씬 전환 코루틴

    private void OnEnable()
    {
        // 취소 이벤트 구독
        Subject<IPlayerCancelHandler>.Attach(this);
    }

    private void OnDisable()
    {
        // 취소 이벤트 구독 해제
        Subject<IPlayerCancelHandler>.Detach(this);
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

    // 씬 전환 코루틴 함수
    private IEnumerator SceneChangeCoroutine()
    {
        // 전환 시작 시간에서 전환 지연 시간만큼 지날 때까지
        while (Time.time - startChangeTime < changeDelayTime)
        {
            // 게임 시간이 멈춰있다면
            if (Time.timeScale == 0)
            {
                // 프레임 단위로 기다리기
                yield return null;
                // 건너뛰기
                continue;
            }

            Subject<IProgressUIHandler>.Publish(h => h.OnStartProgress(ProgressType.ChangeScene, ProgressRatio));
            // 프레임 단위로 기다리기
            yield return null;
        }

        Subject<IProgressUIHandler>.Publish(h => h.OnStartProgress(ProgressType.ChangeScene, 1f));
        // 씬 전환 코루틴 초기화
        sceneChangeCoroutine = null;

        // 등록부에 다음 씬 이름이 등록되어 있다면
        if (SceneRegistry.GetSceneName(nextScene, out string sceneName))
            // 다음 씬 전환
            SceneManager.LoadScene(sceneName);
        // 등록부에 다음 씬 이름이 등록되어 있지 않다면
        else
            Debug.LogError($"[Error] {nextScene}이 Scene Registry에 등록되지 않음");
    }

    // 씬 전환 취소 함수
    public void OnCancel()
    {
        // 씬 전환 코루틴이 비어있다면
        if (sceneChangeCoroutine == null)
            // 종료
            return;

        // 씬 전환 코루틴 종료
        StopCoroutine(sceneChangeCoroutine);
        // 씬 전환 코루틴 초기화
        sceneChangeCoroutine = null;
        Subject<IProgressUIHandler>.Publish(h => h.OnCancelProgress());
    }
}