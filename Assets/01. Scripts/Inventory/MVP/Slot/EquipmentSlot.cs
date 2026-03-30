using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EquipType //장비의 타입과 슬롯의 타입
{
    Weapon,
    Bag,
    Head,
    Body,
    None
}

public class EquipmentSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, 
    IDropHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, 
    IPointerClickHandler
{
    private SlotType _slotType = SlotType.Equip;
    [SerializeField] private EquipType _equipType;
    private Image _backImage;                           //배경 이미지
    private Image _image;                               //장비 슬롯 이미지
    private Sprite defaultImage;                        //아이템이 사라지면 넣을 기본이미지
    private TextMeshProUGUI _itemName;                  //장비 슬롯 이름
    private Slider _slider;
    [SerializeField] private Sprite clickImage;         //클릭시 넣을 이미지
    [SerializeField] private Sprite unClickImage;       //한번 더 클릭시 기본이미지
    [SerializeField] private string defaultTextName;    //장비 슬롯 기본 이름

    private VirtualSlot _virtualSlot;                   //가상 슬롯
    private bool isClicked = false;                             //클릭 되었는지?
    private int _index; //슬롯 인덱스
    public int Index => _index;

    private void Awake()
    {
        //코드로 이미지, 네임 로드
        _backImage = transform.GetChild(0).GetComponent<Image>();
        _image = transform.GetChild(1).GetComponent<Image>();
        _itemName = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        _slider = transform.GetChild(3).GetComponent<Slider>();
        defaultImage = _image.sprite;
        SetSlot();
    }

    public void Initialize(int index, VirtualSlot virtualSlot)
    {
        _index = index;
        _virtualSlot = virtualSlot;
    }

    public void SetSlot(Sprite sprite, string name)
    {
        _image.sprite = sprite;
        _itemName.text = name;
        _slider.gameObject.SetActive(false);
    }
    public void SetSlot(Sprite sprite, string name, float durabilityValue, float maxDurability)
    {
        _image.sprite = sprite;
        _itemName.text = name;
        _slider.maxValue = maxDurability;
        _slider.value = durabilityValue;
        _slider.gameObject.SetActive(true);
    }
    public void SetSlot()
    {
        _image.sprite = defaultImage;
        _itemName.text = defaultTextName;
        _slider.gameObject.SetActive(false);
    }

    #region MouseDrag
    public void OnBeginDrag(PointerEventData eventData)
    {
        //아이템이 없다면 종료
        if (_itemName.text == defaultTextName) return;
        _virtualSlot.SetVirtualSlot(_slotType, _equipType, _image.sprite, Index);
        _virtualSlot.SetActive(true);
        _virtualSlot.gameObject.GetComponent<Image>().raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        //가상 슬롯 비활성화 상태면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        _virtualSlot.gameObject.transform.position = eventData.position;
    }
    public void OnDrop(PointerEventData eventData)
    {
        Subject<ISlotClickRightHandler>.Publish(h => h.OnAllBtnSetActive(false));
        //가상 슬롯 비활성화 상태면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        //Debug.Log("아이템 드롭!!");

        //슬롯 교환 이벤트 발생
        Subject<ISlotExchangeHandler>.Publish(h =>h.OnExchangeSlot
        (_virtualSlot.SlotType, _virtualSlot.Index, _slotType, _index));

        //Debug.Log($"SwapType: {swapType}");
        //onDropSlot?.Invoke(swapType, _virtualSlot.Index, _index);

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //가상 슬롯 비활성화 상태면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        _virtualSlot.gameObject.GetComponent<Image>().raycastTarget = true;
        _virtualSlot.SetActive(false);

        //슬롯 배경이미지 변경
        _backImage.sprite = unClickImage;
        isClicked = !isClicked;
    }
    #endregion

    #region 마우스 커서
    public void OnPointerEnter(PointerEventData eventData)
    {
        //아이템 상태창 세팅
        Subject<ICusorPointerHandler>.Publish(h => h.OnCusorSlotIn(_slotType, _index));
        Debug.Log("상태창 활성화");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //아이템 상태창 비활성화
        Subject<ICusorPointerHandler>.Publish(h => h.OnCusorSlotExit());
        //Debug.Log("상태창 비활성화");
    }
    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_image.sprite == defaultImage) return;
        //좌 우클릭 둘다
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isClicked) _backImage.sprite = unClickImage;
            else _backImage.sprite = clickImage;
            isClicked = !isClicked;
            //클릭한 슬롯의 정보 P에 저장
            Subject<ISlotClickHandler>.Publish(h => h.OnSlotLeftClick(_slotType, _index));
        }
        //우클릭이면 버튼 띄워주기
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Subject<ISlotClickHandler>.Publish(h => h.OnSlotRightClick(_slotType, _index));
            Subject<ISlotClickRightHandler>.Publish(h => h.OnSlotClickRight(gameObject.transform.GetChild(0).transform));
        }
    }
}
