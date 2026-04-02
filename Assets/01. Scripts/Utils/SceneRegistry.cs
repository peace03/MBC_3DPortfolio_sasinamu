using System.Collections.Generic;

public static class SceneRegistry
{
    private static readonly Dictionary<SceneType, string> sceneNames;       // 씬 이름들 딕셔너리
    
    // 생성자
    static SceneRegistry()
    {
        // 초기화
        sceneNames = new Dictionary<SceneType, string>
        {
            { SceneType.Start, "StartScene" },
            { SceneType.Bunker, "BunkerScene" },
            { SceneType.Bunker_Y, "BunkerScene_Y" },
            { SceneType.Game, "GameScene" },
            { SceneType.Game_Y, "GameScene_Y" },
            { SceneType.End, "EndScene" },
            {SceneType.InvenTest, "InventoryTest" }
        };
    }

    // 씬 이름 반환 함수
    public static bool GetSceneName(SceneType type, out string name)
    {
        // 없는 종류라면 false, null / 있다면 true, name
        return sceneNames.TryGetValue(type, out name);
    }
}