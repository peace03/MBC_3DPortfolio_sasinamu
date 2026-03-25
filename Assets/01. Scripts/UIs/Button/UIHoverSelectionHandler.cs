using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverSelectionHandler : MonoBehaviour, IPointerEnterHandler
{
    // 마우스 호버 시 실행되는 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 현재 버튼을 선택한 걸로 바꾸기
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}