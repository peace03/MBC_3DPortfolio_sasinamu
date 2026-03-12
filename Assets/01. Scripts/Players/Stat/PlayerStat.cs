using UnityEngine;

public class PlayerStat : MonoBehaviour, IDamageable
{
    #region Player Info
    [Header("정보")]
    [SerializeField] private float lastUsedStamTime;                    // 마지막 스테미나 사용한 시간

    [Header("기본 스탯")]
    [SerializeField] private float curHp;                               // 현재 체력
    public float CurHp => curHp;

    [SerializeField] private float maxHp;                               // 최대 체력
    public float MaxHp => maxHp;

    public float HpRatio => curHp / maxHp;                              // 체력 비율

    [Space(10)]
    [SerializeField] private float curStamina;                          // 현재 스테미나
    [SerializeField] private float maxStamina;                          // 최대 스테미나
    public float StamRatio => curStamina / maxStamina;                  // 스테미나 비율

    [SerializeField] private float stamRegenWaitTime;                   // 스테미나 회복 대기 시간
    [SerializeField] private float stamRegenRate;                       // 초당 스테미나 회복량

    [Space(10)]
    [SerializeField] private float attackPower;                         // 공격력
    [SerializeField] private float defensePower;                        // 방어력
    [SerializeField] private int weight;                                // 무게
    [SerializeField] private float weightSpeedMultiplier;               // 무게 속도 배율
    public float WeightSpeedMultiplier => weightSpeedMultiplier;

    [Space(10)]
    [SerializeField] private int curHunger;                             // 현재 허기
    [SerializeField] private int maxHunger;                             // 최대 허기
    public float hungerRatio => curHunger / maxHunger;                  // 허기 비율

    [SerializeField] private int curThirst;                             // 현재 갈증
    [SerializeField] private int maxThirst;                             // 최대 갈증
    public float thirstRatio => curThirst / maxThirst;                  // 갈증 비율

    [Header("스탯 제한 관련")]
    [SerializeField] private float maxInteractDistance;                 // 최대 상호작용 거리
    [SerializeField] private float minInteractAngle;                    // 최소 상호작용 각도
    [SerializeField] private float maxInteractAngle;                    // 최대 상호작용 각도

    [Space(10)]
    [SerializeField] private float singleFireDelayTime;         // 단발 발사 딜레이 시간
    [SerializeField] private float autoFireDelayTime;           // 연발 발사 딜레이 시간

    public int LayerNumber => gameObject.layer;                 // 레이어 번호
    #endregion

    private void Awake()
    {
        // 초기화
        curHp = maxHp;
        curStamina = maxStamina;
        curHunger = maxHunger;
        curThirst = maxThirst;
    }

    private void Update()
    {
        RegenStamina();
    }

    // 스테미나 회복 함수
    private void RegenStamina()
    {
        // 스테미나 회복 대기 시간이 지났다면
        if (Time.time - lastUsedStamTime >= stamRegenWaitTime)
        {
            // 스테미나 회복
            curStamina += stamRegenRate * Time.deltaTime;
            // 스테미나 최소값, 최대값에 맞춰서 반영
            curStamina = Mathf.Clamp(curStamina, 0, maxStamina);
        }
    }

    // 스테미나 사용 함수
    public bool UseStamina(float amount)
    {
        // 스테미나가 부족하다면
        if (curStamina - amount < 0)
            // 사용 불가
            return false;

        // 스테미나 감소
        curStamina -= amount;
        // 스테미나 사용한 시간 갱신
        lastUsedStamTime = Time.time;
        // 스테미나 최소값, 최대값에 맞춰서 반영
        curStamina = Mathf.Clamp(curStamina, 0, maxStamina);
        // 사용 가능
        return true;
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