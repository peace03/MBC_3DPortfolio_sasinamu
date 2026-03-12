//using UnityEngine;

//public class PlayerFire : MonoBehaviour
//{
//    [Header("정보")]
//    [SerializeField] private Transform firePoint;           // 발사 위치
//    [SerializeField] private bool pressedFireButton;        // 발사 버튼 누름 여부
//    [SerializeField] private FireMode fireMode;             // 발사 모드
//    [SerializeField] private bool isSingleFire;             // 단발 발사 여부

//    private PlayerStat stat;

//    private void Awake()
//    {
//        stat = SystemFacade.instance.PlayerStat;
//    }

//    private void OnEnable()
//    {
//        // 발사 이벤트 구독
//        EventBus<FireEvent>.OnEvent += SetPressedFireButton;
//    }

//    private void Update()
//    {
//        // 발사 버튼을 누르고 있다면
//        if (pressedFireButton)
//        {
//            // 발사 모드에 따라
//            switch (fireMode)
//            {
//                // 단발이라면
//                case FireMode.Single:
//                    // 발사하지 않은 상태라면
//                    if (!isSingleFire)
//                    {
//                        // 총 발사
//                        HandleFire(singleFireDelayTime);
//                        // 발사한 상태
//                        isSingleFire = true;
//                    }
//                    break;
//                // 연발이라면
//                case FireMode.Auto:
//                    // 총 발사
//                    HandleFire(autoFireDelayTime);
//                    break;
//            }
//        }
//        // 발사 버튼을 누르고 있지 않고, 발사한 상태라면
//        else if (isSingleFire)
//            // 발사하지 않은 상태
//            isSingleFire = false;
//    }

//    private void OnDisable()
//    {
//        // 발사 이벤트 구독 해제
//        EventBus<FireEvent>.OnEvent -= SetPressedFireButton;
//    }

//    // 발사 버튼 입력 값 설정 함수
//    private void SetPressedFireButton(FireEvent data) => pressedFireButton = data.isPressed;

//    // 발사 관리 함수
//    private void HandleFire(float delay)
//    {
//        // 현재 시간이 다음 발사 시간보다 작다면
//        if (Time.time < Time.time + delay)
//            // 종료
//            return;

//        // S형 총알 받아오기
//        Bullet bullet = SystemFacade.instance.BulletFactory.SmallBulletPool.Get();
//        // 총알 정보, 위치, 각도 설정 후 총알 발사
//        bullet.FireBullet(new DamagedEvent(stat, stat.AttackPower), firePoint.position, firePoint.rotation);
//    }
//}