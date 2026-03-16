//using UnityEngine;

//public class PlayerEquipment : MonoBehaviour
//{
//    // 장착 아이템 오브젝트 착용될 위치? 같은 걸 받아오면 될듯...

//    private void OnEnable()
//    {
//        // 장전 이벤트 구독
//        EventBus<ReloadEvent>.OnEvent += HandleReload;
//    }

//    private void OnDisable()
//    {
//        // 장전 이벤트 구독 해제
//        EventBus<ReloadEvent>.OnEvent -= HandleReload;
//    }

//    // 장전 관리 함수
//    private void HandleReload(ReloadEvent data)
//    {
//        Debug.Log("장전");
//    }
//}