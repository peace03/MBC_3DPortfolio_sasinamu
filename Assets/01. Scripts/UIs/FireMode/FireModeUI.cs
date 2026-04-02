using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FireModeUI : MonoBehaviour, IFireModeUIHandler
{
    [Header("정보")]
    [SerializeField] private GameObject fireModeUI;
    [SerializeField] private Text fireModeText;
    [SerializeField] private string singleFireModeText;
    [SerializeField] private string autoFireModeText;

    [Header("스탯")]
    [SerializeField] private float popupTime;

    private float curPopupTime;

    private void OnEnable()
    {
        Subject<IFireModeUIHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IFireModeUIHandler>.Detach(this);
    }

    public void OnFireModeUI(FireMode mode)
    {
        switch (mode)
        {
            case FireMode.Single:
                fireModeText.text = singleFireModeText;
                break;
            case FireMode.Auto:
                fireModeText.text = autoFireModeText;
                break;
            default:
                fireModeText.text = "";
                break;
        }

        curPopupTime = Time.time;
        StartCoroutine(PopupCoroutine());
    }

    private IEnumerator PopupCoroutine()
    {
        fireModeUI.SetActive(true);

        while (Time.time - curPopupTime < popupTime)
        {
            if (Time.timeScale == 0)
            {
                yield return null;
                continue;
            }

            yield return null;
        }

        fireModeUI.SetActive(false);
    }
}