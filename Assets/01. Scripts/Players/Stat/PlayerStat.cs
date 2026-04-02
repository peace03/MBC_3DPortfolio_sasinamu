using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour, IDamageable, IUseItemHandler, IInventoryWeightHandler
{
    #region Player Info
    [Header("정보")]
    [SerializeField] private Slider hpSlider;                           // 체력 슬라이더
    [SerializeField] private Text hpText;                               // 체력 텍스트
    [SerializeField] private Slider hungerSlider;                       // 허기 슬라이더
    [SerializeField] private Slider thirstSlider;                       // 갈증 슬라이더
    [SerializeField] private float lastUsedStamTime;                    // 마지막 스테미나 사용한 시간
    [SerializeField] private float lastDecreaseHungerAndThirstTime;     // 마지막 허기, 갈증 감소 시간

    [Header("기본 스탯")]
    [SerializeField] private float curHp;                               // 현재 체력
    [SerializeField] private float maxHp;                               // 최대 체력
    private float HpRatio => curHp / maxHp;                             // 체력 비율

    [Space(10)]
    [SerializeField] private float curStamina;                          // 현재 스테미나
    [SerializeField] private float maxStamina;                          // 최대 스테미나
    private float StamRatio => curStamina / maxStamina;                 // 스테미나 비율

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
    private float HungerRatio => curHunger / maxHunger;                 // 허기 비율

    [SerializeField] private float curThirst;                           // 현재 갈증
    [SerializeField] private float maxThirst;                           // 최대 갈증
    private float ThirstRatio => curThirst / maxThirst;                 // 갈증 비율

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
        ChangeHpUI();
        ChangeHungerAndThirstUI();
    }

    private void OnEnable()
    {
        // 아이템 사용 이벤트 구독
        Subject<IUseItemHandler>.Attach(this);
        
        Subject<IInventoryWeightHandler>.Attach(this);
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
        
        Subject<IInventoryWeightHandler>.Detach(this);
    }

    // 피격 함수
    public void Damaged(string name, float amount)
    {
        // 영구 보존 구역(GameDataManager)에서 3번 슬롯(조끼)의 메모리 주소를 끌어옵니다.
        Item vestItem = GameDataManager.Instance.EquipModel.GetItem(3);

        // 조끼를 입고 있다면
        if (vestItem is VestItem vest)
        {
            // 조끼 내구도 차감 (데미지 계산 공식은 기획에 맞게 적용)
            // 비트 연산 결과 내구도가 0 이하로 떨어졌다면
            if (vest.DecreaseDurability() <= 0)
            {
                // 3번 슬롯을 파괴하라는 통합 신호 발송
                Subject<IEquipmentDestroyHandler>.Publish(h => h.OnEquipmentDestroyed(3));
            }
            Subject<ISlotChanged>.Publish(h => h.OnUpdateSingleSlot(SlotType.Equip, 3));
        }

        // 데미지 입음
        curHp -= amount;
        // 체력 UI 변경
        ChangeHpUI();

        // 죽었다면
        if (curHp <= 0)
            // 플레이어 죽음 이벤트 발행
            Subject<IPlayerDeadHandler>.Publish(h => h.OnPlayerDead(name));
    }

    // 체력 UI 변경 함수
    private void ChangeHpUI()
    {
        // 체력 슬라이더 값 변경
        hpSlider.value = HpRatio;
        // 체력 텍스트 변경
        hpText.text = $"{curHp}/{maxHp}";
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
            Subject<IStaminaUIHandler>.Publish(h => h.OnStaminaUI(StamRatio));
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
        Subject<IStaminaUIHandler>.Publish(h => h.OnStaminaUI(StamRatio));
        // 사용 가능
        return true;
    }

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
        // 허기, 갈증 UI 변경
        ChangeHungerAndThirstUI();
    }

    // 허기, 갈증 UI 변경 함수
    private void ChangeHungerAndThirstUI()
    {
        // 허기 슬라이더 값 변경
        hungerSlider.value = HungerRatio;
        // 갈증 슬라이더 값 변경
        thirstSlider.value = ThirstRatio;
    }

    // 회복 아이템 사용 함수
    public void OnUseCureItem(float cureAmount)
    {
        // HP 회복
        curHp = Mathf.Clamp(curHp + cureAmount, 0, maxHp);
        // 체력 UI 변경
        ChangeHpUI();
    }

    // 음식 아이템 사용 함수
    public void OnUseFoodItem(float hunger, float thirst)
    {
        // 허기 회복
        curHunger = Mathf.Clamp(curHunger + hunger, 0, maxHunger);
        // 갈증 회복
        curThirst = Mathf.Clamp(curThirst + thirst, 0, maxThirst);
        // 허기, 갈증 UI 변경
        ChangeHungerAndThirstUI();
    }

    public void UpdateAttackPower(float power) => attackPower = power;

    public void UpdateDefensePower(float power) => defensePower = power;

    public void OnInventoryWeight(float weight) => curInvWeight = weight;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxInteractDistance);
    }
}