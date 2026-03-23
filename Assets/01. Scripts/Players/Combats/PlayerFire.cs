using System.Collections;
using UnityEngine;

public class PlayerFire : MonoBehaviour,
    IPlayerFireHandler, IPlayerFireModeHandler, IPlayerReloadHandler, IPlayerCancelHandler
{
    [Header("정보")]
    [SerializeField] private Transform firePoint;           // 발사 위치
    [SerializeField] private bool canFire;                  // 사격 가능 여부
    [SerializeField] private float lastFireTime;            // 마지막 사격 시간
    [SerializeField] private bool isReloading;              // 장전 중 여부
    [SerializeField] private float startReloadingTime;      // 장전 시작 시간

    [Header("스탯")]
    [SerializeField] private FireMode curFireMode;          // 현재 사격 모드
    [SerializeField] private float curFireDelayTime;        // 현재 사격 딜레이 시간
    [SerializeField] private float reloadingTime;           // 사격 진행 시간


    private PlayerStat stat;                                // 플레이어 스탯
    private BulletFactory bulletFactory;                    // 총알 공장

    private void Awake()
    {
        // 초기화
        curFireMode = FireMode.Single;
        UpdateFireDelayTimte();
    }

    private void OnEnable()
    {
        // 사격 이벤트 구독
        Subject<IPlayerFireHandler>.Attach(this);
        // 사격 모드 변경 이벤트 구독
        Subject<IPlayerFireModeHandler>.Attach(this);
        // 장전 이벤트 구독
        Subject<IPlayerReloadHandler>.Attach(this);
        // 취소 이벤트 구독
        Subject<IPlayerCancelHandler>.Attach(this);
    }

    private void Update()
    {
        // 장전
        HandleReloading();
        // 사격
        HandleFireBullet();
    }

    private void OnDisable()
    {
        // 사격 이벤트 구독 해제
        Subject<IPlayerFireHandler>.Detach(this);
        // 사격 모드 변경 이벤트 구독 해제
        Subject<IPlayerFireModeHandler>.Detach(this);
        // 장전 이벤트 구독 해제
        Subject<IPlayerReloadHandler>.Detach(this);
        // 취소 이벤트 구독 해제
        Subject<IPlayerCancelHandler>.Detach(this);
    }

    // 초기화 함수
    public void Init(PlayerStat stat, BulletFactory factory)
    {
        this.stat = stat;
        bulletFactory = factory;
    }

    // 사격 입력 값 설정 함수
    public void OnFire(bool isPressed) => canFire = isPressed;

    // 사격 관리 함수
    private void HandleFireBullet()
    {
        // 사격이 불가능한 상태거나, 초기 상태가 아니고 마지막 사격 시간에서 딜레이 시간만큼 지나지 않았거나, 장전 중이라면
        if (!canFire || (lastFireTime != 0 && Time.time - lastFireTime < curFireDelayTime) || isReloading)
            // 종료
            return;

        // S형 총알 받아오기
        Bullet bullet = bulletFactory.SmallBulletPool.Get();
        // 공격 주체의 레이어 번호, 발사 위치, 발사 각도 설정 후 총알 발사
        bullet.FireBullet(gameObject.layer, firePoint.position, firePoint.rotation);
        // 마지막 사격 시간 갱신
        lastFireTime = Time.time;

        // 단발 모드라면
        if (curFireMode == FireMode.Single)
            // 사격 불가능
            canFire = false;
    }

    // 사격 모드 변경 함수
    public void OnFireMode()
    {
        // 사격 중이거나, 장전 중이라면
        if (canFire || isReloading)
            // 종료
            return;

        // 마지막 사격 모드라면
        if (curFireMode == FireMode.Auto)
            // 첫번째 사격 모드로 변경
            curFireMode = FireMode.Single;
        // 마지막 사격 모드가 아니라면
        else
            // 다음 사격 모드로 변경
            curFireMode++;

        // 사격 딜레이 시간 갱신
        UpdateFireDelayTimte();
        // 사격 모드 UI에 이벤트 발행하기
        Debug.Log("UI에게 사격 모드 변경 알려주기");
    }

    // 사격 딜레이 시간 갱신 함수
    private void UpdateFireDelayTimte()
    {
        // 현재 사격 모드에 따라
        switch (curFireMode)
        {
            // 단발이라면
            case FireMode.Single:
                // 단발 딜레이 시간 저장
                curFireDelayTime = stat.SingleFireDelayTime;
                break;
            // 연발이라면
            case FireMode.Auto:
                // 연발 딜레이 시간 저장
                curFireDelayTime = stat.AutoFireDelayTime;
                break;
        }
    }

    // 장전 함수
    public void OnReload()
    {
        // 장전 중이라면
        if (isReloading)
            // 종료
            return;

        // 장전 중
        isReloading = true;
        // 장전 시작 시간 저장
        startReloadingTime = Time.time;
    }

    // 장전 관리 함수
    private void HandleReloading()
    {
        // 장전 중이 아니라면
        if (!isReloading)
            // 종료
            return;

        // 여기서 UI로 값 보내주면 될듯?
        Debug.Log("장전 중...");

        // 장전 시작 시간에서 장전 시간만큼 지났다면
        if (Time.time - startReloadingTime >= reloadingTime)
        {
            Debug.Log("장전 끝!");
            // 정전 종료
            isReloading = false;
            // 종료
            return;
        }
    }

    // 장전 취소 함수
    public void OnCancel()
    {
        // 장전 취소
        isReloading = false;
        // 나중에 UI로 값 보내주기
        Debug.Log("장전 취소");
    }
}