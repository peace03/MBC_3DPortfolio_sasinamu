using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class WorkStationView : MonoBehaviour
{
    private List<WorkStationBtn> _buttons;
    [SerializeField] private int btnCount;  //버튼 개수

    private Color Red = new Color(1f, 0f, 0f, 0.3f);
    private Color Green = new Color(0.5f, 1f, 0.3f);

    public void Init()
    {
        _buttons = new List<WorkStationBtn>();
        for (int i = 0; i < btnCount; i++)
        {
            _buttons.Add(transform.GetChild(1).GetChild(i).GetComponent<WorkStationBtn>());
            _buttons[i].SetColor(Red);
            _buttons[i].GetComponent<Button>().interactable = false;
        }
    }

    private void OnEnable()
    {
        Subject<IWorkStation>.Publish(h => h.OnActiveWorkSationUI(_buttons));
    }
}
