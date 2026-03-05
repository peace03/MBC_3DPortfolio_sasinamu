using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, IInteractable
{
    [Header("정보")]
    [SerializeField] private string boxName;        // 상자 이름

    // 상호작용 함수
    public void Interact()
    {
        // UI 활성화 이벤트 발생
        EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
        // UI에 전리품 상자 정보 넘겨주기
        EventBus<LootBoxEvent>.Publish(new LootBoxEvent(boxName));
    }
}