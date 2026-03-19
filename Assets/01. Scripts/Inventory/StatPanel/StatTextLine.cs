using TMPro;
using UnityEngine;

//상태창의 아이템 능력치 출력 라인
public class StatTextLine : MonoBehaviour
{
    private TextMeshProUGUI _statName;
    private TextMeshProUGUI _statValue;

    private void Awake()
    {
        _statName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _statValue = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void SetStatText(string statName, string statValue)
    {
        _statName.text = statName;
        _statValue.text = statValue;
    }
}
