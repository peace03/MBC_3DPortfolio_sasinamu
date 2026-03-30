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
    [SerializeField] private Button mainMenuButton;         // 메인 메뉴로 돌아가기 버튼

    private void Awake()
    {
        // 초기화
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void OnEnable()
    {
        // 초기화
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
}