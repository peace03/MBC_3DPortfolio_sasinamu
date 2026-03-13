using UnityEngine;

public class SceneChanger : MonoBehaviour, IInteractable
{
    [Header("불러올 Scene 정보")]
    [SerializeField] private SceneType nextScene;        // 다음 씬

    // 상호작용 함수 => 씬 전환
    public void Interact() => SystemFacade.instance.ChangeScene(nextScene);
}