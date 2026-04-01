using UnityEngine;

// 제작대 오브젝트에 붙일 스크립트
public class CraftingTableInteract : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // UIManager에게 제작대를 열라고 브로드캐스팅
        Subject<ICraftingHandler>.Publish(h => h.OnCraftingTable());
    }
}