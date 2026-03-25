using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private SceneType mainMenuScene;       // 메인 메뉴 씬

    [Header("버튼")]
    [SerializeField] private Button continueButton;         // 게임으로 돌아가기 버튼
    [SerializeField] private Button settingsButton;         // 설정 버튼
    [SerializeField] private Button mainMenuButton;         // 메인 메뉴로 돌아가기 버튼
    [SerializeField] private Button gameQuitButton;         // 게임 종료 버튼

    private void Awake()
    {
        // 초기화
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(SettingsGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (gameQuitButton != null)
            gameQuitButton.onClick.AddListener(QuitGame);
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
    }

    // 게임으로 돌아가기 함수
    private void ContinueGame()
    {
        // 일시정지 UI 닫기
        gameObject.SetActive(false);
        // 일시정지 이벤트 발생
        Subject<IGamePauseHandler>.Publish(h => h.OnGamePause());
    }

    // 게임 설정 함수
    private void SettingsGame()
    {
        Debug.Log("설정");
    }

    // 메인 메뉴로 돌아가기 함수
    private void ReturnToMainMenu()
    {
        // 등록부에 다음 씬 이름이 등록되어 있다면
        if (SceneRegistry.GetSceneName(mainMenuScene, out string sceneName))
        {
            // 게임 시간 시작
            Time.timeScale = 1f;
            // 메인 메뉴 씬 전환
            SceneManager.LoadScene(sceneName);
        }
        // 등록부에 다음 씬 이름이 등록되어 있지 않다면
        else
            Debug.LogError($"[Error] {mainMenuScene}이 Scene Registry에 등록되지 않음");
    }

    // 게임 종료 함수
    private void QuitGame()
    {
        // 유니티 에디터라면
        #if UNITY_EDITOR
            // 실행 종료
            UnityEditor.EditorApplication.isPlaying = false;
        // 그 외라면
        #else
            // 게임 종료
            Application.Quit();
        #endif
    }
}