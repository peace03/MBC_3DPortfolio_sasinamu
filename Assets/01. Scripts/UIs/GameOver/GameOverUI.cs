using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private SceneType bunkerScene;     // 벙커 씬
    [SerializeField] private GameObject playerStaminaUI;

    [Header("버튼")]
    [SerializeField] private Button continueButton;     // 계속하기 버튼

    private void OnEnable()
    {
        playerStaminaUI.SetActive(false);
    }

    private void OnDestroy()
    {
        playerStaminaUI.SetActive(true);
    }

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
        // 등록부에 다음 씬 이름이 등록되어 있다면
        if (SceneRegistry.GetSceneName(bunkerScene, out string sceneName))
        {
            // 게임 시간 시작
            Time.timeScale = 1f;
            // 벙커 씬 전환
            SceneManager.LoadScene(sceneName);
        }
        // 등록부에 다음 씬 이름이 등록되어 있지 않다면
        else
            Debug.LogError($"[Error] {bunkerScene}이 Scene Registry에 등록되지 않음");
    }
}