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
    [SerializeField] private float curFireDelayTime;        // 현재 사격 지연 시간
    [SerializeField] private float reloadingDelayTime;      // 장전 지연 시간

    private PlayerStat stat;                                // 플레이어 스탯
    private BulletFactory bulletFactory;                    // 총알 공장
    private Coroutine reloadingCoroutine;                   // 장전 코루틴

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
        // 사격이 불가능한 상태거나, 초기 상태가 아니고 마지막 사격 시간에서 사격 지연 시간만큼 지나지 않았거나, 장전 중이라면
        if (!canFire || (lastFireTime != 0 && Time.time - lastFireTime < curFireDelayTime) || isReloading)
            // 종료
            return;

        // S형 총알 받아오기
        Bullet bullet = bulletFactory.SmallBulletPool.Get();
        // 사격(플레이어 이름, 플레이어 레이어, 사격 위치, 사격 각도)
        bullet.FireBullet(name, gameObject.layer, firePoint.position, firePoint.rotation);
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

        // 사격 지연 시간 갱신
        UpdateFireDelayTimte();
        // 사격 모드 UI에 이벤트 발행하기
        Debug.Log("UI에게 사격 모드 변경 알려주기");
    }

    // 사격 지연 시간 갱신 함수
    private void UpdateFireDelayTimte()
    {
        // 현재 사격 모드에 따라
        switch (curFireMode)
        {
            // 단발이라면
            case FireMode.Single:
                // 단발 지연 시간 저장
                curFireDelayTime = stat.SingleFireDelayTime;
                break;
            // 연발이라면
            case FireMode.Auto:
                // 연발 지연 시간 저장
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
        // 장전 코루틴 시작
        reloadingCoroutine = StartCoroutine(ReloadingCoroutine());
    }

    // 장전 코루틴 함수
    private IEnumerator ReloadingCoroutine()
    {
        // 장전 시작 시간에서 장전 지연 시간만큼 지날 때까지
        while (Time.time - startReloadingTime < reloadingDelayTime)
        {
            // 게임 시간이 멈춰있다면
            if (Time.timeScale == 0)
            {
                // 프레임 단위로 기다리기
                yield return null;
                // 건너뛰기
                continue;
            }

            Debug.Log($"장전 진행 시간 : {(Time.time - startReloadingTime):F1}초");
            // 여기서 UI 슬라이더 값 전달하기

            // 프레임 단위로 기다리기
            yield return null;
        }

        Debug.Log("장전 끝!");
        // 정전 종료
        isReloading = false;
    }

    // 장전 취소 함수
    public void OnCancel()
    {
        // 장전 코루틴이 비어있다면
        if (reloadingCoroutine == null)
            // 종료
            return;

        // 장전 취소
        isReloading = false;
        // 장전 코루틴 정지
        StopCoroutine(reloadingCoroutine);
        // 장전 코루틴 초기화
        reloadingCoroutine = null;
        // 나중에 UI로 값 보내주기
        Debug.Log("장전 취소");
    }
}