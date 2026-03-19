using UnityEngine;
using UnityEngine.UI;

public class VirtualSlot : MonoBehaviour
{
    [SerializeField] private Image _image;
    private int _index;
    public int Index => _index;

    private bool _isFromEquipment; //from 슬롯이 장비 인벤토리에서 온 것인가?
    public bool IsFromEquipment => _isFromEquipment;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetVirtualSlot(bool isFromEquipment, Sprite image, int index)
    {
        _isFromEquipment = isFromEquipment;
        _image.sprite = image;
        _index = index;
    }

    public void ToggleActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
