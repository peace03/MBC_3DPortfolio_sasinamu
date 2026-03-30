using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Data/GunData")]
public class GunData : ConsumableData
{
    [Header("총 정보")]
    [SerializeField] private int bulletId;      // 총알 ID
    [SerializeField] private int fireSpeed; //발사속도
    [SerializeField] private int maxAmmo; //최대 탄약 수

    public int BulletId => bulletId;
    public int FireSpeed => fireSpeed;
    public int MaxAmmo => maxAmmo;
}
