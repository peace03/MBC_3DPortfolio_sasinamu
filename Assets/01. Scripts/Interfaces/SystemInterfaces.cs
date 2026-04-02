using System.Collections.Generic;
using UnityEngine;

// 팝업 UI 닫기 관리 인터페이스
public interface IPopupUIClosedHandler
{
    public void OnClosedPopupUI();
}

// 상호작용이 가능한 대상 인터페이스
public interface IInteractable
{
    // 상호작용 함수
    public void Interact();
}

// 데미지 입을 수 있는 대상 인터페이스
public interface IDamageable
{
    // 데미지 반영 함수
    public void Damaged(string name, float amount);
}

// 적 죽음 관리 인터페이스
public interface IEnemyDeadHandler
{
    public void OnEnemyDead(GameObject prefab, Vector3 position);
}

// 적 일시정지 관리 인터페이스
public interface IEnemyPauseHandler
{
    public void OnEnemyPause(bool state);
}

// 일시정지 관리 인터페이스
public interface IGamePauseHandler
{
    public void OnGamePause();
}

// 버튼으로 열리는 UI 관리 인터페이스
public interface IOpenUIByButtonHandler
{
    public void OnOpenUIByButton(UIType type);
}

public interface IDeadUIHandler
{
    public void OnDeadUI(string name);
}

// UI 상태 관리 인터페이스
public interface IUIStateHandler
{
    public void OnUIState(bool state);
}

// 상자 관리 인터페이스
public interface IBoxHandler
{
    public void OnBox(InventoryModel boxModel);
}

// 가방 관리 인터페이스
public interface IInventoryHandler
{
    public void OnInventory();
}

// 지도 관리 인터페이스
public interface IMapHandler
{
    public void OnMap();
}

// 조작설명 관리 인터페이스
public interface IControlManualHandler
{
    public void OnControlManual();
}

public interface IQuickSlotStateHandler
{
    public void OnQuickSlotState(bool state);
}

public interface IEquipmentSlotHandler
{
    public void OnEquipmentSlot(InventoryModel model);
}

//작업대 인터페이스
public interface ICraftingHandler { void OnCraftingTable(); }

//열쇠 가공기 인터페이스
public interface IKeyMakerHandler { void OnKeyMaker(); }

public interface IQuickSlotHandler
{
    public void OnQuickSlot(InventoryModel model);
}

public interface IInventoryWeightHandler
{
    public void OnInventoryWeight(float weight);
}

public interface ICameraHandler
{
    public void OnCamera(bool state);
}

public interface IStageClearHandler
{
    public void OnStageClear(int stage);
}

public interface IItemDataHandler
{
    public void OnItemData(List<ItemData> datas);
}

public interface IProgressUIHandler
{
    public void OnStartProgress(ProgressType type, float value);
    public void OnCancelProgress();
}

public interface IFireModeUIHandler
{
    public void OnFireModeUI(FireMode mode);
}

public interface IStaminaUIHandler
{
    public void OnStaminaUI(float value);
}