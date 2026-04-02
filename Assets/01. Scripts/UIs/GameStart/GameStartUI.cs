using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour, IPopupUIClosedHandler
{
    [Header("버튼")]
    [SerializeField] private Button gameStartButton;            // 시작하기 버튼
    [SerializeField] private Button settingsButton;             // 설정 버튼
    [SerializeField] private Button gameQuitButton;             // 게임 종료 버튼

    [Header("UI")]
    [SerializeField] private GameObject gameLevelUI;            // 게임 난이도 UI
    [SerializeField] private GameObject settingsUI;             // 설정 UI
    [SerializeField] private GameObject gameQuitUI;             // 게임 종료 UI

    private DuckovInputActions inputActions;                    // 인풋 시스템
    private InputAction closeAction;                            // 닫기 액션(ESC)
    private GameObject lastSelectedButton;                      // 마지막 선택 버튼
    private GameObject lastPopupUI;                             // 마지막 팝업 UI

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        Application.targetFrameRate = 30;

        // 초기화
        if (gameStartButton != null)
            gameStartButton.onClick.AddListener(GameStart);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(SettingsGame);

        if (gameQuitButton != null)
            gameQuitButton.onClick.AddListener(QuitGame);

        EventSystem.current.SetSelectedGameObject(gameStartButton.gameObject);
        inputActions = new DuckovInputActions();
        inputActions.GameStart.Enable();
        closeAction = inputActions.GameStart.Close;
        closeAction.performed += ClosedPopupUI;
        // 팝업 UI 닫기 이벤트 구독
        Subject<IPopupUIClosedHandler>.Attach(this);
    }

    private void OnDestroy()
    {
        inputActions.GameStart.Disable();
        closeAction.performed -= ClosedPopupUI;
        // 팝업 UI 닫기 이벤트 구독 해제
        Subject<IPopupUIClosedHandler>.Detach(this);
    }

    // 게임 시작 함수
    private void GameStart()
    {
        // 마지막 선택 버튼 저장
        lastSelectedButton = gameStartButton.gameObject;
        // 마지막 팝업 UI 저장
        lastPopupUI = gameLevelUI;
        // 메인 버튼들 클릭 불가
        UpdateMainButtonState(false);
        // 게임 난이도 UI 열기
        gameLevelUI.SetActive(true);
    }

    // 게임 설정 함수
    private void SettingsGame()
    {
        // 마지막 선택 버튼 저장
        lastSelectedButton = settingsButton.gameObject;
        // 마지막 팝업 UI 저장
        lastPopupUI = settingsUI;
        // 메인 버튼들 클릭 불가
        UpdateMainButtonState(false);
        // 설정 UI 열기
        settingsUI.SetActive(true);
    }

    // 게임 종료 함수
    private void QuitGame()
    {
        // 마지막 선택 버튼 저장
        lastSelectedButton = gameQuitButton.gameObject;
        // 마지막 팝업 UI 저장
        lastPopupUI = gameQuitUI;
        // 메인 버튼들 클릭 불가
        UpdateMainButtonState(false);
        // 게임 종료 UI 열기
        gameQuitUI.SetActive(true);
    }

    // 메인 버튼들 상태 갱신 함수
    private void UpdateMainButtonState(bool state)
    {
        gameStartButton.interactable = state;
        settingsButton.interactable = state;
        gameQuitButton.interactable = state;
    }

    // 팝업 UI 닫기 함수
    public void OnClosedPopupUI()
    {
        // 현재 선택 버튼을 마지막 선택 버튼으로 변경
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        // 메인 버튼들 클릭 가능
        UpdateMainButtonState(true);
    }

    // ESC로 팝업 UI 닫기 함수
    public void ClosedPopupUI(InputAction.CallbackContext context)
    {
        // 마지막 팝업 UI 닫기
        lastPopupUI.SetActive(false);
        // 현재 선택 버튼을 마지막 선택 버튼으로 변경
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        // 메인 버튼들 클릭 가능
        UpdateMainButtonState(true);
    }
}