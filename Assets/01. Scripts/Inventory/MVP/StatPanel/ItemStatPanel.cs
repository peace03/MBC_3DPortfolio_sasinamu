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
    private List<StatTextLine> _statTexts;

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
        _statTexts = new List<StatTextLine>();

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
        _itemName.text = item._data.Name;
        _weight.text = item._data.Weight.ToString() + "kg";
        _explain.text = item._data.Explain;

        //아이템 스탯 리스트 초기화
        var stats = item.GetItemStats();
        for (int i = 0; i < Mathf.Max(stats.Count, _statTexts.Count); i++)
        {
            if (i >= _statTexts.Count) //아이템 스탯 개수가 리스트 크기 초과했다면 - 생성
            {
                //Debug.Log("1");
                GameObject go = Instantiate(_statText, transform);
                StatTextLine stl = go.GetComponent<StatTextLine>();
                _statTexts.Add(stl);
                //Debug.Log("2");
            }
            if (i < stats.Count) //인덱스가 아이템 스탯 개수보다 적으면 초기화
            {
                //Debug.Log("3");
                _statTexts[i].SetStatText(stats[i].statName, stats[i].statValue);
                _statTexts[i].gameObject.SetActive(true);
                //Debug.Log("4");
            }
            else //인덱스가 아이템 스탯 개수보다 많으면 비활성화
            {
                _statTexts[i].gameObject.SetActive(false);
            }
        }
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
