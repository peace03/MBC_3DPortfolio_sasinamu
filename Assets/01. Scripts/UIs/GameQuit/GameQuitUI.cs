using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameQuitUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button gameQuitButton;             // 게임 종료 버튼
    [SerializeField] private Button closeButton;                // 닫기 버튼

    private void Awake()
    {
        // 초기화
        if (gameQuitButton != null)
            gameQuitButton.onClick.AddListener(QuitGame);

        if (closeButton != null)
            closeButton.onClick.AddListener(CancelGameQuit);
    }

    private void OnEnable()
    {
        // 초기화
        EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
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

    // 닫기 버튼 함수
    private void CancelGameQuit()
    {
        // 게임 종료 UI 닫기
        gameObject.SetActive(false);
        // 팝업 UI 닫기 이벤트 발행
        Subject<IPopupUIClosedHandler>.Publish(h => h.OnClosedPopupUI());
    }
}