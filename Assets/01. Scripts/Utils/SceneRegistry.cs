using System.Collections.Generic;

public class SceneRegistry
{
    private Dictionary<SceneType, string> sceneNames;       // 씬 이름들 딕셔너리
    
    // 생성자
    public SceneRegistry()
    {
        // 초기화
        sceneNames = new Dictionary<SceneType, string>
        {
            { SceneType.Start, "StartScene" },
            { SceneType.Bunker, "BunkerScene" },
            { SceneType.Game, "GameScene" },
            { SceneType.End, "EndScene" }
        };
    }

    // 씬 이름 반환 함수
    public bool GetSceneName(SceneType type, out string name)
    {
        // 없는 종류라면 false, null / 있다면 true, name
        return sceneNames.TryGetValue(type, out name);
    }
}