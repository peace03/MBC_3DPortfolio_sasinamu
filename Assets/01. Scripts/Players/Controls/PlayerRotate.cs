using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private InputManager inputManager;      // 입력 매니저

    private Vector3 lastDirection;          // 마지막 방향

    private void Update()
    {
        // 회전
        HandleRotate();
    }

    // 초기화 함수
    public void Init(InputManager manager) => inputManager = manager;

    // 회전 관리 함수
    public void HandleRotate()
    {
        // 마우스 위치 받아오기
        Vector3 mousePos = inputManager.GetMouseWorldPosition(transform.position.y);
        
        // 마우스 위치가 0이라면
        if (mousePos == Vector3.zero)
        {
            // 마지막 방향으로 플레이어 회전
            transform.rotation = Quaternion.LookRotation(lastDirection);
            // 종료
            return;
        }

        // 플레이어에서 마우스 위치의 방향 저장
        lastDirection = (mousePos - transform.position).normalized;
        // 회전
        transform.rotation = Quaternion.LookRotation(lastDirection);
    }
}