using UnityEngine;
using UnityEngine.Pool;

public class ItemWorldObject : MonoBehaviour
{
    // 월드에 떨어진 아이템을 위한 변수들
    [Header("아이템 정보")]
    [SerializeField] private Item dropItem;
    public Item DropItem => dropItem;

    private IObjectPool<ItemWorldObject> poolRef;

    public void Init(Item item, IObjectPool<ItemWorldObject> pool)
    {
        dropItem = item;
        poolRef = pool;
    }

    public void ReturnToPool()
    {
        dropItem = null;
        poolRef.Release(this);
    }
}