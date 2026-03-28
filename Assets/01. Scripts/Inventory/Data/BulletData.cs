using UnityEngine;

public enum BulletType
{
    S, L
}

[CreateAssetMenu(fileName = "BulletData", menuName = "Data/BulletData")]
public class BulletData : CountableData
{
    [Header("피격 정보")]
    [SerializeField] private BulletType bulletType; //총알 타입
    [SerializeField] private float damage; //총알 데미지
    [SerializeField] private float durabilityDamage; //피격 대상 내구도 감소시키는 양

    public BulletType BulletType => bulletType;
    public float Damage => damage;
    public float DurabilityDamage => durabilityDamage;
}
