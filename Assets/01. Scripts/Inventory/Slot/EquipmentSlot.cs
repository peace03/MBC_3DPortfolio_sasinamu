using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//public enum EquipType //장비의 타입과 슬롯의 타입
//{
//    Weapon,
//    Bag,
//    Head,
//    Body
//}

public class EquipmentSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image _image;                               //장비 슬롯 이미지
    private Sprite defaultImage;                        //아이템이 사라지면 넣을 기본이미지
    private TextMeshProUGUI _itemName;                  //장비 슬롯 이름
    [SerializeField] private string defaultTextName;    //장비 슬롯 기본 이름

    private VirtualSlot _virtualSlot;               //가상 슬롯
    private Action<SwapType, int, int> onDropSlot;  //아이템 드롭되면 교체 이벤트 발생
    private Action<SlotSource, int> onCusorEnter;   //커서가 슬롯 들어갈 때 호출
    private Action onCusorExit;                     //커서가 슬롯 벗어날 때 호출

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

    public void Initialize(int index, VirtualSlot virtualSlot,
        Action<SwapType, int, int> callback1,
        Action<SlotSource, int> callback2,
        Action callback3)
    {
        _index = index;
        _virtualSlot = virtualSlot;
        onDropSlot = callback1;
        onCusorEnter = callback2;
        onCusorExit = callback3;
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
        _virtualSlot.SetVirtualSlot(true, _image.sprite, Index);
        _virtualSlot.ToggleActive(true);
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
        SwapType swapType;
        if (_virtualSlot.IsFromEquipment) swapType = SwapType.EquipToEquip;
        else swapType = SwapType.BagToEquip;
        //Debug.Log($"SwapType: {swapType}");
        onDropSlot?.Invoke(swapType, _virtualSlot.Index, _index);

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //가상 슬롯 비활성화 상태면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        _virtualSlot.gameObject.GetComponent<Image>().raycastTarget = true;
        _virtualSlot.ToggleActive(false);
    }
    #endregion

    #region 마우스 커서
    public void OnPointerEnter(PointerEventData eventData)
    {
        //이벤트 발생
        //Debug.Log($"현재 마우스 커서\n이름: {_itemName.text}");
        onCusorEnter?.Invoke(SlotSource.Equipment, _index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("마우스 커서 이탈!!!!");
        onCusorExit?.Invoke();
    }
    #endregion
}
