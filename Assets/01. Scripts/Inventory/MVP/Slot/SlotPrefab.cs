using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class SlotPrefab : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private SlotType _slotType;
    private EquipType _equipType = EquipType.None;
    [SerializeField] private Image _backImage;      //배경 이미지
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemNum;
    [SerializeField] private Slider _slider;

    [SerializeField] private Sprite defaultImage;    //아이템이 사라지면 넣을 기본이미지
    [SerializeField] private Sprite clickImage;     //슬롯 클릭 이미지

    private int _index;             //해당 슬롯의 인덱스
    private bool isClicked = false;         //선택되었는지?

    public int Index => _index;

    private VirtualSlot _virtualSlot;       //아이템 드래그 표시 슬롯

    //private Action<SwapType, int, int> onDropSlot;   //시작 인덱스, 종료 인덱스
    //private Action<SlotSource, int> onCusorEnter;    //커서가 슬롯 들어갈 때 호출
    //private Action onCusorExit;                      //커서가 슬롯 벗어날 때 호출

    private void Awake()
    {
        defaultImage = _image.sprite;
    }

    public void Initialize(int index, SlotType slotType, VirtualSlot virtualSlot)
    {
        _index = index;
        _slotType = slotType;
        _virtualSlot = virtualSlot;
        SetSlot();
    }

    //public void OnButton()
    //{
    //    Debug.Log("버튼 클릭됨!");
    //}

    #region SetSlot
    //Countable일 때
    public void SetSlot(Sprite image, string itemName, string itemNum)
    {
        _image.sprite = image;
        _itemName.text = itemName;
        _itemName.gameObject.SetActive(true);
        _itemNum.text = itemNum;
        _itemNum.gameObject.SetActive(true);
        _slider.gameObject.SetActive(false);
    }
    //UnCountable일 때
    public void SetSlot(Sprite image, string itemName)
    {
        _image.sprite = image;
        _itemName.text = itemName;
        _itemName.gameObject.SetActive(true);
        _itemNum.gameObject.SetActive(false);
        _slider.gameObject.SetActive(false);
    }
    //Consumable일 때 (치료, 총 템밖에 없음)
    public void SetSlot(Sprite image, string itemName, float durabilityValue, float maxDurability)
    {
        _image.sprite = image;
        _itemName.text = itemName;
        _slider.maxValue = maxDurability;
        _slider.value = durabilityValue;
        _itemName.gameObject.SetActive(true);
        _itemNum.gameObject.SetActive(false);
        _slider.gameObject.SetActive(true);
        Debug.Log("호출");
    }
    //비어있는 슬롯일 때
    public void SetSlot()
    {
        _image.sprite = defaultImage;
        _itemName.gameObject.SetActive(false);
        _itemNum.gameObject.SetActive(false);
        _slider.gameObject.SetActive(false);
    }
    #endregion

    #region 마우스 드래그 앤 드롭
    public void OnBeginDrag(PointerEventData eventData)
    {
        //아이템이 없으면 종료
        if (!_itemName.gameObject.activeSelf) return;
        _virtualSlot.SetVirtualSlot(_slotType, _equipType, _image.sprite, _index);
        _virtualSlot.SetActive(true);
        _virtualSlot.gameObject.GetComponent<Image>().raycastTarget = false;

        //Debug.Log($"선택된 슬롯 인덱스: {_Index}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //가상슬롯 비활성화면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        _virtualSlot.gameObject.transform.position = eventData.position;
        //Debug.Log("");
    }

    public void OnDrop(PointerEventData eventData)
    {
        //가상슬롯 비활성화면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        //장비 슬롯 아이템일 경우 비어있는 칸 아니면 슬롯 교환 안함
        if (_virtualSlot.SlotType == SlotType.Equip && _itemName.gameObject.activeSelf == true) return;

        //슬롯 교환 이벤트 발생
        Subject<ISlotExchangeHandler>.Publish(h => 
        h.OnExchangeSlot(_virtualSlot.SlotType, _virtualSlot.Index, _slotType, _index));
        //Debug.Log($"SwapType: {swapType}");
        //onDropSlot?.Invoke(swapType, _virtualSlot.Index, _index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Subject<ISlotClickRightHandler>.Publish(h => h.OnAllBtnSetActive(false));
        //가상슬롯 비활성화면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        _virtualSlot.gameObject.GetComponent<Image>().raycastTarget = true;
        _virtualSlot.SetActive(false);

        //슬롯 배경이미지 변경
        _backImage.sprite = defaultImage;
        isClicked = !isClicked;
    }
    #endregion

    #region 마우스 커서
    public void OnPointerEnter(PointerEventData eventData)
    {
        //아이템 상태창 세팅
        Subject<ICusorPointerHandler>.Publish(h => h.OnCusorSlotIn(_slotType, _index));
        //Debug.Log("상태창 활성화");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //아이템 상태창 비활성화
        Subject<ICusorPointerHandler>.Publish(h => h.OnCusorSlotExit());
        //Debug.Log("상태창 비활성화");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_image.sprite == defaultImage) return;
        //좌 우클릭 둘다
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isClicked) _backImage.sprite = defaultImage;
            else _backImage.sprite = clickImage;
            isClicked = !isClicked;
            //클릭한 슬롯의 정보 P에 저장
            Subject<ISlotClickHandler>.Publish(h => h.OnSlotLeftClick(_slotType, _index));
        }
        //우클릭이면 버튼 띄워주기
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Subject<ISlotClickHandler>.Publish(h => h.OnSlotRightClick(_slotType, _index));
            Subject<ISlotClickRightHandler>.Publish(h => h.OnSlotClickRight(gameObject.transform));
        }
    }
    #endregion
}
