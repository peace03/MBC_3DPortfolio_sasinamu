using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuBarUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button invButton;              // 가방 버튼
    [SerializeField] private Button statButton;             // 스탯 버튼
    [SerializeField] private Button mapButton;              // 지도 버튼
    [SerializeField] private Button settingsButton;         // 설정 버튼

    [Header("UI")]
    [SerializeField] private GameObject inventoryUI;        // 가방 UI
    [SerializeField] private GameObject statUI;             // 스탯 UI
    [SerializeField] private GameObject mapUI;              // 지도 UI
    [SerializeField] private GameObject settingsUI;         // 설정 UI

    private UIType curSubUI;                                // 최근 서브 UI

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

    // 메뉴 바 UI 갱신 함수
    public void UpdateMenuBarUI(UIType type)
    {
        // 메뉴 버튼들 변경
        UpdateMenuButtons(type);
        // 서브 UI 변경
        OpenSubUI(type);
    }

    // 메뉴 버튼들 갱신 함수
    private void UpdateMenuButtons(UIType type)
    {
        // UI 종류에 따라서
        switch (type)
        {
            // 장비라면
            case UIType.Inventory:
                EventSystem.current.SetSelectedGameObject(invButton.gameObject);
                break;
            // 스탯이라면
            case UIType.Stat:
                EventSystem.current.SetSelectedGameObject(statButton.gameObject);
                break;
            // 지도라면
            case UIType.Map:
                EventSystem.current.SetSelectedGameObject(mapButton.gameObject);
                break;
            // 설정이라면
            case UIType.Settings:
                EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
                break;
        }
    }

    // 서브 UI 닫기 함수
    private void CloseSubUI()
    {
        // UI 종류에 따라서
        switch (curSubUI)
        {
            case UIType.Inventory:
                inventoryUI.SetActive(false);
                break;
            case UIType.Stat:
                statUI.SetActive(false);
                break;
            case UIType.Map:
                mapUI.SetActive(false);
                break;
            case UIType.Settings:
                settingsUI.SetActive(false);
                break;
        }
    }

    // 서브 UI 열기 함수
    private void OpenSubUI(UIType type)
    {
        // 이전 UI 닫기
        CloseSubUI();
        // 현재 서브 UI 변경
        curSubUI = type;

        // UI 종류에 따라서
        switch (type)
        {
            case UIType.Inventory:
                inventoryUI.SetActive(true);
                break;
            case UIType.Stat:
                statUI.SetActive(true);
                break;
            case UIType.Map:
                mapUI.SetActive(true);
                break;
            case UIType.Settings:
                settingsUI.SetActive(true);
                break;
        }

        Subject<IQuickSlotStateHandler>.Publish(h => h.OnQuickSlotState(curSubUI == UIType.Inventory));
        Subject<ICameraHandler>.Publish(h => h.OnCamera(curSubUI == UIType.Map));
    }

    // 가방 버튼 함수
    private void InventoryButton() => UpdateMenuBarUI(UIType.Inventory);

    // 스탯 버튼 함수
    private void StatButton() => UpdateMenuBarUI(UIType.Stat);

    // 지도 버튼 함수
    private void MapButton() => UpdateMenuBarUI(UIType.Map);

    // 설정 버튼 함수
    private void SettingsButton() => UpdateMenuBarUI(UIType.Settings);
}