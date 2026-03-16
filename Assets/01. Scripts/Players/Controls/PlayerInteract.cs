//using UnityEngine;

//public class PlayerInteract : MonoBehaviour
//{
//    [Header("정보")]
//    [SerializeField] private LayerMask interactLayer;           // 상호작용 레이어
//    [SerializeField] private Collider curInteractTarget;        // 현재 상호작용 타겟

//    private PlayerManager manager;                              // 플레이어 매니저

//    private void OnEnable()
//    {
//        // 상호작용 이벤트 구독
//        EventBus<InteractEvent>.OnEvent += OnInteract;
//    }

//    private void Update()
//    {
//        // 상호작용 타겟 설정
//        SetInteractableTarget();
//    }

//    private void OnDisable()
//    {
//        // 상호작용 이벤트 구독 해제
//        EventBus<InteractEvent>.OnEvent -= OnInteract;
//    }

//    // 초기화 함수
//    public void Init(PlayerManager manager) => this.manager = manager;

//    // 상호작용 가능한 타겟 설정하는 함수
//    private void SetInteractableTarget()
//    {
//        // 반지름이 최대 상호작용 거리인 구에 닿은, 상호작용 레이어에 속한 물체들 저장
//        var targets = Physics.OverlapSphere(transform.position, manager.Stat.MaxInteractDistance, interactLayer);

//        // 근처에 상호작용 가능한 물체들이 없고, 현재 상호작용 타겟이 있다면
//        if (targets.Length == 0 && curInteractTarget != null)
//        {
//            // 현재 상호작용 타겟 비우기
//            curInteractTarget = null;
//            // 종료
//            return;
//        }
//        // 근처에 상호작용 가능한 물체가 없다면
//        else if (targets.Length == 0)
//            // 종료
//            return;

//        // 저장할 타겟
//        Collider target = null;
//        // 타겟에 대한 점수
//        float score = -1f;

//        // 물체들의 수만큼
//        foreach (var trg in targets)
//        {
//            // 상호작용 인터페이스가 없다면
//            if (!trg.TryGetComponent(out IInteractable interactable))
//            {
//                Debug.Log($"[Error] Target {trg.gameObject.name}에 인터페이스가 없음");
//                // 건너뛰기
//                continue;
//            }

//            // 상호작용 가능한지 확인 후, 반환되는 타겟에 대한 점수 저장
//            float value = CheckCanInteract(trg);

//            // 이전 타겟에 대한 점수보다 현재 점수가 크다면
//            if (value > score)
//            {
//                // 타겟 변경
//                target = trg;
//                // 점수 갱신
//                score = value;
//            }
//        }

//        // 현재 상호작용 타겟에 저장
//        curInteractTarget = target;
//    }

//    // 상호작용 가능 여부 확인 함수
//    private float CheckCanInteract(Collider target)
//    {
//        // 플레이어와 가까운 물체의 표면 좌표 저장
//        Vector3 trgClosestPos = target.ClosestPoint(transform.position);
//        // 플레이어 반지름 저장
//        float radius = Mathf.Max(transform.localScale.x, transform.localScale.z) * 0.5f;
//        // 물체의 표면 좌표와의 거리를 받아와서, 계산하기 편하게 정규화하기
//        float dis = Mathf.InverseLerp(radius, manager.Stat.MaxInteractDistance, Vector3.Distance(transform.position, trgClosestPos));
//        // 거리에 따른 상호작용 가능한 각도 저장
//        float canInteractAngle = Mathf.Lerp(manager.Stat.MaxInteractAngle, manager.Stat.MinInteractAngle, dis);
//        // 물체의 표면 좌표와의 방향 저장
//        Vector3 dir = (trgClosestPos - transform.position).normalized;
//        // 물체의 표면 좌표와의 각도 저장
//        float angle = Vector3.Angle(transform.forward, dir);

//        // 상호작용이 불가능한 각도라면
//        if (angle > canInteractAngle)
//            // 종료
//            return -1f;

//        // 0 ~ 2 사이의 점수를 반환 (1 - 거리(0~1)) + (1 - 각도(0~1))
//        return (1f - dis) + (1f - angle / canInteractAngle);
//    }

//    // 상호작용 실행 함수
//    private void OnInteract(InteractEvent data)
//    {
//        // 현재 상호작용 타겟이 비어있지 않고, 상호작용 인터페이스를 가지고 있다면
//        if (curInteractTarget != null && curInteractTarget.TryGetComponent(out IInteractable interactable))
//            // 상호작용 함수 실행(전리품 상자 : UI 활성화)
//            interactable.Interact();
//    }
//}