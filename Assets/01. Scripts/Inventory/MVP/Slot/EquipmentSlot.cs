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

public class EquipmentSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private SlotType _slotType = SlotType.Equip;
    [SerializeField] private EquipType _equipType;
    private Image _image;                               //장비 슬롯 이미지
    private Sprite defaultImage;                        //아이템이 사라지면 넣을 기본이미지
    private TextMeshProUGUI _itemName;                  //장비 슬롯 이름
    [SerializeField] private string defaultTextName;    //장비 슬롯 기본 이름

    private VirtualSlot _virtualSlot;                   //가상 슬롯

    private int _index; //슬롯 인덱스
    public int Index => _index;

    private void Awake()
    {
        //코드로 이미지, 네임 로드
        _image = transform.GetChild(0).GetComponent<Image>();
        _itemName = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
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
    }
    public void SetSlot()
    {
        _image.sprite = defaultImage;
        _itemName.text = defaultTextName;
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
        //가상 슬롯 비활성화 상태면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        Debug.Log("아이템 드롭!!");

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
    }
    #endregion

    #region 마우스 커서
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("마우스 커서 이탈!!!!");
        //onCusorExit?.Invoke();
    }
    #endregion
}
