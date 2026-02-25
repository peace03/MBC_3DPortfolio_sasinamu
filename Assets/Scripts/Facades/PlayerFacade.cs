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

    public Vector2 GetInput()
    {
        return inputManager.GetMoveInput();
    }
}