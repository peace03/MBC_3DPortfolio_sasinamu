using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameLevelButton : UIHoverSelectionHandler, ISelectHandler
{
    [Header("정보")]
    [SerializeField] private Text descriptionText;      // 설명 텍스트
    [SerializeField] private string description;        // 설명

    // 선택했을 때 실행되는 함수
    public void OnSelect(BaseEventData eventData) => descriptionText.text = description;
}