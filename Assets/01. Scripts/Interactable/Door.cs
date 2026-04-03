using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour, IInteractable
{
    [Header("문 열기 조건 설정")]
    [SerializeField] private List<NeedItem> _needItems;
    [SerializeField] private bool _consumeItems = true; // 열쇠/카드키처럼 소모하지 않을 거라면 false로 체크

    [Header("상태 변화 대상 (물리적 회전)")]
    [SerializeField] private Transform _doorHinge; // 회전축(Pivot) 역할을 할 Transform
    [SerializeField] private Vector3 _openLocalEulerAngles = new Vector3(0, 90, 0); // 열렸을 때의 목표 각도
    [SerializeField] private float _openSpeed = 2f; // 문이 열리는 속도 상수
    [SerializeField] private GameObject fogObject;

    [Header("월드 스페이스 UI")]
    [SerializeField] private GameObject _uiCanvas;
    [SerializeField] private TextMeshProUGUI _reqText;
    [SerializeField] private Image _uiBackground;
    [SerializeField] private Color _colorCanOpen = Color.green;
    [SerializeField] private Color _colorCannotOpen = Color.red;
    [SerializeField] private float _uiShowDistance = 3f;

    [SerializeField] private Transform _playerTransform;
    private bool _canOpenNow = false;
    private float _uiRefreshTimer = 0f;
    private bool _isOpenEnded = false; // 문이 이미 열렸는지 상태를 잠그는 플래그

    private void Start()
    {
        _uiCanvas.SetActive(false);
    }

    private void Update()
    {
        if (_isOpenEnded) return; // 문이 완전히 열렸다면 더 이상 거리를 계산하지 않고 CPU 사이클을 아낍니다.

        // 인스펙터에 플레이어가 제대로 할당되지 않았을 경우 메모리 참조 에러 방지
        if (_playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, _playerTransform.position);

        if (distance <= _uiShowDistance)
        {
            if (!_uiCanvas.activeSelf) _uiCanvas.SetActive(true);

            // 0.2초 주기 최적화 (가비지 컬렉터 부하 방지)
            _uiRefreshTimer += Time.deltaTime;
            if (_uiRefreshTimer >= 0.2f)
            {
                RefreshUI();
                _uiRefreshTimer = 0f;
            }
        }
        else
        {
            if (_uiCanvas.activeSelf) _uiCanvas.SetActive(false);
        }
        //Debug.Log(distance);
    }

    private void RefreshUI()
    {
        string reqTextStr = "";

        // 기존 Bridge 수리에 썼던 인터페이스를 그대로 활용하여 인벤토리를 뒤집니다.
        Subject<IRepairableHandler>.Publish(h =>
        {
            _canOpenNow = h.CheckCanRepair(_needItems, out reqTextStr);
        });

        _reqText.text = "E키를 눌러 열기\n" + reqTextStr;
        _uiBackground.color = _canOpenNow ? _colorCanOpen : _colorCannotOpen;
    }

    // [IInteractable 구현] - PlayerInteract가 레이더로 쏴주는 진입점
    public void Interact()
    {
        if (_isOpenEnded) return;

        if (!_canOpenNow)
        {
            Debug.Log("도어: 조건이 만족되지 않아 열 수 없습니다.");
            return;
        }

        // 제1원칙 분기 처리: 아이템을 소모할 것인가?
        if (_consumeItems)
        {
            // 재료 소모를 요청
            Subject<IRepairableHandler>.Publish(h =>
            {
                bool success = h.ConsumeRepairItems(_needItems);
                if (success) OpenDoor(); // 소모 성공 시 개방
            });
        }
        else
        {
            // 소모가 false라면, Check 조건만 통과했으므로 인벤토리 차감 없이 바로 문 개방
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        _isOpenEnded = true;
        _uiCanvas.SetActive(false); // UI를 끕니다.
        fogObject.SetActive(false);

        // 문이 열렸으므로 레이더에 더 이상 잡히지 않도록 콜라이더를 끕니다.
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 메인 스레드를 멈추지 않고 비동기적으로 문을 엽니다.
        StartCoroutine(OpenDoorRoutine());
    }

    // 드라이 런(Dry Run): 데이터 프름 추적
    private IEnumerator OpenDoorRoutine()
    {
        // 1. 현재 각도와 목표 각도를 메모리에 캐싱
        Quaternion startRot = _doorHinge.localRotation;
        Quaternion endRot = Quaternion.Euler(_openLocalEulerAngles);
        float time = 0f;

        // 2. time이 1.0(100%)에 도달할 때까지 프레임마다 반복
        while (time < 1f)
        {
            time += Time.deltaTime * _openSpeed;
            // 3. 구면 선형 보간(Slerp)으로 부드러운 각도 계산 및 대입
            _doorHinge.localRotation = Quaternion.Slerp(startRot, endRot, time);
            yield return null; // 4. 다음 프레임까지 대기
        }

        // 5. while문 종료 후 미세한 부동소수점 오차를 완벽한 목표 각도로 강제 정렬
        _doorHinge.localRotation = endRot;
        this.enabled = false; // Update 연산 완전 종료
    }
}