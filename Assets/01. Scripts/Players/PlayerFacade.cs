using UnityEngine;

public class PlayerFacade : MonoBehaviour
{
    public static PlayerFacade instance;

    [Header("매니저")]
    [SerializeField] private InputManager inputManager;     // 입력 매니저

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