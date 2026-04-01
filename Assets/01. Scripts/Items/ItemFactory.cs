using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ItemFactory : MonoBehaviour, IItemDataHandler
{
    [Header("최대 아이템 생성 개수")]
    [SerializeField] private int maxItemCount;

    private Dictionary<int, IObjectPool<GameObject>> itemDictionary;

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
        foreach (var data in datas)
        {
            if (itemDictionary.ContainsKey(data.ID))
            {
                Debug.LogError($"[Error] {data.ID}가 있음");
                continue;
            }

            itemDictionary.Add(data.ID, CreatePool(data.ObjectPrefab));
        }
    }

    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(() => CreateItem(prefab), GetItem, ReturnItem, DestroyItem, maxSize: maxItemCount);
    }

    private GameObject CreateItem(GameObject prefab)
    {
        return Instantiate(prefab);
    }

    private void GetItem(GameObject item) => item.SetActive(true);

    private void ReturnItem(GameObject item) => item.SetActive(false);

    private void DestroyItem(GameObject item) => Destroy(item);
}