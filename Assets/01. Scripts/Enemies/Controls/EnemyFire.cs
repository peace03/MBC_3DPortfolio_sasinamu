using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    private Transform target;                   // 목표
    private Transform firePoint;                // 사격 위치
    private BulletFactory bulletFactory;        // 총알 공장

    private float spreadAngle;                  // 퍼짐 각도

    // 초기화 함수
    public void Init(BulletFactory factory, Transform target, Transform firePoint, float angle)
    {
        bulletFactory = factory;
        this.target = target;
        this.firePoint = firePoint;
        spreadAngle = angle;
    }

    // 총알 발사 함수
    public void FireBullet()
    {
        // 방향 구하기
        Vector3 dir = target.position - firePoint.position;
        // 랜덤 탄 퍼짐 각도 구하기
        float randomAngle = Random.Range(-spreadAngle, spreadAngle);
        // 방향 각도에 탄 퍼짐 각도 더하기
        Quaternion finalRot = Quaternion.LookRotation(dir.normalized) * Quaternion.Euler(0, randomAngle, 0);
        // 사격(적 이름, 적 레이어, 사격 위치, 총알 각도)
        bulletFactory.SmallBulletPool.Get()
            .FireBullet(transform.parent.name, gameObject.layer, firePoint.position, finalRot);
    }
}