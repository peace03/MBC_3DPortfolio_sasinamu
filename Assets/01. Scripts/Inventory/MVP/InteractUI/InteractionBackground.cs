using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionBackground : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Middle)
        {
            Subject<ISlotClickRightHandler>.Publish(h => h.OnSlotClickRight(gameObject.transform));
        }
    }
}
