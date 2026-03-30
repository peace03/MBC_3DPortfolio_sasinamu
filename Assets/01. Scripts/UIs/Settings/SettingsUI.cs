using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button languageRefreshButton;      // 언어 새로고침 버튼
    [SerializeField] private Button closeButton;                // 닫기 버튼

    [Header("슬라이더")]
    [SerializeField] private Slider backgroundSoundSlider;      // 배경음 슬라이더
    [SerializeField] private Slider effectSoundSlider;          // 효과음 슬라이더

    private void Awake()
    {
        // 초기화
        if (languageRefreshButton != null)
            languageRefreshButton.onClick.AddListener(RefreshLanguage);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosedUI);

        if (backgroundSoundSlider != null)
            backgroundSoundSlider.onValueChanged.AddListener(ChangedBackgroundSound);

        if (effectSoundSlider != null)
            effectSoundSlider.onValueChanged.AddListener(ChangedEffectSound);
    }

    // 언어 새로고침 함수
    private void RefreshLanguage()
    {
        Debug.Log("언어 새로고침 버튼 누름");
    }

    // 닫기 함수
    private void ClosedUI()
    {
        // 설정 UI 닫기
        gameObject.SetActive(false);
        // 팝업 UI 닫기 이벤트 발행
        Subject<IPopupUIClosedHandler>.Publish(h => h.OnClosedPopupUI());
    }

    // 배경음 조절 함수
    private void ChangedBackgroundSound(float value)
    {
        Debug.Log("배경음 변경");
    }

    // 효과음 조절 함수
    private void ChangedEffectSound(float value)
    {
        Debug.Log("효과음 변경");
    }
}