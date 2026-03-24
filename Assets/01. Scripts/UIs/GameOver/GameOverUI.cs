using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button continueButton;     // 계속하기 버튼

    private void Awake()
    {
        // 계속하기 버튼이 비어있지 않다면
        if (continueButton != null)
            // 계속하기 버튼에 클릭 기능 추가
            continueButton.onClick.AddListener(ContinueGame);
    }

    // 게임 계속하기 함수
    private void ContinueGame()
    {
        // 게임 오버 UI 닫기
        gameObject.SetActive(false);
        // UI가 닫힌 상태임
        Subject<IUIStateHandler>.Publish(h => h.OnUIState(false));
        // 벙커 씬으로 이동
        SceneManager.LoadScene("BunkerScene");
    }
}