using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemFacade : MonoBehaviour
{
    private SceneRegistry sceneRegistry;                            // 씬 등록부

    private void Awake()
    {
        sceneRegistry = new SceneRegistry();
    }

    // 씬 전환 함수
    public void ChangeScene(SceneType scene)
    {
        // 존재하는 씬이라면
        if (sceneRegistry.GetSceneName(scene, out string sceneName))
        {
            // 나중에 UI도 해야함

            // 씬 로드
            SceneManager.LoadScene(sceneName);
        }
        else
            Debug.LogError($"[Error] {scene}은 등록하지 않는 Scene입니다.");
    }
}