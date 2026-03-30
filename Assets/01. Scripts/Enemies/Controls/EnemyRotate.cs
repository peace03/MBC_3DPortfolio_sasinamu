using UnityEngine;

public class EnemyRotate : MonoBehaviour
{
    // 적 회전 함수
    public void RotateToTarget(Transform target)
    {
        // 방향 구하기
        Vector3 dir = target.position - transform.position;
        // 높이 제거
        dir.y = 0;

        // 방향이 같다면
        if (dir.normalized == Vector3.zero)
            // 종료
            return;

        // 회전
        transform.forward = dir.normalized;
    }
}