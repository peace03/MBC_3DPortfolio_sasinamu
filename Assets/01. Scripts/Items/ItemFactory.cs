using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ItemFactory : MonoBehaviour, IItemDataHandler
{
    [Header("정보")]
    [SerializeField] private Transform container;

    [Header("최대 아이템 생성 개수")]
    [SerializeField] private int maxItemCount;

    private Dictionary<int, IObjectPool<ItemWorldObject>> itemDictionary;

    private void OnEnable()
    {
        Subject<IItemDataHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IItemDataHandler>.Detach(this);
    }

    public void OnItemData(List<ItemData> datas)
    {
        itemDictionary = new();

        foreach (var data in datas)
        {
            if (itemDictionary.ContainsKey(data.ID))
            {
                Debug.LogError($"[Error] {data.ID}가 있음");
                continue;
            }

            itemDictionary.Add(data.ID, CreatePool(data));
        }
    }

    public ItemWorldObject GetWorldItem(Item item)
    {
        if (item == null)
            return null;

        if (itemDictionary.TryGetValue(item._data.ID, out var pool))
        {
            var worldItem = pool.Get();
            worldItem.Init(item, pool);
            worldItem.gameObject.transform.localPosition = Vector3.zero;
            worldItem.gameObject.transform.localScale = Vector3.one;
            return worldItem;
        }

        Debug.LogError($"[Error] {item._data.ID}의 아이템({item._data.Name})에 해당하는 오브젝트 풀이 없습니다.");
        return null;
    }

    private IObjectPool<ItemWorldObject> CreatePool(ItemData data)
    {
        return new ObjectPool<ItemWorldObject>(() => CreateItem(data), GetItem, ReturnItem, DestroyItem, maxSize: maxItemCount);
    }

    private ItemWorldObject CreateItem(ItemData data)
    {
        return Instantiate(data.Object2DPrefab).GetComponent<ItemWorldObject>();
    }

    private void GetItem(ItemWorldObject item) => item.gameObject.SetActive(true);

    private void ReturnItem(ItemWorldObject item)
    {
        item.transform.SetParent(container);
        item.gameObject.SetActive(false);
    }

    private void DestroyItem(ItemWorldObject item) => Destroy(item);
}