// 01. Scripts/Interactable/EscapeTrigger.cs
using System.Collections;
using UnityEngine;

/// <summary>
/// 탈출 구역의 콜라이더에 부착되어 플레이어의 체류 시간을 측정하고 탈출 이벤트를 발행합니다.
/// </summary>
[RequireComponent(typeof(Collider))] // 이 스크립트는 반드시 Collider(IsTrigger)가 필요함을 명시
public class EscapeTrigger : MonoBehaviour, IPlayerCancelHandler
{
    [Header("탈출 설정")]
    [Tooltip("탈출에 필요한 대기 시간(초)")]
    [SerializeField] private float escapeDelayTime = 5f;

    // 메모리 상에서 실행 중인 탈출 타이머를 추적하기 위한 포인터
    private Coroutine escapeCoroutine;

    /// <summary>
    /// 물리 엔진이 플레이어의 진입을 감지했을 때 호출됩니다.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 들어온 객체가 플레이어인지 해시값 태그로 비교 (메모리상 가장 빠른 문자열 비교 방식)
        if (other.CompareTag("Player"))
        {
            // 타이머 코루틴을 메모리에 할당하고 실행
            escapeCoroutine = StartCoroutine(EscapeRoutine());
            Debug.Log("플레이어 감지");
        }
    }

    /// <summary>
    /// 플레이어가 영역 밖으로 나갔을 때 호출됩니다.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 범위를 벗어나면 탈출 프로세스 전면 취소
            CancelEscape();
            Debug.Log("플레이어 이탈");
        }
    }

    /// <summary>
    /// IPlayerCancelHandler의 구현부. 플레이어가 외부 요인(피격, 구르기 등)으로 인해 행동이 취소될 때 호출.
    /// </summary>
    public void OnCancel()
    {
        CancelEscape();
    }

    /// <summary>
    /// 진행 중인 코루틴을 강제 종료하고 메모리를 정리하는 메서드
    /// </summary>
    private void CancelEscape()
    {
        // 코루틴이 현재 실행 중이라면 (메모리 주소가 null이 아니라면)
        if (escapeCoroutine != null)
        {
            StopCoroutine(escapeCoroutine); // 코루틴 실행 중단
            escapeCoroutine = null;         // 포인터 초기화 (가비지 컬렉터에게 넘김)
        }

        // TODO: UI 매니저에게 진행도를 0으로 초기화하라는 이벤트를 발행할 수 있는 공간입니다.
    }

    /// <summary>
    /// 매 프레임마다 시간을 누적하여 탈출을 검증하는 독립적인 논리 스레드(로우 레벨 코루틴)
    /// </summary>
    private IEnumerator EscapeRoutine()
    {
        float timer = 0f;

        // 타이머가 목표 시간에 도달할 때까지 루프
        while (timer < escapeDelayTime)
        {
            // 이전 프레임부터 현재 프레임까지 걸린 시간(델타 타임)을 누적
            timer += Time.deltaTime;
            Debug.Log(timer);
            // TODO: UI 매니저에게 (timer / escapeDelayTime) 비율을 보내 게이지 바를 채우게 할 수 있습니다.

            // 다음 프레임 렌더링까지 대기 (제어권을 유니티 엔진에 양보)
            yield return null;
        }

        // 루프를 빠져나왔다 == 지정된 시간을 무사히 버텼다.
        // 게임 종료 이벤트를 허공에 발행 (누가 듣든 상관하지 않음 -> 완전한 디커플링)
        Subject<IGameEndingHandler>.Publish(h => h.OnGameEnding());

        // 이벤트가 여러 번 발행되는 것을 막기 위해 코루틴 메모리 정리
        CancelEscape();
    }
}