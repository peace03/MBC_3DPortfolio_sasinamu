using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLevelUI : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private SceneType bunkerScene;             // 벙커 씬

    [Header("버튼")]
    [SerializeField] private Button easyLevelButton;            // 쉬움 난이도 버튼
    [SerializeField] private Button hardLevelButton;            // 어려움 난이도 버튼

    private void Awake()
    {
        // 초기화
        if (easyLevelButton != null)
            easyLevelButton.onClick.AddListener(SelectEasyLevel);

        if (hardLevelButton != null)
            hardLevelButton.onClick.AddListener(SelectHardLevel);
    }

    private void OnEnable()
    {
        // 게임 난이도 UI 버튼 초기화 코루틴 시작
        StartCoroutine(InitGameLevelUIButton());
    }

    // 쉬움 난이도 선택 함수
    private void SelectEasyLevel()
    {
        Debug.Log("쉬움 난이도 시작");
        // 게임 시작
        StartGame();
    }

    // 게임 시작 함수
    private void StartGame()
    {
        // 등록부에 다음 씬 이름이 등록되어 있다면
        if (SceneRegistry.GetSceneName(bunkerScene, out string sceneName))
        {
            // 선택 초기화
            EventSystem.current.SetSelectedGameObject(null);
            // 벙커 씬 전환
            SceneManager.LoadScene(sceneName);
        }
        // 등록부에 다음 씬 이름이 등록되어 있지 않다면
        else
            Debug.LogError($"[Error] {bunkerScene}이 Scene Registry에 등록되지 않음");
    }

    // 어려움 난이도 선택 함수
    private void SelectHardLevel()
    {
        Debug.Log("어려움 난이도 시작");
        // 게임 시작
        StartGame();
    }

    // 게임 난이도 UI 버튼 초기화 함수
    private IEnumerator InitGameLevelUIButton()
    {
        // 프레임 단위로 기다리기
        yield return null;
        // 쉬움 난이도 버튼으로 초기화
        EventSystem.current.SetSelectedGameObject(easyLevelButton.gameObject);
    }
}