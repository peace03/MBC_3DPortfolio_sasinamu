using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemFacade : MonoBehaviour
{
    public static SystemFacade instance;                            // 인스턴스

    [Header("매니저")]
    [SerializeField] private PlayerManager playerManager;           // 플레이어 매니저
    [SerializeField] private InputManager inputManager;             // 입력 매니저

    [Header("공장")]
    [SerializeField] private BulletFactory bulletFactory;           // 총알 공장
    public BulletFactory BulletFactory => bulletFactory;

    private SceneRegistry sceneRegistry;                            // 씬 등록부

    private void Awake()
    {
        // 객체가 있다면
        if (instance != null)
        {
            // 파괴
            Destroy(gameObject);
            // 종료
            return;
        }

        // 초기화
        instance = this;
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