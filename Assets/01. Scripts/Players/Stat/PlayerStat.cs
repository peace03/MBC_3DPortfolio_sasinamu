using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class PlayerStat : MonoBehaviour, IDamageable, IUseItemHandler
{
    #region Player Info
    [Header("정보")]
    [SerializeField] private float lastUsedStamTime;                    // 마지막 스테미나 사용한 시간
    [SerializeField] private float lastDecreaseHungerAndThirstTime;     // 마지막 허기, 갈증 감소 시간

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
    public float AttackPower => attackPower;

    [SerializeField] private float defensePower;                        // 방어력
    public float DefensePower => defensePower;

    [Space(10)]
    [SerializeField] private float curInvWeight;                        // 현재 가방 무게
    [SerializeField] private float maxInvWeight;                        // 최대 가방 무게
    [SerializeField] private float speedMultiplierMaxInvWeight;         // 최대 가방 무게의 속도 배율
    public float SpeedMultiplierCurInvWeight                            // 현재 가방 무게의 속도 배율
    { get { return Mathf.Lerp(1, speedMultiplierMaxInvWeight, curInvWeight / maxInvWeight); } }

    [Space(10)]
    [SerializeField] private float curHunger;                           // 현재 허기
    [SerializeField] private float maxHunger;                           // 최대 허기
    public float hungerRatio => curHunger / maxHunger;                  // 허기 비율

    [SerializeField] private float curThirst;                           // 현재 갈증
    [SerializeField] private float maxThirst;                           // 최대 갈증
    public float thirstRatio => curThirst / maxThirst;                  // 갈증 비율

    [SerializeField] private float hungerAndThirstDereaseDelayTime;     // 허기, 갈증 감소 지연 시간
    [SerializeField] private float hungerAndThirstDecreaseAmount;       // 허기, 갈증 감소량

    [Header("스탯 제한 관련")]
    [SerializeField] private float maxInteractDistance;                 // 최대 상호작용 거리
    public float MaxInteractDistance => maxInteractDistance;

    [SerializeField] private float minInteractAngle;                    // 최소 상호작용 각도
    public float MinInteractAngle => minInteractAngle;

    [SerializeField] private float maxInteractAngle;                    // 최대 상호작용 각도
    public float MaxInteractAngle => maxInteractAngle;

    [Space(10)]
    [SerializeField] private float singleFireDelayTime;                 // 단발 사격 지연 시간
    public float SingleFireDelayTime => singleFireDelayTime;

    [SerializeField] private float autoFireDelayTime;                   // 연발 사격 지연 시간
    public float AutoFireDelayTime => autoFireDelayTime;
    #endregion

    private void Awake()
    {
        // 초기화
        curHp = maxHp;
        curStamina = maxStamina;
        curHunger = maxHunger;
        curThirst = maxThirst;
    }

    private void OnEnable()
    {
        // 아이템 사용 이벤트 구독
        Subject<IUseItemHandler>.Attach(this);
    }

    private void Update()
    {
        // 스테미나 회복
        RegenStamina();
        // 허기, 갈증 감소
        DecreaseHungerAndThirst();
    }

    private void OnDisable()
    {
        // 아이템 사용 이벤트 구독 해제
        Subject<IUseItemHandler>.Detach(this);
    }

    // 공격력, 방어력 갱신 함수
    public void UpdateAttackAndDefensePowers(float attack, float defense)
    {
        Debug.Log($"[변경 전] 공격력 : {attackPower} / 방어력 : {defensePower}");
        // 공격력
        attackPower = attack;
        // 방어력
        defensePower = defense;
        Debug.Log($"[변경 후] 공격력 : {attackPower} / 방어력 : {defensePower}");
    }

    // 현재 가방 무게 갱신 함수
    public void UpdateCurrentInventoryWeight(float weight)
    {
        Debug.Log($"[변경 전] 가방 무게 : {curInvWeight}");
        // 현재 가방 무게
        curInvWeight = weight;
        Debug.Log($"[변경 후] 가방 무게 : {curInvWeight}");
    }

    // 피격 함수
    public void Damaged(string name, float amount)
    {
        // 데미지 입음
        curHp -= amount;

        // 죽었다면
        if (curHp <= 0)
            // 플레이어 죽음 이벤트 발행
            Subject<IPlayerDeadHandler>.Publish(h => h.OnPlayerDead(name));
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

        // 스테미나 최소값, 최대값에 맞춰서 감소
        curStamina = Mathf.Clamp(curStamina - amount, 0, maxStamina);
        // 스테미나 사용한 시간 갱신
        lastUsedStamTime = Time.time;
        // 사용 가능
        return true;
    }

    // 현재 무게 갱신 함수
    public void UpdateCurrentWeight(float weight) => curInvWeight = weight;

    // 허기, 갈증 감소 함수
    private void DecreaseHungerAndThirst()
    {
        // 마지막 허기, 갈증 소모 시간에서 허기, 갈증 소모 지연 시간만큼 지나지 않았다면
        if (Time.time - lastDecreaseHungerAndThirstTime <= hungerAndThirstDereaseDelayTime)
            // 종료
            return;

        // 허기 감소
        curHunger = Mathf.Max(curHunger - hungerAndThirstDecreaseAmount, 0);
        // 갈증 감소
        curThirst = Mathf.Max(curThirst - hungerAndThirstDecreaseAmount, 0);
        // 마지막 허기, 갈증 소모 시간 저장
        lastDecreaseHungerAndThirstTime = Time.time;
    }

    // 회복 아이템 사용 함수
    public void OnUseCureItem(float cureAmount)
    {
        Debug.Log($"[변경 전] 현재 HP : {curHp}");
        // HP 회복
        curHp = Mathf.Clamp(curHp + cureAmount, 0, maxHp);
        Debug.Log($"[변경 후] 현재 HP : {curHp}");
    }

    // 음식 아이템 사용 함수
    public void OnUseFoodItem(float hunger, float thirst)
    {
        Debug.Log($"[변경 전] 현재 허기 : {curHunger} / 현재 갈증 : {curThirst}");
        // 허기 회복
        curHunger = Mathf.Clamp(curHunger + hunger, 0, maxHunger);
        // 갈증 회복
        curThirst = Mathf.Clamp(curThirst + thirst, 0, maxThirst);
        Debug.Log($"[변경 후] 현재 허기 : {curHunger} / 현재 갈증 : {curThirst}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxInteractDistance);
    }
}