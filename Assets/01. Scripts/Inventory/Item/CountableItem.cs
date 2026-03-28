using UnityEngine;

[System.Serializable]
public class CountableItem : Item
{
    private int _curAmount; //현재 아이템 개수
    public CountableItem(CountableData data, int curAmount = 1) : base(data)
    {
        _curAmount = curAmount;
    }

    public int CurAmount => _curAmount;
    public int MaxAmount => (_data as CountableData).MaxAmount;

    //현재 적재량 업데이트 및 남은 수량 반환
    public void SetCurAmount(int amount)
    {
        _curAmount = amount;
    }
}
