using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private Camera mainCam;                             // 메인 카메라

    private Plane groundPlane;                          // 캐릭터의 아래 평면

    private void Awake()
    {
        // 초기화
        mainCam = Camera.main;
    }

    private void Update()
    {
        // 회전
        HandleRotate();
    }

    // 회전 관리 함수
    public void HandleRotate()
    {
        // 캐릭터의 아래 위치에 위를 바라보는 보이지 않는 평면 생성
        groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        // 카메라의 위치에서 카메라 렌즈(니어 플레인) 위의 마우스 위치를 통과하는 레이저 저장
        Ray ray = mainCam.ScreenPointToRay(SystemFacade.instance.GetMousePosition());

        // 평면과 부딪히는 레이저가 있다면
        if (groundPlane.Raycast(ray, out float distance))
        {
            // 카메라와 레이저의 거리를 이용해 마우스의 위치 저장
            Vector3 hitPoint = ray.GetPoint(distance);
            // y 값 초기화
            hitPoint.y = 0f;
            // 플레이어에서 마우스 위치의 방향 저장
            Vector3 dir = (hitPoint - transform.position).normalized;
            // x축 회전 막기(위 아래로 회전)
            dir.y = 0f;
            // 회전(방향까지 바라보기 위한 회전 값 넣기)
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}