using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CreateItemView : MonoBehaviour
{
    private List<CreateItemBtn> _buttons;
    [SerializeField] private int btnCount;  //버튼 개수

    private Color Red = new Color(1f, 0f, 0f, 0.3f);
    private Color Green = new Color(0.5f, 1f, 0.3f);

    public void Init()
    {
        _buttons = new List<CreateItemBtn>();
        for (int i = 0; i < btnCount; i++)
        {
            _buttons.Add(transform.GetChild(1).GetChild(i).GetComponent<CreateItemBtn>());
            _buttons[i].SetColor(Red);
            _buttons[i].GetComponent<Button>().interactable = false;
        }
        Debug.Log("작업대 View 초기화 호출 완료");
        Subject<IWorkStation>.Publish(h => h.OnActiveWorkSationUI(_buttons));
    }

    private void OnEnable()
    {
        Subject<IWorkStation>.Publish(h => h.OnActiveWorkSationUI(_buttons));
    }
}
