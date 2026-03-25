using UnityEngine;
using UnityEngine.UI;

public class MenuBarUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button invButton;          // 가방 버튼
    [SerializeField] private Button statButton;         // 스탯 버튼
    [SerializeField] private Button mapButton;          // 지도 버튼
    [SerializeField] private Button settingsButton;     // 설정 버튼

    private void Awake()
    {
        if (invButton != null)
            invButton.onClick.AddListener(InventoryButton);

        if (statButton != null)
            statButton.onClick.AddListener(StatButton);

        if (mapButton != null)
            mapButton.onClick.AddListener(MapButton);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(SettingsButton);
    }

    // 메뉴 버튼들 갱신 함수
    public void UpdateMenuButtons(UIType type)
    {
        // 메뉴 버튼들 초기화
        invButton.interactable = true;
        statButton.interactable = true;
        mapButton.interactable = true;
        settingsButton.interactable = true;

        // UI 종류에 따라서
        switch (type)
        {
            // 장비라면
            case UIType.Inventory:
                // 장비 버튼 클릭 불가
                invButton.interactable = false;
                break;
            // 스탯이라면
            case UIType.Stat:
                // 스탯 버튼 클릭 불가
                statButton.interactable = false;
                break;
            // 지도라면
            case UIType.Map:
                // 지도 버튼 클릭 불가
                mapButton.interactable = false;
                break;
            // 설정이라면
            case UIType.Settings:
                // 설정 버튼 클릭 불가
                settingsButton.interactable = false;
                break;
        }
    }

    // 가방 버튼 함수
    private void InventoryButton()
    {
        // 메뉴 버튼들 갱신
        UpdateMenuButtons(UIType.Inventory);
        // 버튼으로 UI 열기 이벤트 발생
        Subject<IOpenUIByButtonHandler>.Publish(h => h.OnOpenUIByButton(UIType.Inventory));
    }

    // 스탯 버튼 함수
    private void StatButton()
    {
        // 메뉴 버튼들 갱신
        UpdateMenuButtons(UIType.Stat);
        // 버튼으로 UI 열기 이벤트 발생
        Subject<IOpenUIByButtonHandler>.Publish(h => h.OnOpenUIByButton(UIType.Stat));
    }

    // 지도 버튼 함수
    private void MapButton()
    {
        // 메뉴 버튼들 갱신
        UpdateMenuButtons(UIType.Map);
        // 버튼으로 UI 열기 이벤트 발생
        Subject<IOpenUIByButtonHandler>.Publish(h => h.OnOpenUIByButton(UIType.Map));
    }

    // 설정 버튼 함수
    private void SettingsButton()
    {
        // 메뉴 버튼들 갱신
        UpdateMenuButtons(UIType.Settings);
        // 버튼으로 UI 열기 이벤트 발생
        Subject<IOpenUIByButtonHandler>.Publish(h => h.OnOpenUIByButton(UIType.Settings));
    }
}