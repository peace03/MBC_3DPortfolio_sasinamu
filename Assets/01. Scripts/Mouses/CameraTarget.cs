using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform player;      // 플레이어

    [Header("카메라 제한")]
    [SerializeField] private float maxX;            // 최대 좌우 값
    [SerializeField] private float maxZ;            // 최대 상하 값

    private Camera mainCam;                         // 메인 카메라

    private void Awake()
    {
        // 초기화
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        // 마우스의 뷰 포트 값 가져오기
        Vector3 mousePos = GetMouseViewportPosition();
        // 이동할 방향 값 가져오기
        Vector3 moveDir = CalculateWorldMoveDirection(mousePos);
        // 이동
        transform.position = player.position + moveDir;
    }

    // 뷰 포트로 바꾼 마우스 위치 반환 함수
    private Vector3 GetMouseViewportPosition()
    {
        // 마우스 위치를 0(왼쪽, 아래) ~ 1(오른쪽, 위)의 값으로 변환
        Vector3 mousePos = mainCam.ScreenToViewportPoint(SystemFacade.instance.GetMousePosition());
        // 마우스 값을 중앙에 맞게 설정
        mousePos.x -= 0.5f;
        mousePos.y -= 0.5f;
        // 최대 카메라 값과 비교할 수 있게 설정
        mousePos.x *= (maxX * 2f);
        mousePos.y *= (maxZ * 2f);
        // 마우스의 좌우 값 제한
        mousePos.x = Mathf.Clamp(mousePos.x, -maxX, maxX);
        // 마우스의 상하 값 제한
        mousePos.y = Mathf.Clamp(mousePos.y, -maxZ, maxZ);
        // 마우스 값 반환
        return mousePos;
    }

    // 마우스 위치에 따른 이동 방향 반환 함수
    private Vector3 CalculateWorldMoveDirection(Vector3 mousePos)
    {
        // 메인 카메라의 정면 값 저장
        Vector3 camForward = mainCam.transform.forward;
        // 메인 카메라의 우측 값 저장
        Vector3 camRight = mainCam.transform.right;
        // 메인 카메라의 높이 값 초기화
        camForward.y = camRight.y = 0f;
        // 메인 카메라 정면 값 정리
        camForward.Normalize();
        // 메인 카메라 우측 값 정리
        camRight.Normalize();
        // 이동 방향 반환
        return camForward * mousePos.y + camRight * mousePos.x;
    }
}