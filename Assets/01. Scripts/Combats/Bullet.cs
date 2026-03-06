using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("스탯")]
    [SerializeField] private float bulletSpeed;     // 총알 속도

    private Rigidbody rb;                           // 총알 리지드바디

    private DamagedEvent bulletInfo;                // 총알 정보(쏜 사람, 맞은 사람, 데미지)

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 총알과 닿은 콜라이더가 데미지를 입을 수 있는 인터페이스를 가지고 있다면
        if (other.TryGetComponent(out IDamageable target))
        {
            // 맞은 놈 설정
            bulletInfo.target = other.gameObject;
            // 데미지 전달하기
            target.Damaged(bulletInfo);
            // 피격 이펙트가 있다면 넣기
        }

        // 총알 비활성화
        gameObject.SetActive(false);
    }

    // 총알 정보 설정 함수
    public void SetBulletInfo(DamagedEvent data)
    {
        // 총알 정보 설정
        bulletInfo = data;
    }
}