using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactory : MonoBehaviour
{
    [Header("총알")]
    [SerializeField] private GameObject bulletPrefab;               // 총알 프리팹
    [SerializeField] private int bulletCount;                       // 총알 개수

    private List<Bullet> allBullectList = new List<Bullet>();       // 모든 총알 리스트
    private Queue<Bullet> bulletPool = new Queue<Bullet>();         // 사용 가능한 총알 큐

    private void Awake()
    {
        for (int i = 0; i < bulletCount; i++)
            CreateNewBullet();
    }

    public Bullet GetBullet()
    {
        if (bulletPool.Count > 0)
            return bulletPool.Dequeue();
        else
            return CreateNewBullet();
    }

    private Bullet CreateNewBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        allBullectList.Add(bullet.GetComponent<Bullet>());
        bullet.transform.parent = transform;
        bullet.SetActive(false);
        return bullet.GetComponent<Bullet>();
    }

    public void ReturnBullet(Bullet bullet)
    {
        bulletPool.Enqueue(bullet);
    }
}