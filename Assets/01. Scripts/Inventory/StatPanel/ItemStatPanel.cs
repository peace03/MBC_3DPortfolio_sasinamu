using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//아이템 상태창 패널
public class ItemStatPanel : MonoBehaviour
{
    private Transform _panel;
    private TextMeshProUGUI _itemName;
    private TextMeshProUGUI _weight;
    private TextMeshProUGUI _explain;
    [SerializeField] private GameObject _statText;
    private List<GameObject> _statTexts;

    private RectTransform _rect;
    private RectTransform parentRect;
    private Canvas _canvas;
    private Vector2 offSet;             //상태창 패널 오프셋 조절

    private void Awake()
    {
        //아이템 스탯 패널 초기화
        _panel = transform;
        _itemName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _weight = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _explain = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        _statTexts = new List<GameObject>();

        //마우스 포인터 레이캐스트 설정
        _rect = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        GetComponent<Image>().raycastTarget = false;
        offSet = new Vector2(10f, 0f);

        //panel 비활성화로 시작
        InActiveToggle();
    }

    private void LateUpdate()
    {
        Vector2 mousePos;
        if (gameObject.activeSelf)
        {
            //마우스 위치 반환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                Input.mousePosition,
                null,
                out mousePos
                );
            _rect.anchoredPosition = mousePos + offSet;
        }
    }

    public void SetStatPanel(Item item)
    {
            _itemName.text = item._data._name;
            _weight.text = item._data._weight.ToString() + "kg";
            _explain.text = item._data._explain;
    }

    public void ActiveToggle()
    {
        gameObject.SetActive(true);
    }
    public void InActiveToggle()
    {
        gameObject.SetActive(false);
    }
}
