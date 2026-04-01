// 사격 모드
public enum FireMode
{
    Single,         // 단발
    Auto            // 연발
}

// 플레이어 애니메이션 상태
public enum PlayerAnimState
{
    Idle,           // 대기
    Walk            // 이동
}

// 적 애니메이션 상태
public enum EnemyAnimState
{
    Idle,           // 대기
    Move,           // 이동
    Attack,         // 공격
    Damaged,        // 피격
    Dead            // 죽음
}

// 씬 종류
public enum SceneType
{
    Start,          // 시작
    Bunker,         // 벙커
    Bunker_Y,         // 벙커_Y
    Game,           // 게임
    Game_Y,           // 게임_Y
    End,             // 끝
    InvenTest
}

// UI 종류
public enum UIType
{
    None,           // 없음
    GamePause,      // 일시정지 UI
    Settings,       // 세팅 UI
    GameQuit,       // 게임종료 UI
    GameOver,       // 게임오버 UI
    Inventory,      // 가방 UI
    Stat,           // 스탯 UI
    Map,            // 지도 UI
    Box,            // 상자 UI
    CraftingTable,  // 제작대 UI (추가)
    KeyMaker        // 열쇠가공기 UI (추가)
}