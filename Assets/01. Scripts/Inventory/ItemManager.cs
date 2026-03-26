using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [Header("전체 아이템 데이터(Scriptable Object)")]
    [SerializeField] private List<ItemData> _itemDatas;

    [Header("플레이어 위치")]
    [SerializeField] private Transform _player;

    private void Awake()
    {
        _itemDatas = new List<ItemData>(Resources.LoadAll<ItemData>("ItemDatas"));
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
                return new CureKitItem(cureKitData, cureKitData.MaxDurability);
            case GunData gunData:
                return new GunItem(gunData, Random.Range(1, gunData.MaxAmmo), gunData.MaxDurability);
            case ConsumableData consumableData:
                return new ConsumableItem(consumableData, consumableData.MaxDurability);
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
            if (itemdata.ID == id) return itemdata;
        }
        return null;

        //return _itemD.ContainsKey(id) ? _itemD[id] : null; 
    }

    //아이템 월드에 생성
    public void CreateItemObjInWorld(GameObject gameObject)
    {
        Transform transform = Instantiate(gameObject).GetComponent<Transform>();
        transform.position = _player.position;
    }

}
