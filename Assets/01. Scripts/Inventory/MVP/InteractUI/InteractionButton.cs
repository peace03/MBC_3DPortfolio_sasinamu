using TMPro;
using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    private Transform _dropButton;
    private Transform _useButton;
    public Transform UseButton => _useButton;
    public void Init()
    {
        _useButton = gameObject.transform.GetChild(0);
        _dropButton = gameObject.transform.GetChild(1);
        //_dropButton.gameObject.SetActive(false);
    }

    //슬롯에서 마우스 우클릭 했을 때
    public void SlotClickRight(Transform transform)
    {
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
        gameObject.SetActive(false);
    }
}
