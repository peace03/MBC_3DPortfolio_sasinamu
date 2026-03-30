using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] private Transform camera;

    private void OnEnable()
    {
        camera = Camera.main.transform;
    }

    private void Update()
    {
        // 체력 UI 방향을 카메라가 바라보는 방향으로
        transform.forward = camera.forward;
    }
}