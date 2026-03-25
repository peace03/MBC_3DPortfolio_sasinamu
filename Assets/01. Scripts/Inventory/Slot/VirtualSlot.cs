using UnityEngine;
using UnityEngine.UI;

public class VirtualSlot : MonoBehaviour
{
    [SerializeField] private Image _image;
    private int _index;
    //private bool _isFromEquipment; //from 슬롯이 장비 인벤토리에서 온 것인가?
    private SlotType _slotType;
    private EquipType _equipType;    //드래그 시작 슬롯의 장비 타입

    public int Index => _index;
    //public bool IsFromEquipment => _isFromEquipment;
    public SlotType SlotType => _slotType;
    public EquipType EquipType => _equipType;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetVirtualSlot(SlotType slotType, EquipType equipType, Sprite image, int index)
    {
        _equipType = equipType;
        //_isFromEquipment = isFromEquipment;
        _slotType = slotType;
        _image.sprite = image;
        _index = index;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
