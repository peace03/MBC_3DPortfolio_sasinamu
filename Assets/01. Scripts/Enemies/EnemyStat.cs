//using UnityEngine;

//public class EnemyStat : MonoBehaviour, IDamageable
//{
//    #region Enemy Info
//    [Header("정보")]
//    [SerializeField] private bool isDamaged;                // 피격 여부
//    public bool IsDamaged => isDamaged;

//    public bool IsDead => curHp <= 0;                       // 죽음 여부

//    [Header("스탯")]
//    [SerializeField] private float curHp;                   // 현재 체력
//    [SerializeField] private float maxHp;                   // 최대 체력
//    public float HpRatio => curHp / maxHp;                  // 체력 비율

//    [Space(10)]
//    [SerializeField] private float attackPower;             // 공격력
//    public float AttackPower => attackPower;

//    [SerializeField] private float moveSpeed;               // 이동 속도
//    public float MoveSpeed => moveSpeed;

//    [Space(10)]
//    [SerializeField] private float attackDelayTime;         // 공격 딜레이 시간
//    public float AttackDelayTime => attackDelayTime;

//    [SerializeField] private float hitStunTime;             // 피격 경직 시간
//    public float HitStunTime => hitStunTime;

//    [SerializeField] private float idleTime;                // 대기 시간
//    public float IdleTime => idleTime;

//    [Header("스탯 제한 관련")]
//    [SerializeField] private float maxAttackDistance;       // 최대 공격 거리
//    public float MaxAttackDistance => maxAttackDistance;

//    [SerializeField] private int maxSpreadAngle;            // 최대 탄 퍼짐 각도
//    public int MaxSpreadAngle => maxSpreadAngle;

//    [SerializeField] private float maxChaseDistance;        // 최대 추적 거리
//    public float MaxChaseDistance => maxChaseDistance;

//    [SerializeField] private float maxReturnDistance;       // 최대 복귀 거리
//    public float MaxReturnDistance => maxReturnDistance;

//    [SerializeField] private float maxPatrolDistance;       // 최대 순찰 거리
//    public float MaxPatrolDistance => maxPatrolDistance;

//    public int LayerNumber => gameObject.layer;             // 레이어 번호
//    #endregion

//    private void OnEnable()
//    {
//        // Hp 설정
//        curHp = maxHp;
//    }

//    // 피격 함수
//    public void Damaged(DamagedEvent data)
//    {
//        // Hp 감소
//        curHp -= data.amount;
//        // 피격 상태
//        isDamaged = true;
//        // 피격 이벤트 발생
//        EventBus<DamagedEvent>.Publish(data);
//    }

//    // 피격 상태 초기화 함수
//    public void ResetDamagedState() => isDamaged = false;

//    // Scene 화면에서 선을 그려주는 함수
//    private void OnDrawGizmos()
//    {
//        // 노랑색 설정
//        Gizmos.color = Color.yellow;
//        // 반지름이 최대 추격 거리이고, 중심이 적 위치인, 속이 비어있는 구 그리기
//        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
//    }
//}