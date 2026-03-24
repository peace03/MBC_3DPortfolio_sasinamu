using UnityEngine;

public class EnemyStat : MonoBehaviour, IDamageable
{
    #region Enemy Info
    [Header("정보")]
    [SerializeField] private bool isDamaged;                // 피격 여부
    public bool IsDamaged => isDamaged;

    public bool IsDead => curHp <= 0;                       // 죽음 여부

    [Header("스탯")]
    [SerializeField] private float curHp;                   // 현재 체력
    [SerializeField] private float maxHp;                   // 최대 체력
    public float HpRatio => curHp / maxHp;                  // 체력 비율

    [Space(10)]
    [SerializeField] private float attackPower;             // 공격력
    public float AttackPower => attackPower;

    [SerializeField] private float moveSpeed;               // 이동 속도
    public float MoveSpeed => moveSpeed;

    [Space(10)]
    [SerializeField] private float attackDelayTime;         // 공격 지연 시간
    public float AttackDelayTime => attackDelayTime;

    [SerializeField] private float hitStunTime;             // 피격 경직 시간
    public float HitStunTime => hitStunTime;

    [SerializeField] private float idleTime;                // 대기 시간
    public float IdleTime => idleTime;

    [Header("스탯 제한 관련")]
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
    #endregion

    private void OnEnable()
    {
        // Hp 설정
        curHp = maxHp;
    }

    // 피격 함수
    public void Damaged(string name, float amount)
    {
        // 피격 상태
        isDamaged = true;
        // Hp 감소
        curHp -= amount;
    }

    // 피격 상태 초기화 함수
    public void ResetDamagedState() => isDamaged = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
    }
}