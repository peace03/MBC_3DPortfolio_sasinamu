using UnityEngine;
using UnityEngine.Pool;

public class ItemWorldObject : MonoBehaviour
{
    // 월드에 떨어진 아이템을 위한 변수들
    //[Header("아이템 정보")]
    //[SerializeField] private int itemId;
    //[SerializeField] private int itemCount;

    private IObjectPool<GameObject> poolRef;
}