using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Data/BulletData")]
public class BulletData : CountableData
{
    [Header("피격 정보")]
    public float Damage; //총알 데미지
    public float DurabilityDamage; //피격 대상 내구도 감소시키는 양
}
