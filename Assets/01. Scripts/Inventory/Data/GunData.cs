using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Data/GunData")]
public class GunData : ConsumableData
{
    [Header("총 정보")]
    [SerializeField] private BulletType bulletType; //총이 사용하는 총알 타입
    [SerializeField] private int fireSpeed; //발사속도
    [SerializeField] private int maxAmmo; //최대 탄약 수

    public BulletType BulletType => bulletType;
    public int FireSpeed => fireSpeed;
    public int MaxAmmo => maxAmmo;
}
