using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private string attackerName;           // 사격자 이름
    [SerializeField] private LayerMask attackerLayer;       // 사격자(플레이어, 적)의 레이어

    [Header("스탯")]
    [SerializeField] private float bulletSpeed;             // 총알 속도
    [SerializeField] private float bulletAttackPower;       // 총알 데미지
    [SerializeField] private float returnTime;              // 총알 반납 시간(총알 비활성화 시점)

    private IObjectPool<Bullet> poolRef;                    // 총알 오브젝트 풀 주소

    private Rigidbody rb;                                   // 총알 리지드바디
    private Coroutine returnToPoolCoroutine;                // 총알 반납 코루틴

    private void Awake()
    {
        // 초기화
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        // 속도 초기화
        rb.linearVelocity = Vector3.zero;
    }

    private void OnDisable()
    {
        // 총알 반납 코루틴이 비어있지 않다면
        if (returnToPoolCoroutine != null)
        {
            // 총알 반납 코루틴 정지
            StopCoroutine(returnToPoolCoroutine);
            // 총알 반납 코루틴 초기화
            returnToPoolCoroutine = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        // 비활성화 중이거나, 발사 주체의 레이어와 닿은 오브젝트의 레이어가 같다면(아군이라면)
        if (!gameObject.activeSelf || attackerLayer == other.gameObject.layer)
            // 종료
            return;

        // 총알과 닿은 콜라이더가 데미지를 입을 수 있는 인터페이스를 가지고 있다면
        if (other.TryGetComponent(out IDamageable target))
            // 데미지 전달하기
            target.Damaged(attackerName, bulletAttackPower);

        // 총알 반납
        poolRef.Release(this);
    }

    // 총알 데미지 설정 함수
    public void SetBulletAttackPower(float power) => bulletAttackPower = power;

    // 오브젝트 풀 주소 설정 함수
    public void SetPoolReference(IObjectPool<Bullet> pool) => poolRef = pool;

    // 총알 발사 함수
    public void FireBullet(string name, LayerMask layer, Vector3 pos, Quaternion rot)
    {
        // 이름 설정
        attackerName = name;
        // 총알 정보 설정
        attackerLayer = layer;
        // 총알 위치, 각도 설정
        transform.SetPositionAndRotation(pos, rot);
        // 총알 발사
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        // 총알 반납 코루틴 시작
        ReturnBulletToPool();
    }

    // 총알 반납 함수
    private void ReturnBulletToPool()
    {
        // 총알 반납 코루틴이 비어있지 않다면
        if (returnToPoolCoroutine != null)
        {
            // 총알 반납 코루틴 정지
            StopCoroutine(returnToPoolCoroutine);
            // 총알 반납 코루틴 초기화
            returnToPoolCoroutine = null;
        }

        // 총알 반납 코루틴 저장 및 시작
        returnToPoolCoroutine = StartCoroutine(ReturnToPoolCoroutine());
    }

    // 총알 반납 코루틴 함수
    private IEnumerator ReturnToPoolCoroutine()
    {
        // 총알 반납 시간만큼 대기
        yield return new WaitForSeconds(returnTime);
        // 총알 반납
        poolRef.Release(this);
        // 총알 반납 코루틴 초기화
        returnToPoolCoroutine = null;
    }
}