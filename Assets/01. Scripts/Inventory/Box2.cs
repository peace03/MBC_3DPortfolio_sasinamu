using UnityEngine;

public class Box2 : MonoBehaviour
{
    private InventoryModel _boxModel;
    public InventoryModel BoxModel => _boxModel;

    public void SetModel(InventoryModel model)
    {
        _boxModel = model;
    }

    // 상호작용 함수
    public void Interact()
    {
        // 상자 이벤트 발생(상자 아이템들 정보 전달)
        Subject<IBoxHandler>.Publish(h => h.OnBox(_boxModel));
        Debug.Log("상호작용");
    }
}
