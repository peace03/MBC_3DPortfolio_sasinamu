using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour, IStaminaUIHandler
{
    [SerializeField] private Slider staminaSlider;

    private void OnEnable()
    {
        Subject<IStaminaUIHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IStaminaUIHandler>.Detach(this);
    }

    public void OnStaminaUI(float value)
    {
        if (!staminaSlider.gameObject.activeSelf)
            staminaSlider.gameObject.SetActive(true);

        staminaSlider.value = value;

        if (value >= 1f)
            staminaSlider.gameObject.SetActive(false);
    }
}