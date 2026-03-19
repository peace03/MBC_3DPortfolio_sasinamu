using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Data/GunData")]
public class GunData : ConsumableData
{
    [Header("총 정보")]
    public int fireSpeed; //발사속도
    public int maxAmmo; //최대 탄약 수
}
