using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    [Header("정보")]
    // GameObject 대신 아이템 객체(아마... Item?)로 바꾸시기!
    // 바꾸시면서 Interfaces/SystemInterfaces.cs에 IBoxHandler고치기
    [SerializeField] private List<GameObject> boxItems = new();     // 상자 아이템들

    // 상호작용 함수
    public void Interact()
    {
        // 상자 이벤트 발생(상자 아이템들 정보 전달)
        Subject<IBoxHandler>.Publish(h => h.OnBox(boxItems));
    }
}