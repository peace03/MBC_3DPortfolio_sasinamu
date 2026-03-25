using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameQuitUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button gameQuitButton;             // 게임 종료 버튼
    [SerializeField] private Button gameQuitCancelButton;       // 게임 종료 취소 버튼

    private void Awake()
    {
        // 초기화
        if (gameQuitButton != null)
            gameQuitButton.onClick.AddListener(QuitGame);

        if (gameQuitCancelButton != null)
            gameQuitCancelButton.onClick.AddListener(CancelGameQuit);
    }

    private void OnEnable()
    {
        // 초기화
        EventSystem.current.SetSelectedGameObject(gameQuitCancelButton.gameObject);
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

    // 게임 종료 취소
    private void CancelGameQuit()
    {
        // 게임 종료 UI 닫기
        gameObject.SetActive(false);
        // 게임 종료 취소 이벤트 발행
        Subject<IGameQuitCancelHandler>.Publish(h => h.OnGameQuitCancel());
    }
}