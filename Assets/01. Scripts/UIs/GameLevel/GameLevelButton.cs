using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameLevelButton : UIHoverSelectionHandler, ISelectHandler
{
    [Header("정보")]
    [SerializeField] private Text descriptionText;      // 설명 텍스트
    [SerializeField] private string description;        // 설명

    // 마우스가 버튼 위에 올라오면 실행되는 함수
    public override void OnPointerEnter(PointerEventData eventData)
    {
        // 현재 버튼을 선택한 걸로 바꾸기
        base.OnPointerEnter(eventData);
    }

    // 선택했을 때 실행되는 함수
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("들어옴 2");
        // 설명 텍스트 내용 변경
        descriptionText.text = description;
    }
}