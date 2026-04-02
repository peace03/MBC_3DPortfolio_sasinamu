using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCollider : MonoBehaviour
{
    [SerializeField] private SceneType nextSceneType;
    [SerializeField] private float loadingTime;

    private Coroutine loadingCoroutine;

    private float curLoadingTime;

    private float ProgressRatio
    {
        get
        {
            if (loadingTime <= 0f)
                return 0f;

            return Mathf.Clamp01((Time.time - curLoadingTime) / loadingTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (loadingCoroutine != null)
            return;

        curLoadingTime = Time.time;
        loadingCoroutine = StartCoroutine(SceneChangeLoadingCoroutine());
    }

    private void OnTriggerExit(Collider other)
    {
        CancelSceneChangeCoroutine();
    }

    private IEnumerator SceneChangeLoadingCoroutine()
    {
        while (Time.time - curLoadingTime < loadingTime)
        {
            if (Time.timeScale == 0)
            {
                yield return null;
                continue;
            }

            Subject<IProgressUIHandler>.Publish(h => h.OnStartProgress(ProgressType.ChangeScene, ProgressRatio));
            yield return null;
        }

        Subject<IProgressUIHandler>.Publish(h => h.OnStartProgress(ProgressType.ChangeScene, 1f));
        loadingCoroutine = null;

        if (SceneRegistry.GetSceneName(nextSceneType, out string sceneName))
            SceneManager.LoadScene(sceneName);
        else
            Debug.LogError($"[Error] {nextSceneType}이 Scene Registry에 등록되지 않음");
    }

    private void CancelSceneChangeCoroutine()
    {
        if (loadingCoroutine == null)
            return;

        StopCoroutine(loadingCoroutine);
        loadingCoroutine = null;
        Subject<IProgressUIHandler>.Publish(h => h.OnCancelProgress());
    }
}