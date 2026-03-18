using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform player;              // 플레이어

    [Header("매니저")]
    [SerializeField] private InputManager inputManager;     // 인풋 매니저

    [Header("카메라 제한")]
    [SerializeField] private float maxX;                    // 최대 좌우 값
    [SerializeField] private float maxZ;                    // 최대 상하 값

    private void LateUpdate()
    {
        // 플레이어 위치를 중심으로, 현재 마우스 위치에 따라 이동
        transform.position = player.position + CalculateMoveDistance();
    }

    // 이동 거리 계산 함수
    private Vector3 CalculateMoveDistance()
    {
        // 현재 마우스 위치의 비율 저장
        Vector2 mousePos = inputManager.GetMouseOffsetRatio();
        // 카메라의 정면 방향 저장
        Vector3 forward = Camera.main.transform.forward;
        // 카메라의 우측 방향 저장
        Vector3 right = Camera.main.transform.right;
        // 높이 제거
        forward.y = right.y = 0f;
        // 이동 거리 반환(카메라의 정면 * 현재 마우스 세로 비율 * 최대 세로 값 + 카메라의 우측 * 현재 마우스의 가로 비율 * 최대 가로 값)
        return forward.normalized * mousePos.y * maxZ + right.normalized * mousePos.x * maxX;
    }
}