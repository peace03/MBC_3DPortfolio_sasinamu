using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Data/BulletData")]
public class BulletData : CountableData
{
    [Header("피격 정보")]
    [SerializeField] private float damage; //총알 데미지
    [SerializeField] private float durabilityDamage; //피격 대상 내구도 감소시키는 양

    public float Damage => damage;
    public float DurabilityDamage => durabilityDamage;
}
