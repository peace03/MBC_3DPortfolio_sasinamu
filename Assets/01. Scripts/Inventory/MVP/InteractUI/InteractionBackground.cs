using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionBackground : MonoBehaviour, IPointerClickHandler, IUIStateHandler
{
    private bool openedUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Middle && openedUI)
        {
            Subject<ISlotClickRightHandler>.Publish(h => h.OnSlotClickRight(gameObject.transform));
        }
    }

    public void OnUIState(bool state) => openedUI = state;
}
