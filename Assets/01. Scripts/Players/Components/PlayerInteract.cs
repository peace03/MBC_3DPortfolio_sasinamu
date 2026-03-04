using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("정보")]
    [SerializeField] private LayerMask interactLayer;           // 상호작용 레이어
    [SerializeField] private Collider curInteractTarget;        // 현재 상호작용 타겟

    [Header("스탯")]
    [SerializeField] private float maxInteractDistance;         // 최대 상호작용 거리
    [SerializeField] private float maxInteractAngle;            // 최대 상호작용 각도

    private void OnEnable()
    {
        // 상호작용 이벤트 구독
        EventBus<InteractEvent>.OnEvent += OnInteract;
    }

    private void Update()
    {
        // 상호작용 타겟 설정
        SetInteractableTarget();
    }

    private void OnDisable()
    {
        // 상호작용 이벤트 구독 해제
        EventBus<InteractEvent>.OnEvent -= OnInteract;
    }

    // 상호작용 가능한 타겟 설정하는 함수
    private void SetInteractableTarget()
    {
        // 반지름이 최대 상호작용 거리인 구에 닿은, 상호작용 레이어에 속한 물체들 받아오기
        var targets = Physics.OverlapSphere(transform.position, maxInteractDistance,
                                            interactLayer);

        // 근처에 상호작용 가능한 물체들이 없고, 현재 상호작용 타겟이 있다면
        if (targets.Length == 0 && curInteractTarget != null)
        {
            // 현재 상호작용 타겟 비우기
            curInteractTarget = null;
            // 종료
            return;
        }
        // 근처에 상호작용 가능한 물체가 없다면
        else if (targets.Length == 0)
            // 종료
            return;

        // 저장할 타겟
        Collider target = null;
        // 타겟과의 거리, 각도에 대한 점수
        float targetScore = float.MaxValue;

        // 물체들의 수만큼
        foreach (var trg in targets)
        {
            // 상호작용 인터페이스가 없다면
            if (!trg.TryGetComponent(out IInteractable interactable))
                // 건너뛰기
                continue;

            // 물체와의 거리 저장
            var dis = Vector3.Distance(trg.transform.position, transform.forward);
            // 물체와의 방향 저장
            var dir = (trg.transform.position - transform.position).normalized;
            // 물체와의 각도 저장
            var angle = Vector3.Angle(transform.forward, dir);

            // 상호작용 가능한 거리와 각도 조건에 맞지 않다면
            if (!CheckCanInteract(dis, angle))
                // 건너뛰기
                continue;

            // 거리와 각도(10도 == 1f)에 다른 점수 저장
            var score = dis + angle * 0.1f;

            // 점수가 타겟 점수보다 작다면
            if (score < targetScore)
            {
                // 타겟 점수 갱신
                targetScore = score;
                // 타겟 갱신
                target = trg;
            }
        }

        // 현재 상호작용 타겟에 저장
        curInteractTarget = target;
    }

    // 상호작용 가능 여부 확인 함수
    private bool CheckCanInteract(float dis, float angle)
    {
        // 물체와 가깝다면
        if (dis <= maxInteractDistance * 0.5f)
            // 각도가 최대 상호작용 각도 안에 있다면 true 반환
            return angle <= maxInteractAngle;
        // 물체와 적당히 가깝다면
        else if (dis <= maxInteractDistance)
            // 각도가 최대 상호작용 각도의 절반 안에 있다면 true 반환
            return angle <= maxInteractAngle * 0.5f;

        // 멀다면 false 반환
        return false;
    }

    // 상호작용 실행 함수
    private void OnInteract(InteractEvent data)
    {
        // 현재 상호작용 타겟이 비어있지 않고, 상호작용 인터페이스를 가지고 있다면
        if (curInteractTarget != null && curInteractTarget.TryGetComponent(out IInteractable interactable))
            // 상호작용 함수 실행(전리품 상자 : UI 활성화)
            interactable.Interact();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxInteractDistance);
    }
}