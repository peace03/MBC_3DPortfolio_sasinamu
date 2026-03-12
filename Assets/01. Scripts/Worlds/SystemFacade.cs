using UnityEngine;

public class SystemFacade : MonoBehaviour
{
    public static SystemFacade instance;                        // 인스턴스

    [Header("매니저")]
    [SerializeField] private PlayerManager playerManager;       // 플레이어 매니저
    [SerializeField] private InputManager inputManager;         // 입력 매니저

    [Header("공장")]
    [SerializeField] private BulletFactory bulletFactory;       // 총알 공장
    public BulletFactory BulletFactory => bulletFactory;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // 마우스 위치 반환 함수
    public Vector2 GetMousePosition()
    {
        return inputManager.CurMousePos;
    }
}