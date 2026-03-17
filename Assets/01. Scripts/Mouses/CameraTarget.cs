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
    [SerializeField] private float verticalWeight;          // 쿼터뷰 보정 값(위, 아래 시야 확장)

    private void Start()
    {
        // 초기화
        CalculateVerticalWeight();
    }

    private void LateUpdate()
    {
        // 이게 -1 ~ 1이고, 내가 원하는 거는 어떻게 보면? 10 * 10 이라는 최대 사각형 크기만큼만 움직였으면 하니까?
        // 이 마우스 좌표에 최대 크기를 곱해주고?
        // 그걸 카메라의 forward랑 right를 조합해서 transform.position을 움직여주면? 내가 원하는 움직임 나올 것 같은데?

        Vector2 mousePos = inputManager.GetMouseOffsetRatio();
        //Debug.Log(mousePos);
        mousePos.x *= maxX;
        mousePos.y *= maxZ;
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = right.y = 0f;
        Vector3 dir = (forward * mousePos.y + right * mousePos.x);
        //Debug.Log(dir);
        transform.position = player.position + dir;

        // 이동(플레이어 위치 기반)
        //transform.position = player.position + GetMoveDirection();
        //Vector2 test = inputManager.GetMouseOffsetRatio();
        //var test = Camera.main.transform.forward;
        //test.y = 0f;
        //transform.position = player.position + test * maxZ;
    }

    public void Test()
    {
        // 메인 카메라의 정면 값 저장
        Vector3 camForward = Camera.main.transform.forward;
        // 메인 카메라의 우측 값 저장
        Vector3 camRight = Camera.main.transform.right;
        // 메인 카메라의 높이 값 초기화
        camForward.y = camRight.y = 0f;
        // 메인 카메라 정면 값 정리
        camForward.Normalize();
        // 메인 카메라 우측 값 정리
        camRight.Normalize();
    }

    // 쿼터뷰 보정 값 계산 함수
    public void CalculateVerticalWeight()
    {
        // 카메라의 상하 각도 값 저장
        float angle = Camera.main.transform.eulerAngles.x;
        // 각도(한바퀴를 360으로 나눈 것)를 라디안(한 바퀴를 2π로 나눈 것)으로 변환 후,
        // Sin 함수를 이용해 기울어진 각도에 따른 높이(비율) 저장
        float sinValue = Mathf.Sin(angle * Mathf.Deg2Rad);
        // 쿼터뷰 보정 값 저장(0으로 나눌 순 없으므로 보정)
        verticalWeight = 1f / Mathf.Max(sinValue, 0.01f);
    }

    // 이동 방향 반환 함수
    private Vector3 GetMoveDirection()
    {
        // 화면 전체 크기에서 현재 마우스 위치 비율 값 저장
        Vector2 ratio = inputManager.GetMouseOffsetRatio();
        // 최대 좌우 값 반영
        float moveX = ratio.x * maxX;
        // 최대 상하 값과 쿼터뷰 보정 값 반영
        float moveZ = ratio.y * maxZ * verticalWeight;
        // 이동 방향 반환
        return new Vector3(moveX, 0, moveZ);
    }
}