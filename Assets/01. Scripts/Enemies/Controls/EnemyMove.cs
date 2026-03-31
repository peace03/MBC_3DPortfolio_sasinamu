using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private CharacterController cc;     // 적 캐릭터 컨트롤러

    private float moveSpeed;            // 이동 속도

    private void Awake()
    {
        // 초기화
        cc = GetComponent<CharacterController>();
    }

    // 초기화 함수
    public void Init(float speed) => moveSpeed = speed;

    // 이동 함수
    public void MoveToTarget(Vector3 position, float distance)
    {
        // 도착했다면
        if ((position - transform.position).magnitude <= distance)
        {
            // 위치 맞춰주기
            transform.position = position;
            // 종료
            return;
        }

        // 이동
        cc.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    // 회전 함수
    public void RotateToTarget(Transform target)
    {
        // 방향 구하기
        Vector3 dir = target.position - transform.position;
        // 높이 제거
        dir.y = 0f;

        // 방향이 같다면
        if (dir.normalized == Vector3.zero)
            // 종료
            return;

        // 회전
        transform.forward = dir.normalized;
    }
}