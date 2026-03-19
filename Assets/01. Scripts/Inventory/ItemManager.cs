using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    //싱글톤 ? 인스턴스?
    [Header("의존성 주입")] //BootStrapper 역할: P에 MV 연결해줌
    public InventoryView _view; //인스펙터에서 연결
    private InventoryModel _model;
    private InventoryPresenter _present;

    //[Header("전체 아이템 데이터(Scriptable Object)")]
    [SerializeField] private List<ItemData> _itemDatas; //가능하면 Dictionary : id로 찾기 쉬워서
    //[SerializeField] private Dictionary<int, ItemData> _itemD; //가능하면 Dictionary : id로 찾기 쉬워서

    private void Awake()
    {
        //presenter에 M, V 연결
        _model = new InventoryModel();
        _present = new InventoryPresenter(_view, _model, this);

        //아이템 데이터(SO) 불러오기
        _itemDatas = new List<ItemData>(Resources.LoadAll<ItemData>("ItemDatas"));
    }
    private void Start()
    {
        //presenter 설정 초기화
        _present.InitializePresenter(_itemDatas);
    }

    //아이템 인스턴스 생성
    public Item CreateItemInstance(int id) // int id
    {
        //Item newItem;
        switch (ReturnItemData_One(id))
        {
            case BulletData bulletData:
                return new BulletItem(bulletData, 3);
            case CountableData countableData:
                return new CountableItem(countableData, 3);
            case CureKitData cureKitData:
                return new CureKitItem(cureKitData, cureKitData.maxDurability);
            case GunData gunData:
                return new GunItem(gunData, Random.Range(1, gunData.maxAmmo), gunData.maxDurability);
            case ConsumableData consumableData:
                return new ConsumableItem(consumableData, consumableData.maxDurability);
            case FoodData foodData:
                return new FoodItem(foodData);
            case BagData bagData:
                return new BagItem(bagData);
            case ItemData itemData1:
                return new Item(itemData1);
            default:
                return null;
        }
        //Debug.Log($"아이템 타입: {newItem.GetType()}");
    }

    //ItemData 1개만 반환
    public ItemData ReturnItemData_One(int id)
    {
        foreach (var itemdata in _itemDatas)
        {
            if (itemdata._id == id) return itemdata;
        }
        return null;

        //return _itemD.ContainsKey(id) ? _itemD[id] : null; 
    }

}
