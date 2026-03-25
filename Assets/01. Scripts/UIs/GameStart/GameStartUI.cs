using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour, IGameQuitCancelHandler
{
    [Header("정보")]
    [SerializeField] private SceneType bunkerScene;             // 벙커 씬
    [SerializeField] private SceneType playerCustomScene;       // 유저 꾸미기 씬   

    [Header("버튼")]
    [SerializeField] private Button gameStartButton;            // 시작하기 버튼
    [SerializeField] private Button customizingButton;          // 꾸미기 버튼
    [SerializeField] private Button settingsButton;             // 설정 버튼
    [SerializeField] private Button gameQuitButton;             // 게임 종료 버튼

    [Header("UI")]
    [SerializeField] private GameObject settingsUI;             // 설정 UI
    [SerializeField] private GameObject gameQuitUI;             // 게임 종료 UI

    private void Awake()
    {
        // 초기화
        if (gameStartButton != null)
            gameStartButton.onClick.AddListener(GameStart);

        if (customizingButton != null)
            customizingButton.onClick.AddListener(PlayerCustomizing);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(SettingsGame);

        if (gameQuitButton != null)
            gameQuitButton.onClick.AddListener(QuitGame);

        EventSystem.current.SetSelectedGameObject(gameStartButton.gameObject);
        // 게임 종료 취소 이벤트 구독
        Subject<IGameQuitCancelHandler>.Attach(this);
    }

    private void OnDestroy()
    {
        // 게임 종료 취소 이벤트 구독 해제
        Subject<IGameQuitCancelHandler>.Detach(this);
    }

    // 게임 시작 함수
    private void GameStart()
    {
        // 등록부에 다음 씬 이름이 등록되어 있다면
        if (SceneRegistry.GetSceneName(bunkerScene, out string sceneName))
            // 벙커 씬 전환
            SceneManager.LoadScene(sceneName);
        // 등록부에 다음 씬 이름이 등록되어 있지 않다면
        else
            Debug.LogError($"[Error] {bunkerScene}이 Scene Registry에 등록되지 않음");
    }

    // 플레이어 꾸미기 함수
    private void PlayerCustomizing()
    {
        // 등록부에 다음 씬 이름이 등록되어 있다면
        if (SceneRegistry.GetSceneName(playerCustomScene, out string sceneName))
            // 플레이어 꾸미기 씬 전환
            SceneManager.LoadScene(sceneName);
        // 등록부에 다음 씬 이름이 등록되어 있지 않다면
        else
            Debug.LogError($"[Error] {playerCustomScene}이 Scene Registry에 등록되지 않음");
    }

    // 게임 설정 함수
    private void SettingsGame()
    {
        Debug.Log("설정");
    }

    // 게임 종료 함수
    private void QuitGame()
    {
        // 메인 버튼들 클릭 불가
        UpdateMainButtonState(false);
        // 게임 종료 UI 열기
        gameQuitUI.SetActive(true);
    }

    // 메인 버튼들 상태 갱신 함수
    private void UpdateMainButtonState(bool state)
    {
        gameStartButton.interactable = state;
        customizingButton.interactable = state;
        settingsButton.interactable = state;
        gameQuitButton.interactable = state;
    }

    // 게임 종료 취소 함수
    public void OnGameQuitCancel()
    {
        // 현재 선택 버튼을 게임 종료 버튼으로 변경
        EventSystem.current.SetSelectedGameObject(gameQuitButton.gameObject);
        // 메인 버튼들 클릭 가능
        UpdateMainButtonState(true);
    }
}