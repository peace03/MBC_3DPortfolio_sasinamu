// 플레이어 달리기 관리 인터페이스
public interface IPlayerRunHandler
{
    public void OnRun(bool isPressed);
}

// 플레이어 구르기 관리 인터페이스
public interface IPlayerRollHandler
{
    public void OnRoll();
}

// 플레이어 상호작용 관리 인터페이스
public interface IPlayerInteractHandler
{
    public void OnInteract();
}

// 플레이어 총알 발사 관리 인터페이스
public interface IPlayerFireHandler
{
    public void OnFire(bool isPressed);
}

// 플레이어 발사 모드 변경 관리 인터페이스
public interface IPlayerFireModeHandler
{
    public void OnFireMode();
}

// 플레이어 재장전 관리 인터페이스
public interface IPlayerReloadHandler
{
    public void OnReload();
}

// 플레이어 퀵슬롯 관리 인터페이스
public interface IPlayerQuickSlotHandler
{
    public void OnQuickSlot(int slotNumber);
}