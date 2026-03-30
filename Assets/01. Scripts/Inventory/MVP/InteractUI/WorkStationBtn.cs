using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkStationBtn : MonoBehaviour
{
    [System.Serializable]
    public struct NeedItem
    {
        public int id;
        public int count;
    }

    public int ResultItemID; // 이 버튼이 만들어낼 결과물 아이템 ID
    [SerializeField] private Image _btnImage;           //색상 변경용
    [SerializeField] private Button _craftButton; // 실제 클릭할 UI 버튼
    [SerializeField] private TextMeshProUGUI _reqText; // "아이템명 (보유/필요)"를 띄울 텍스트
    [SerializeField] private List<NeedItem> _needItem;    //생성하는데 필요한 아이템(인스펙터에서 설정)

    public List<NeedItem> NeedItemList => _needItem;

    private void Start()
    {
        // 버튼이 눌리면 전역 이벤트 버스에 나 자신(this)을 실어서 방송(Publish)합니다.
        // Presenter가 이 방송을 듣고 OnCraftButtonClicked(this)를 실행하게 됩니다.
        _craftButton.onClick.AddListener(() =>
        {
            Subject<ICraftItemHandler>.Publish(h => h.OnCraftButtonClicked(this));
        });
    }

    // Presenter가 계산한 데이터를 받아 UI를 갱신합니다.
    public void UpdateUI(bool canCraft, string formattedText)
    {
        _craftButton.interactable = canCraft; // 조건 달성 시 활성화
        _reqText.text = formattedText;

        // 시각적 피드백 (녹색/적색)
        GetComponent<Image>().color = canCraft ? new Color(0.5f, 1f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
    }

    public void SetColor(Color color)
    {
        _btnImage.color = color;
    }
}
