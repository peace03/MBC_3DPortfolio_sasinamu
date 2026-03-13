using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private Transform firePoint;           // 발사 위치
    [SerializeField] private bool pressedFireButton;        // 사격 버튼 누름 여부
    [SerializeField] private float lastFireTime;            // 마지막 사격 시간
    [SerializeField] private FireMode curFireMode;          // 현재 사격 모드
    [SerializeField] private bool isSingleFire;             // 단발 발사 여부

    private PlayerManager manager;

    private void OnEnable()
    {
        // 사격 이벤트 구독
        EventBus<FireEvent>.OnEvent += SetPressedFireButton;
        // 사격 모드 변경 이벤트 구독
        EventBus<FireModeEvent>.OnEvent += HandleFireMode;
    }

    private void Update()
    {
        // 사격 버튼을 누르고 있다면
        if (pressedFireButton)
        {
            // 사격 모드에 따라
            switch (curFireMode)
            {
                // 단발이라면
                case FireMode.Single:
                    // 발사하지 않은 상태라면
                    if (!isSingleFire)
                    {
                        // 총알 발사
                        HandleFireBullet(manager.Stat.SingleFireDelayTime);
                        // 발사함
                        isSingleFire = true;
                    }
                    break;
                // 연발이라면
                case FireMode.Auto:
                    // 총알 발사
                    HandleFireBullet(manager.Stat.AutoFireDelayTime);
                    break;
            }
        }
        // 사격 버튼을 누르고 있지 않고, 발사한 상태라면
        else if (isSingleFire)
            // 발사하지 않은 상태
            isSingleFire = false;
    }

    private void OnDisable()
    {
        // 사격 이벤트 구독 해제
        EventBus<FireEvent>.OnEvent -= SetPressedFireButton;
        // 사격 모드 변경 이벤트 구독 해제
        EventBus<FireModeEvent>.OnEvent -= HandleFireMode;
    }

    // 초기화 함수
    public void Init(PlayerManager manager) => this.manager = manager;

    // 사격 버튼 입력 값 설정 함수
    private void SetPressedFireButton(FireEvent data) => pressedFireButton = data.isPressed;

    // 총알 발사 관리 함수
    private void HandleFireBullet(float delay)
    {
        // 초기 상태가 아니고, 마지막 발사 시간에서 딜레이 시간만큼 지나지 않았다면
        if (lastFireTime != 0 && Time.time - lastFireTime <= delay)
            // 종료
            return;

        // S형 총알 받아오기
        Bullet bullet = SystemFacade.instance.BulletFactory.SmallBulletPool.Get();
        // 총알 정보, 위치, 각도 설정 후 총알 발사
        bullet.FireBullet(new DamagedEvent(manager.Stat, manager.Stat.AttackPower),
                                                firePoint.position, firePoint.rotation);
        // 마지막 발사 시간 갱신
        lastFireTime = Time.time;
    }

    // 사격 모드 변경 관리 함수
    private void HandleFireMode(FireModeEvent data)
    {
        // 사격 버튼을 누르고 있다면
        if (pressedFireButton)
            // 종료
            return;
        // 마지막 사격 모드라면
        else if (curFireMode == FireMode.Auto)
        {
            // 사격 모드 첫번째로
            curFireMode = 0;
            // 종료
            return;
        }

        // 사격 모드 변경
        curFireMode++;
    }
}