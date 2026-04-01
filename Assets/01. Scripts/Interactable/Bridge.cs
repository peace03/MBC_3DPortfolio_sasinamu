using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bridge : MonoBehaviour, IInteractable
{
    [Header("수리 재료 설정")]
    [SerializeField] private List<NeedItem> _needItems;

    [Header("상태 변화 대상")]
    [SerializeField] private GameObject _bridgeBroken;
    [SerializeField] private GameObject _bridgeFixed;

    [Header("월드 스페이스 UI")]
    [SerializeField] private GameObject _uiCanvas;
    [SerializeField] private TextMeshProUGUI _reqText;
    [SerializeField] private Image _uiBackground;
    [SerializeField] private Color _colorCanRepair = Color.green;
    [SerializeField] private Color _colorCannotRepair = Color.red;
    [SerializeField] private float _uiShowDistance = 50f; // UI가 켜지는 기준 거리

    [SerializeField] private Transform _playerTransform;
    private bool _canRepairNow = false;
    private float _uiRefreshTimer = 0f; // 메모리 최적화를 위한 타이머

    private void Start()
    {
        _bridgeBroken.SetActive(true);
        _bridgeFixed.SetActive(false);
        _uiCanvas.SetActive(false);
    }

    private void Update()
    {
        // 플레이어를 찾지 못했다면 연산을 건너뜁니다.
        if (_playerTransform == null) return;

        // [핵심 변경점] 물리 트리거 대신 수학적 거리(Distance)를 계산합니다. (절대 실패하지 않음)
        float distance = Vector3.Distance(transform.position, _playerTransform.position);

        if (distance <= _uiShowDistance)
        {
            // 1. 거리에 들어오면 UI 캔버스를 켭니다.
            if (!_uiCanvas.activeSelf) _uiCanvas.SetActive(true);

            // 2. 매 프레임 텍스트를 연산하지 않고 0.2초 주기로만 인벤토리를 검사합니다.
            _uiRefreshTimer += Time.deltaTime;
            if (_uiRefreshTimer >= 0.2f)
            {
                RefreshUI();
                _uiRefreshTimer = 0f;
            }
        }
        else
        {
            // 거리를 벗어나면 UI를 끕니다.
            if (_uiCanvas.activeSelf) _uiCanvas.SetActive(false);
        }
        //Debug.Log(distance);
    }

    private void RefreshUI()
    {
        string reqTextStr = "";
        Subject<IRepairableHandler>.Publish(h =>
        {
            _canRepairNow = h.CheckCanRepair(_needItems, out reqTextStr);
        });

        _reqText.text = "E키를 누르세요\n" + reqTextStr;
        _uiBackground.color = _canRepairNow ? _colorCanRepair : _colorCannotRepair;
    }

    // [IInteractable 구현] - Box.cs와 동일한 레이더 응답 로직
    public void Interact()
    {
        if (!_canRepairNow)
        {
            Debug.Log("다리: 재료가 부족하여 고칠 수 없습니다.");
            return;
        }

        // Presenter에게 재료 차감을 요청
        Subject<IRepairableHandler>.Publish(h =>
        {
            bool success = h.ConsumeRepairItems(_needItems);
            if (success)
            {
                CompleteRepair();
            }
        });
    }

    private void CompleteRepair()
    {
        Debug.Log("다리 수리 완료!");

        _bridgeBroken.SetActive(false);
        _bridgeFixed.SetActive(true);
        _uiCanvas.SetActive(false);

        // 수리 완료 후 레이더에 더 이상 잡히지 않도록 콜라이더를 끕니다.
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        this.enabled = false; // 연산(Update) 완전 종료
    }
}