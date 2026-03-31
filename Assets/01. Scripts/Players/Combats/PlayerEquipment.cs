using UnityEngine;

public class PlayerEquipment : MonoBehaviour, IEquipWear
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
        
        Subject<IEquipWear>.Attach(this);
    }

    private void OnDisable()
    {
        // 장전 이벤트 구독 해제

        Subject<IEquipWear>.Detach(this);
    }

    public void OnGunDestroy(int index, Item item)
    {
        Debug.Log("아이템 파괴 예정");
    }

    public void OnGunSwap(int index1, Item item1, int index2, Item item2)
    {
        equipments[index1] = item1;
        equipments[index2] = item2;
    }

    public void OnEquipWear(int index, Item item) => equipments[index] = item;



    // 장전 관리 함수
}