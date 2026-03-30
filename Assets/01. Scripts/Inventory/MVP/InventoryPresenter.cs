using System.Collections.Generic;
using UnityEngine;

public enum SlotType
{
    Equip,      //장비
    Bag,        //가방
    Box,        //상자
    Storage,    //창고
    Quick,       //퀵슬롯
    None
}

public class InventoryPresenter : ISlotExchangeHandler, ISlotChanged, 
    ISlotClickHandler, IPlayerInteractHandler, IButtonHandler, ICusorPointerHandler,
    IBoxHandler, IFireBullet, IWorkStation, ICraftItemHandler
{
    private FacadeView _view;
    private InventoryModel _bagModel;
    private InventoryModel _equipModel;
    private InventoryModel _storageModel;
    private InventoryModel _quickModel;
    private InventoryModel _boxModel;
    private ItemManager _itemManager;

    //슬롯 좌클릭시 저장
    private SlotType _slotTypeLeft = SlotType.None;
    private int _slotIndexLeft;
    //슬롯 우클릭시 저장
    private SlotType _slotTypeRight = SlotType.None;
    private int _slotIndexRight;
    //현재 활성화된 제작대의 버튼 리스트를 기억해둘 캐싱 변수
    private List<WorkStationBtn> _currentWorkStationBtns;


    public InventoryPresenter(FacadeView view,
        InventoryModel equipModel, InventoryModel bagModel, InventoryModel storageModel,InventoryModel quickModel,
        ItemManager manager)
    {
        _view = view;
        _equipModel = equipModel;
        _bagModel = bagModel;
        _storageModel = storageModel;
        _quickModel = quickModel;
        _itemManager = manager;
    }

    //초기화
    public void InitializePresenter()
    {
        //_bagInvenView.SetEvent(OnSlotDragDrop, UpdateStatPanel); //구독시키기
        //Debug.Log("실행1");
        Test();
        //Debug.Log("실행2");
        UpdateAllSlot(SlotType.Bag);
        UpdateAllSlot(SlotType.Box);
        //Debug.Log("실행3");
    }

    public void UpdateAllSlot(SlotType slotType)
    {
        InventoryModel model = GetModel(slotType);
        for (int i = 0; i < model.Capacity; i++)
        {
            OnUpdateSingleSlot(slotType, i);
        }
    }

    void Test()
    {
        CreateItem(1);
        CreateItem(13);
        CreateItem(20);
        CreateItem(16);
        CreateItem(8);
        CreateItem(1);
        CreateItem(18);
        CreateItem(9);
    }

    //ItemData의 id로 객체 생성하도록 M에 요청
    private bool CreateItem(int id)
    {
        //Debug.Log("P - CreateItem 호출 완료");
        //_model.CheckFullInventory();
        Item item = _itemManager.CreateItemInstance(id); //인스턴스 내부에서 호출 피드백
        //Debug.Log($"P - 생성된 인스턴스 Type: {item.GetType()}");
        //Debug.Log($"item: {item}");
        return _bagModel.AddItem(item);
    }

    //아이템 스탯창 업데이트
    public void OnCusorSlotIn(SlotType slotType, int index)
    {
        InventoryModel model = GetModel(slotType);
        _view.SetItemStatPanel(model.GetItem(index));
    }
    //아이템 스탯창 비활성화
    public void OnCusorSlotExit()
    {
        _view.StatPanelInActive();
    }


    #region 아이템 교체
    //아이템 교체
    public void OnExchangeSlot(SlotType fromSlotType, int fromIndex, SlotType toSlotType, int toIndex)
    {
        Item fromItem;
        Item toItem;
        //아이템 교체
        fromItem = GetModelItem(fromSlotType, fromIndex);
        toItem = GetModelItem(toSlotType, toIndex);
        if (fromSlotType == SlotType.Equip || toSlotType == SlotType.Equip)
            if (!CanExchange(fromSlotType, fromIndex, fromItem, toSlotType, toIndex, toItem)) return; //교환 불가능하면 종료
        Debug.Log("슬롯 교환 호출!!");
        //아이템 적재
        if (fromItem is CountableItem fcount && toItem is CountableItem tcount)
            if (fcount._data.ID == tcount._data.ID)
                if (tcount.CurAmount <= tcount.MaxAmount)
                {
                    int rest = GetModel(toSlotType).PutCountToItem(toSlotType, toIndex, fcount.CurAmount);
                    GetModel(fromSlotType).PutCountFromItem(fromSlotType, fromIndex, rest);
                    return;
                }
        PutModelItem(fromSlotType, fromIndex, toItem);
        PutModelItem(toSlotType, toIndex, fromItem);
        //UI 업데이트는 자동으로 됨
    }
    //모델의 해당 인덱스 아이템 가져오기
    public Item GetModelItem(SlotType slotType, int index)
    {
        InventoryModel model = GetModel(slotType);
        return model.GetItem(index);
    }
    //슬롯 타입에 맞는 모델 얻기
    public InventoryModel GetModel(SlotType slotType)
    {
        switch (slotType)
        {
            case SlotType.Equip:
                return _equipModel;
            case SlotType.Bag:
                return _bagModel;
            case SlotType.Box:
                return _boxModel;
            case SlotType.Storage:
                return _storageModel;
            case SlotType.Quick:
                return _quickModel;
            default:
                break;
        }
        return null;
    }
    //교환 가능한지 검사 및 교체될 아이템 이벤트로 반환
    public bool CanExchange(SlotType fromSlotType, int fromIndex, Item fromItem,
        SlotType toSlotType, int toIndex, Item toItem) //보완해야함
    {
        bool equal;
        if (fromSlotType == SlotType.Equip && toSlotType == SlotType.Equip)
        {//장비 -> 장비
            //무기끼리만 교환 가능
            if (fromIndex < 2 && toIndex < 2)
            {
                //장비 교체 알림
                Subject<IEquipWear>.Publish(h => h.OnGunSwap(fromIndex, fromItem, toIndex, toItem));
                return true;
            }
        }
        else if (fromSlotType == SlotType.Equip)
        {//장비 -> 가방
            if (toItem == null) equal = true;
            else equal = EqualEquipType(fromIndex, fromItem, toItem);
            //장비 교체 알림
            if (equal == true) Subject<IEquipWear>.Publish(h => h.OnEquipWear(toIndex, toItem));
            return equal;
        }
        else //toSlotType == SlotType.Equip
        {//가방 -> 장비
            equal = EqualEquipType(toIndex, toItem, fromItem);
            //장비 교체 알림
            if (equal == true) Subject<IEquipWear>.Publish(h => h.OnEquipWear(fromIndex, fromItem));
            return equal;
        }
        return false;
    }
    public bool EqualEquipType(int equipIndex, Item equipItem, Item other)
    {
        switch (equipIndex)
        {
            case 0:
            case 1:
                if (other is GunItem) return true;
                break;
            case 2:
                if (other is BagItem) return true;
                break;
            case 3:
                if (other is ConsumableItem consume1 && consume1.Type == ConsumableType.Helmat) return true;
                break;
            case 4:
                if (other is ConsumableItem consume2 && consume2.Type == ConsumableType.Vest) return true;
                break;
            default:
                break;
        }
        return false;
    }

    //모델의 해당 인덱스 아이템 넣어주기
    public void PutModelItem(SlotType slotType, int index, Item item)
    {
        InventoryModel model = GetModel(slotType);
        model.PutItem(slotType, index, item);
    }
    public void OnUpdateSingleSlot(SlotType slotType, int index)
    {
        InventoryModel model = GetModel(slotType);
        Item item = model.GetItem(index);
        switch (slotType)
        {
            case SlotType.Equip:
                _view.UpdateSingleSlot_Equip(index, item);
                break;
            case SlotType.Bag:
                _view.UpdateSingleSlot_Bag(index, item);
                break;
            case SlotType.Box:
                _view.UpdateSingleSlot_Box(index, item);
                break;
            case SlotType.Storage:
                _view.UpdateSingleSlot_Storage(index, item);
                break;
            case SlotType.Quick:
                _view.UpdateSingleSlot_Quick(index, item);
                break;
            default:
                break;
        }
    }


    #endregion

    //슬롯 클릭되었을 때 실행
    public void OnSlotLeftClick(SlotType slotType, int index)
    {
        _slotTypeLeft = slotType;
        _slotIndexLeft = index;
    }
    public void OnSlotRightClick(SlotType slotType, int index)
    {
        _slotTypeRight = slotType;
        _slotIndexRight = index;
        Debug.Log($"{_slotTypeRight}, {_slotIndexRight}");

        //Use 버튼 표시 가능 판단
        Item item = GetModel(slotType).GetItem(index);
        if (item is CureKitItem || item is FoodItem)
            Subject<ISlotClickRightHandler>.Publish(h => h.OnUseBtnSetActive(true));
        else Subject<ISlotClickRightHandler>.Publish(h => h.OnUseBtnSetActive(false));
        //Debug.Log("우클릭");
    }

    //좌클릭 후 E키로 상호작용 시 아이템 사용
    public void OnInteract()
    {
        //Debug.Log("사용 버튼 눌림");
        if (_slotTypeLeft == SlotType.None) return;
        //아이템 사용
        InventoryModel model = GetModel(_slotTypeLeft);
        model.UseItem(_slotTypeLeft, _slotIndexLeft);
        //Debug.Log("사용 버튼 눌림");
    }

    //아이템 사용하기 버튼 눌렀을 때 실행
    public void OnUseButtonDown()
    {
        if (_slotTypeRight == SlotType.None) return;
        //아이템 사용
        InventoryModel model = GetModel(_slotTypeRight);
        model.UseItem(_slotTypeRight, _slotIndexRight);
    }
    //총 발사시 호출
    public void OnUseGunItem(SlotType slotType, int index)
    {
        InventoryModel model = GetModel(slotType);
        model.UseItem(slotType, index);
    }
    //아이템 버리기 버튼 눌렀을 때 실행
    public void OnDropButtenDown()
    {
        if (_slotTypeRight == SlotType.None) return;
        InventoryModel model = GetModel(_slotTypeRight);
        //오브젝트 생성
        _itemManager.CreateItemObjInWorld(model.GetItemObject(_slotIndexRight));
        //Model에서 삭제
        model.PutItem(_slotTypeRight, _slotIndexRight, null);
    }

    //월드 생성될 때 박스 아이템들 초기화
    public void InitBoxModel(InventoryModel boxModel, int boxCapacity)
    {
        //박스 모델 초기화
        boxModel.Init(boxCapacity);
        //모델에 아이템 랜덤 생성
        int[] IDs = new int[] { 1,8,9,13,16,18,20 };
        for (int i = 0; i < Random.Range(1, boxCapacity); i++)
        {
            Item item = _itemManager.CreateItemInstance(IDs[Random.Range(0, IDs.Length)]);
            //Debug.Log($"생성된 아이템:{item._data.Name}");
            boxModel.AddItem(item);
        }
        _boxModel = boxModel;
    }

    //상자 상호작용 시 호출
    public void OnBox(InventoryModel boxModel)
    {
        Debug.Log("호출");
        _boxModel = boxModel;
        UpdateAllSlot(SlotType.Box);
    }

    // [UI 열림 이벤트 구독 응답] - BootStrapper에서 Attach 해주어야 함!
    public void OnActiveWorkSationUI(List<WorkStationBtn> btns)
    {
        // 1. 넘어온 버튼 리스트를 메모리에 캐싱합니다.
        _currentWorkStationBtns = btns;

        // 2. UI를 최초로 그려줍니다.
        RefreshWorkStationUI(_currentWorkStationBtns);
    }

    // 1. 제작대 UI가 열리거나 아이템이 갱신될 때 버튼 상태를 검증하는 로직
    public void RefreshWorkStationUI(List<WorkStationBtn> btns)
    {
        foreach (var btn in btns)
        {
            bool canCraft = true;
            string reqTextStr = "";

            foreach (var needItem in btn.NeedItemList)
            {
                // 3개의 모델을 순회하며 총합 계산 (Aggregation)
                int totalOwned = _bagModel.GetTotalItemCount(needItem.id)
                               + _storageModel.GetTotalItemCount(needItem.id)
                               + _quickModel.GetTotalItemCount(needItem.id);

                if (totalOwned < needItem.count) canCraft = false;

                // 텍스트 포맷팅 생성 예시: "나무(2/5)\n철광석(10/2)"
                string itemName = _itemManager.GetItemNameByID(needItem.id); // ItemManager에 이름 가져오는 기능이 필요함
                reqTextStr += $"{itemName} ({totalOwned}/{needItem.count})\n";
            }

            // View에게 계산된 결과 전달
            btn.UpdateUI(canCraft, reqTextStr);
        }
    }

    // 2. 제작 버튼 클릭 시 실행될 핵심 트랜잭션
    public void OnCraftButtonClicked(WorkStationBtn btn)
    {
        // [Phase 1: 재료 차감 (Sequential Consumption)]
        foreach (var needItem in btn.NeedItemList)
        {
            int remaining = needItem.count;

            // 폭포수(Waterfall) 방식으로 남은 요구량을 다음 모델로 넘기며 차감
            remaining = _bagModel.ConsumeItem(SlotType.Bag, needItem.id, remaining);
            if (remaining > 0) remaining = _storageModel.ConsumeItem(SlotType.Storage, needItem.id, remaining);
            if (remaining > 0) remaining = _quickModel.ConsumeItem(SlotType.Quick, needItem.id, remaining);
        }

        // [Phase 2: 결과물 생성 및 적재 (Priority Insertion)]
        Item craftedItem = _itemManager.CreateItemInstance(btn.ResultItemID);

        // 우선순위 1: 가방
        if (!_bagModel.AddItem(craftedItem))
        {
            // 우선순위 2: 창고
            if (!_storageModel.AddItem(craftedItem))
            {
                // 예외 처리: 모두 꽉 찼다면 월드에 드랍 (데이터 증발 방지)
                _itemManager.CreateItemObjInWorld(craftedItem._data.ObjectPrefab);
                Debug.LogWarning("가방과 창고가 모두 가득 차서 아이템이 바닥에 떨어졌습니다.");
            }
            else UpdateAllSlot(SlotType.Storage); // 창고 뷰 갱신
        }
        else UpdateAllSlot(SlotType.Bag); // 가방 뷰 갱신

        // [Phase 3: 상태 재동기화]
        // 재료가 소모되었으므로 현재 켜져있는 제작대의 버튼 UI(보유량 텍스트, 활성화 여부)를 다시 갱신해야 합니다.
        // (예: Subject<IWorkStationHandler>.Publish(...) 등을 통해 RefreshWorkStationUI를 다시 호출)
        if (_currentWorkStationBtns != null)
        {
            RefreshWorkStationUI(_currentWorkStationBtns);
        }
    }
}





