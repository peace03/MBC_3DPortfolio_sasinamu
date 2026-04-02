using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour, IProgressUIHandler
{
    [Header("정보")]
    [SerializeField] private GameObject progressUI;
    [SerializeField] private ProgressType curProgressType;
    [SerializeField] private Slider progressSlider;

    private float curProgressValue = 0f;

    private void OnEnable()
    {
        Subject<IProgressUIHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IProgressUIHandler>.Detach(this);
    }

    public void OnStartProgress(ProgressType type, float value)
    {
        if (curProgressType == ProgressType.None)
            curProgressType = type;
        else if (curProgressType != type)
        {
            Debug.Log($"{curProgressType}이 진행 중입니다.");
            return;
        }

        curProgressValue = value;
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        if (!progressUI.activeSelf)
            progressUI.SetActive(true);

        progressSlider.value = curProgressValue;

        if (curProgressValue >= 1f)
        {
            curProgressValue = 0f;
            progressSlider.value = 0f;
            progressUI.SetActive(false);
        }
    }

    public void OnCancelProgress()
    {
        if (!progressUI.activeSelf)
            return;

        curProgressType = ProgressType.None;
        progressSlider.value = 0f;
        curProgressValue = 0f;
        progressUI.SetActive(false);
    }
}