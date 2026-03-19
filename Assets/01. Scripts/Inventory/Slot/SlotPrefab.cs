using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class SlotPrefab : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image _image;
    private TextMeshProUGUI _itemName;
    private TextMeshProUGUI _itemNum;

    private Sprite defaultImage;    //아이템이 사라지면 넣을 기본이미지
    private int _index;             //해당 슬롯의 인덱스
    public int Index => _index;

    private VirtualSlot _virtualSlot;       //아이템 드래그 표시 슬롯

    private Action<SwapType, int, int> onDropSlot;   //시작 인덱스, 종료 인덱스
    private Action<SlotSource, int> onCusorEnter;    //커서가 슬롯 들어갈 때 호출
    private Action onCusorExit;                      //커서가 슬롯 벗어날 때 호출

    private void Awake()
    {
        _image = transform.GetChild(0).GetComponent<Image>();
        _itemName = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _itemNum = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        defaultImage = _image.sprite;
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

    #region SetSlot
    //Countable일 때
    public void SetSlot(Sprite image, string itemName, string itemNum)
    {
        _image.sprite = image;
        _itemName.text = itemName;
        _itemName.gameObject.SetActive(true);
        _itemNum.text = itemNum;
        _itemNum.gameObject.SetActive(true);
    }
    //UnCountable일 때
    public void SetSlot(Sprite image, string itemName)
    {
        _image.sprite = image;
        _itemName.text = itemName;
        _itemName.gameObject.SetActive(true);
        _itemNum.gameObject.SetActive(false);
    }
    //비어있는 슬롯일 때
    public void SetSlot()
    {
        _image.sprite = defaultImage;
        _itemName.gameObject.SetActive(false);
        _itemNum.gameObject.SetActive(false);
    }
    #endregion

    #region 마우스 드래그 앤 드롭
    public void OnBeginDrag(PointerEventData eventData)
    {
        //아이템이 없으면 종료
        if (!_itemName.gameObject.activeSelf) return;
        _virtualSlot.SetVirtualSlot(false, _image.sprite, _index);
        _virtualSlot.ToggleActive(true);
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
        //드래그 시작과 종료 인벤토리 방향 타입
        SwapType swapType;
        if (_virtualSlot.IsFromEquipment) swapType = SwapType.EquipToBag;
        else swapType = SwapType.BagToBag;
        //Debug.Log($"SwapType: {swapType}");
        onDropSlot?.Invoke(swapType, _virtualSlot.Index, _index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //가상슬롯 비활성화면 종료
        if (!_virtualSlot.gameObject.activeSelf) return;
        _virtualSlot.gameObject.GetComponent<Image>().raycastTarget = true;
        _virtualSlot.ToggleActive(false);
        //Debug.Log($"보내준 슬롯 인덱스: {_Index}");
    }
    #endregion

    #region 마우스 커서
    public void OnPointerEnter(PointerEventData eventData)
    {
        //이벤트 발생
        //Debug.Log($"현재 마우스 커서\n이름: {_itemName.text}\t개수: {_itemNum.text}");
        onCusorEnter?.Invoke(SlotSource.Bag, _index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("마우스 커서 이탈!!!!");
        onCusorExit?.Invoke();
    }
    #endregion
}
