using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCollider : MonoBehaviour
{
    [SerializeField] private SceneType nextSceneType;
    [SerializeField] private float loadingTime;

    private Coroutine loadingCoroutine;

    private float curLoadingTime;

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

            Debug.Log($"씬 전환 진행 시간 : {(Time.time - curLoadingTime):F1}초");

            yield return null;
        }

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

    }
}