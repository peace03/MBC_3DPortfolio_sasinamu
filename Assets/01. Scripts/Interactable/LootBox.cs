using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, IInteractable
{
    [Header("정보")]
    [SerializeField] private string boxName;        // 상자 이름
    [SerializeField] private string[] itemsArray;   // 아이템들 확인용 배열(나중에 ItemData 혹은 ItemInstance로 바꾸기)

    private List<string> itemsList;

    private void Start()
    {
        // 초기화(나중에 고치기)
        itemsList = new List<string>();
        itemsList.Add("총");
        itemsList.Add("탄환");
        itemsList.Add("나무");
        // 배열에 아이템 리스트 반영
        itemsArray = itemsList.ToArray();
    }

    // 상호작용 함수
    public void Interact()
    {
        Debug.Log("상호작용 함");
        // UI 활성화 이벤트 발생
        EventBus<UIStateEvent>.Publish(new UIStateEvent(true));
        // UI에 전리품 상자 정보 넘겨주기
        EventBus<LootBoxEvent>.Publish(new LootBoxEvent(boxName, itemsList));
    }
}