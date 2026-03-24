using UnityEngine;
using UnityEngine.Pool;

public class BulletFactory : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private GameObject smallBulletPrefab;              // S형 총알 프리팹
    [SerializeField] private GameObject largeBulletPrefab;              // L형 총알 프리팹

    [Header("총알 컨테이너")]
    [SerializeField] private Transform smallBulletContainer;            // S형 총알 컨테이너
    [SerializeField] private Transform largeBulletContainer;            // L형 총알 컨테이너

    [Header("총알 개수")]
    [SerializeField] private int smallBulletMaxCount;                   // S형 총알 최대 개수
    [SerializeField] private int largeBulletMaxCount;                   // L형 총알 최대 개수

    private IObjectPool<Bullet> smallBulletPool;                        // S형 총알 오브젝트 풀
    public IObjectPool<Bullet> SmallBulletPool => smallBulletPool;

    private IObjectPool<Bullet> largeBulletPool;                        // L형 총알 오브젝트 풀
    public IObjectPool<Bullet> LargeBulletPool => largeBulletPool;

    private void Awake()
    {
        // 초기화
        smallBulletPool = new ObjectPool<Bullet>(() =>
                                        CreateBullet(smallBulletPrefab, smallBulletPool, smallBulletContainer),
                                            GetBullet, ReturnBullet, DestroyBullet, maxSize: smallBulletMaxCount);
        largeBulletPool = new ObjectPool<Bullet>(() =>
                                        CreateBullet(largeBulletPrefab, largeBulletPool, largeBulletContainer),
                                            GetBullet, ReturnBullet, DestroyBullet, maxSize: largeBulletMaxCount);
    }

    // 총알 설정 함수
    private Bullet CreateBullet(GameObject prefab, IObjectPool<Bullet> poolRef, Transform container)
    {
        // ★ 총알 데미지 추가하기 ★

        // 오브젝트 생성
        GameObject bullet = Instantiate(prefab);
        // 오브젝트 풀 주소 설정
        bullet.GetComponent<Bullet>().SetPoolReference(poolRef);
        // 오브젝트를 컨테이너에 모으기
        bullet.transform.parent = container;
        // 총알 반환
        return bullet.GetComponent<Bullet>();
    }

    // 총알 반환 함수
    private void GetBullet(Bullet bullet) => bullet.gameObject.SetActive(true);

    // 총알 반납 함수
    private void ReturnBullet(Bullet bullet) => bullet.gameObject.SetActive(false);

    // 총알 파괴 함수
    private void DestroyBullet(Bullet bullet) => Destroy(bullet.gameObject);
}