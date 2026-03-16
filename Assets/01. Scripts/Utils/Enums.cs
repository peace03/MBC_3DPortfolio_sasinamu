// 사격 모드
public enum FireMode
{
    Single,         // 단발
    Auto            // 연발
}

// 애니메이션 상태
public enum AnimState
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
    Game,           // 게임
    End             // 끝
}

// UI 종류
public enum UIType
{
    None,           // 열린 UI 없음
    GameOver,       // 게임오버 UI
    Box,            // 상자 UI
    Pause           // 일시정지 UI
}