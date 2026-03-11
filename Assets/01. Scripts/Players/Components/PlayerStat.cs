using UnityEngine;

public class PlayerStat : MonoBehaviour, IDamageable
{
    [Header("스탯")]
    [SerializeField] private float curHp;               // 현재 체력
    [SerializeField] private float maxHp;               // 최대 체력
    public float HpRatio => curHp / maxHp;              // 체력 비율

    [SerializeField] private float attackPower;         // 공격력
    public float AttackPower => attackPower;

    [SerializeField] private float defensePower;        // 방어력
    public float DefensePower => defensePower;

    private int hunger;     // 허기
    private int thirst;     // 갈증

    public int LayerNumber => gameObject.layer;         // 레이어 번호

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
        curHp -= data.amount;

        // 죽었다면
        if (curHp <= 0)
            // 플레이어 죽음
            EventBus<DeadEvent>.Publish(new DeadEvent(true));

        // 피격 이벤트 발생
        EventBus<DamagedEvent>.Publish(data);
    }
}