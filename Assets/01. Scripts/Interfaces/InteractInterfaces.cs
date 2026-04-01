using System.Collections.Generic;

// 다리 등 수리 가능한 오브젝트가 Presenter에게 재료 검증/소모를 요청할 때 사용
public interface IRepairableHandler
{
    // 요구 재료가 충분한지 확인하고, 텍스트(예: "나무(2/3)")를 반환
    public bool CheckCanRepair(List<NeedItem> needItems, out string reqTextStr);

    // 실제로 재료를 소모 (성공 시 true 반환)
    public bool ConsumeRepairItems(List<NeedItem> needItems);
}

// 요구 재료 구조체 (기존 제작대에서 사용하던 것과 동일하거나 유사한 구조체 재사용 권장)
[System.Serializable]
public struct NeedItem
{
    public int id;
    public int count;
}
