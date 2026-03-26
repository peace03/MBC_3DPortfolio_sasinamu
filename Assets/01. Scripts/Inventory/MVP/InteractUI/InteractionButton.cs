using TMPro;
using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    private Transform _dropButton;
    public void Init()
    {
        _dropButton = gameObject.transform.GetChild(0);
    }

    //슬롯에서 마우스 우클릭 했을 때
    public void SlotClickRight(Transform transform)
    {
        _dropButton.gameObject.SetActive(true);
        MovePosition(transform);
    }

    public void MovePosition(Transform transform)
    {
        RectTransform rect = GetComponent<RectTransform>();
        RectTransform rect2 = transform.GetComponent<RectTransform>();
        rect.position = 
            new Vector2(rect2.position.x + 50, rect2.position.y);
    }

    public void DropButtonDown()
    {
        Subject<IDropButtonHandler>.Publish(h => h.OnDropButtenDown());
    }
}
