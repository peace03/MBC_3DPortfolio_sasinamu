using UnityEngine;

public class EnemyStat : MonoBehaviour, IDamageable
{
    [Header("기본 스탯")]
    [SerializeField] private float curHp;                   // 현재 Hp
    [SerializeField] private float maxHp;                   // 최대 Hp
    [SerializeField] private float attackPower;             // 공격력
    public float AttackPower => attackPower;
    [SerializeField] private float attackDelayTime;         // 공격 딜레이 시간
    public float AttackDelayTime => attackDelayTime;

    [Header("거리 스탯")]
    [SerializeField] private float maxAttackDistance;       // 최대 공격 거리
    public float MaxAttackDistance => maxAttackDistance;
    [SerializeField] private int maxSpreadAngle;            // 최대 탄 퍼짐 각도
    public int MaxSpreadAngle => maxSpreadAngle;
    [SerializeField] private float maxChaseDistance;        // 최대 추적 거리
    public float MaxChaseDistance => maxChaseDistance;
    [SerializeField] private float maxReturnDistance;       // 최대 복귀 거리
    public float MaxReturnDistance => maxReturnDistance;
    [SerializeField] private float maxPatrolDistance;       // 최대 순찰 거리
    public float MaxPatrolDistance => maxPatrolDistance;

    private bool isDamaged;                                 // 피격 여부
    public bool IsDamaged => isDamaged;
    public bool IsDead => curHp <= 0;                       // 죽음 여부
    public float Ratio => curHp / maxHp;                    // Hp 비율

    private void OnEnable()
    {
        // Hp 설정
        curHp = maxHp;
    }

    // 피격 함수
    public void Damaged(DamagedEvent data)
    {
        // Hp 감소
        curHp -= data.amount;
        // 피격 상태
        isDamaged = true;
        // 피격 이벤트 발생
        EventBus<DamagedEvent>.Publish(data);
    }

    // 피격 상태 초기화 함수
    public void ResetDamagedState() => isDamaged = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
    }
}