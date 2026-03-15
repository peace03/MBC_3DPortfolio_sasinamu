using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button continueBtn;     // 계속하기 버튼

    private void Awake()
    {
        // 계속하기 버튼이 버이있지 않다면
        if (continueBtn != null)
            // 계속하기 버튼에 클릭 기능 추가
            continueBtn.onClick.AddListener(ContinueGame);
    }

    // 게임 계속하기 함수
    private void ContinueGame()
    {
        Debug.Log("게임 계속하기");
        EventBus<UIStateEvent>.Publish(new UIStateEvent(false));
        SceneManager.LoadScene("BunkerScene");
    }
}