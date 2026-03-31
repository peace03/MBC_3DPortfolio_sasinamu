using UnityEngine;

public class Bridge : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // 상자 이벤트 발생(상자 아이템들 정보 전달)
        //Subject<IBoxHandler>.Publish(h => h.OnBox(_boxModel));
        Debug.Log("다리 상호작용");
    }
}
