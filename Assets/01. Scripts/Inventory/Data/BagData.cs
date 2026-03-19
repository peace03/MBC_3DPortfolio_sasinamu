using UnityEngine;

[CreateAssetMenu(fileName = "BagData", menuName = "Data/BagData")]
public class BagData : ItemData
{
    [Header("가방 정보")]
    public int addSlotNum; //추가 슬롯 개수
}
