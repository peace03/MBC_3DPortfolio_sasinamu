using UnityEngine;

public class PlayerEquipment : MonoBehaviour, IEquipmentSlotHandler
{
    [SerializeField] private int equipmentCount;
    [SerializeField] private Item[] equipments;        // 0, 1 : 총 / 2 : 가방 / 3 : 방어구

    private void Awake()
    {
        equipments = new Item[equipmentCount];
    }

    private void OnEnable()
    {
        // 장전 이벤트 구독
        
        Subject<IEquipmentSlotHandler>.Attach(this);
    }

    private void OnDisable()
    {
        // 장전 이벤트 구독 해제

        Subject<IEquipmentSlotHandler>.Detach(this);
    }

    public void OnEquipmentSlot(InventoryModel model)
    {
        for (int i = 0; i < equipmentCount; i++)
            equipments[i] = model.GetItem(i);
    }



    // 장전 관리 함수
}