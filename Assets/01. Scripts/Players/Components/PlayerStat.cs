using UnityEngine;

public class PlayerStat : MonoBehaviour, IDamageable
{
    [Header("스탯")]
    [SerializeField] private float hp;                  // 체력
    public float Hp => hp;
    [SerializeField] private float attackPower;         // 공격력
    public float AttackPower => attackPower;
    [SerializeField] private float defensePower;        // 방어력
    public float DefensePower => defensePower;
    private int hunger;     // 허기
    private int thirst;     // 갈증

    // 스탯 정보 갱신 함수
    public void UpdateStatInfo(PlayerStatInfo info)
    {
        // 공격력
        attackPower = info.AttackPower;
        // 방어력
        defensePower = info.DefensePower;
    }

    // 피격 함수
    public void Damaged(DamagedEvent data)
    {
        // Hp 반영
        hp -= data.amount;

        // 죽었다면
        if (hp <= 0)
            // 플레이어 죽음
            EventBus<DeadEvent>.Publish(new DeadEvent(true));

        // 피격 이벤트 발생
        EventBus<DamagedEvent>.Publish(data);
    }
}