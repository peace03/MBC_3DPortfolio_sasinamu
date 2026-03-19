using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작");
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 중");
        transform.position = eventData.position;
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        Debug.Log("무언가 떨어짐!");
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 종료");
    }

}
