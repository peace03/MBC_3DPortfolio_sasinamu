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
    IBoxHandler, IFireBullet, IWorkStation, ICraftItemHandler,
    IRepairableHandler, IEquipmentDestroyHandler, IBoxModelHandler
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
    private List<CreateItemBtn> _currentWorkStationBtns;


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
        // 1. 아이템 무한 증식 원천 차단 (상태 플래그 검증)
        // GameDataManager를 통해 게임 전체 수명주기(Application Domain) 중 딱 1번만 실행되도록 보장합니다.
        if (GameDataManager.Instance.HasSpawnedInitialItems == false)
        {
            Test(); // 초기 아이템 지급
            GameDataManager.Instance.HasSpawnedInitialItems = true; // 지급 완료 처리
        }

        // [추가된 부분] 씬이 바뀔 때, 이전 씬에서 보존된 Model의 용량대로 View의 UI 칸수를 맞춰줍니다.
        //_view.UpdateBagCapacityUI(_bagModel.Capacity);
        // 목표 용량을 동적으로 계산하고, UI 렌더링과 모델 상태를 완벽하게 자가 치유시킵니다.
        UpdateBagCapacityState();

        //_bagInvenView.SetEvent(OnSlotDragDrop, UpdateStatPanel); //구독시키기
        UpdateAllSlot(SlotType.Equip);
        UpdateAllSlot(SlotType.Bag);
        UpdateAllSlot(SlotType.Quick);
        UpdateAllSlot(SlotType.Storage);
        Debug.Log("프레젠터 초기화 완료");
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
        int[] ids = new int[] { 1,2,3,4,5,6,7,13,14, 15};
        CreateItem(10);
        CreateItem(11);
        CreateItem(1);
        CreateItem(13);
        CreateItem(19);
        CreateItem(16);
        CreateItem(8);
        CreateItem(1);
        CreateItem(18);
        CreateItem(9);
        //int[] ids = new int[] { 6, 5, 7, 16, 2, 13 };

        for (int i = 0; i < ids.Length; i++)
        {
            CreateItem(ids[i]);
        }
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
        //Debug.Log("슬롯 교환 호출!!");
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

        // [추가된 부분] 만약 교체된 슬롯 중 하나라도 '장비창의 가방(2번 인덱스)' 이라면 용량 제어 메서드 호출
        if ((fromSlotType == SlotType.Equip && fromIndex == 2) ||
            (toSlotType == SlotType.Equip && toIndex == 2))
        {
            UpdateBagCapacityState();
        }

        Subject<IEquipmentSlotHandler>.Publish(h => h.OnEquipmentSlot(_equipModel));
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
            case 0: //총
            case 1: //총
                if (other is GunItem) return true;
                break;
            case 2: //가방
                if (other is BagItem) return true;
                break;
            case 3: //조끼
                if (other is VestItem consume2 && consume2.Type == ConsumableType.Vest) return true;
                break;
            case 4:
                //if (other is ConsumableItem consume1 && consume1.Type == ConsumableType.Helmat) return true;
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
        Debug.Log(GetModel(slotType));
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
                Subject<IQuickSlotHandler>.Publish(h => h.OnQuickSlot(model));
                break;
            default:
                break;
        }

        Subject<IInventoryWeightHandler>.Publish(h => h.OnInventoryWeight(_equipModel.GetTotalWeight()
                                                + _bagModel.GetTotalWeight() + _quickModel.GetTotalWeight()));
    }


    #endregion

    //슬롯 클릭되었을 때 호출
    public void OnSlotLeftClick(SlotType slotType, int index)
    {
        //클릭된 슬롯 위치 저장
        _slotTypeLeft = slotType;
        _slotIndexLeft = index;
    }
    public void OnSlotRightClick(SlotType slotType, int index)
    {
        //클릭된 슬롯 위치 저장
        _slotTypeRight = slotType;
        _slotIndexRight = index;

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
        model.FireGun(slotType, index);
    }
    //아이템 버리기 버튼 눌렀을 때 실행
    public void OnDropButtenDown()
    {
        if (_slotTypeRight == SlotType.None) return;
        InventoryModel model = GetModel(_slotTypeRight);
        //오브젝트 생성
        _itemManager.CreateItemObjInWorld(model.GetItemImage(_slotIndexRight));
        //Model에서 삭제
        model.PutItem(_slotTypeRight, _slotIndexRight, null);
    }

    //월드 생성될 때 박스 아이템들 초기화
    public void InitBoxModel(InventoryModel boxModel, int boxCapacity)
    {
        //박스 모델 초기화
        boxModel.Init(boxCapacity);
        //모델에 아이템 랜덤 생성
        int[] exclusionIDs = new int[] { 12, 9, 16, 10, 11, 8 }; //제외아이템
        for (int i = 0; i < Random.Range(1, boxCapacity); i++)
        {
            int id = Random.Range(1, 24);
            foreach (int j in exclusionIDs) //제외 아이템이면 다시생성
            {
                if (id == j)
                {
                    i--;
                    id = 999;
                }
                break;
            }
            Item item = _itemManager.CreateItemInstance(id);
            //Debug.Log($"생성된 아이템:{item._data.Name}");
            boxModel.AddItem(item);
        }
        _boxModel = boxModel;
    }

    //상자 상호작용 시 호출
    public void OnBox(InventoryModel boxModel)
    {
        Debug.Log("P호출");
        _boxModel = boxModel;
        UpdateAllSlot(SlotType.Box);
    }

    // [UI 열림 이벤트 구독 응답] - BootStrapper에서 Attach 해주어야 함!
    public void OnActiveWorkSationUI(List<CreateItemBtn> btns)
    {
        //Debug.Log("P - 제작대 UI 활성화 무전 수신!");
        // 1. 넘어온 버튼 리스트를 메모리에 캐싱합니다.
        _currentWorkStationBtns = btns;

        // 2. UI를 최초로 그려줍니다.
        RefreshWorkStationUI(_currentWorkStationBtns);
    }

    // 1. 제작대 UI가 열리거나 아이템이 갱신될 때 버튼 상태를 검증하는 로직
    public void RefreshWorkStationUI(List<CreateItemBtn> btns)
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
            //Debug.Log($"만들 수 있?:{canCraft} 텍스트: {reqTextStr}");
            // View에게 계산된 결과 전달
            btn.UpdateUI(canCraft, reqTextStr);
        }
    }

    // 2. 제작 버튼 클릭 시 실행될 핵심 트랜잭션
    public void OnCraftButtonClicked(CreateItemBtn btn)
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

    // [IRepairableHandler 구현] 1. 재료가 충분한지 검증하고 UI에 띄울 텍스트를 만들어줍니다.
    public bool CheckCanRepair(List<NeedItem> needItems, out string reqTextStr)
    {
        bool canRepair = true;
        reqTextStr = "";

        foreach (var needItem in needItems)
        {
            // 가방과 퀵슬롯만 검사 (기획에 따라 _storageModel.GetTotalItemCount 추가 가능)
            int totalOwned = _bagModel.GetTotalItemCount(needItem.id)
                           + _quickModel.GetTotalItemCount(needItem.id);

            if (totalOwned < needItem.count) canRepair = false;

            string itemName = _itemManager.GetItemNameByID(needItem.id);
            reqTextStr += $"{itemName} ({totalOwned}/{needItem.count})  ";
        }

        return canRepair;
    }

    // [IRepairableHandler 구현] 2. 수리를 시작하면 실제로 인벤토리에서 재료를 깎아냅니다.
    public bool ConsumeRepairItems(List<NeedItem> needItems)
    {
        // 검증 로직 한 번 더 수행 (안전망)
        if (!CheckCanRepair(needItems, out _)) return false;

        foreach (var needItem in needItems)
        {
            int remaining = needItem.count;
            // 가방에서 먼저 빼고, 부족하면 퀵슬롯에서 마저 뺌
            remaining = _bagModel.ConsumeItem(SlotType.Bag, needItem.id, remaining);
            if (remaining > 0) remaining = _quickModel.ConsumeItem(SlotType.Quick, needItem.id, remaining);
        }

        // 뷰(UI) 갱신
        UpdateAllSlot(SlotType.Bag);
        UpdateAllSlot(SlotType.Quick);

        return true;
    }

    // 장비창 2번 슬롯(가방)의 상태를 확인하고 용량과 데이터를 갱신합니다.
    public void UpdateBagCapacityState()
    {
        int baseCapacity = 10;
        int newCapacity = baseCapacity;

        Item bagSlotItem = _equipModel.GetItem(2);
        if (bagSlotItem is BagItem bag)
        {
            newCapacity += bag.AddSlotNum;
        }

        if (_bagModel.Capacity == newCapacity) return;

        // 1. Model에게 용량 변경 및 초과 아이템 추출 지시
        List<Item> overflowItems = _bagModel.ChangeCapacity(newCapacity);

        // 2. 오버플로우가 발생했다면 '단일 상자'를 스폰하여 데이터 캐리어로 활용
        if (overflowItems != null && overflowItems.Count > 0)
        {
            // 💡 상자가 떨어질 위치 계산 (현재 View의 위치 혹은 의존성 주입된 플레이어의 위치 기준)
            Vector3 dropPos = _itemManager.Player;

            // ItemManager에게 상자 스폰 지시
            GameObject boxObj = _itemManager.CreateDroppedBoxInWorld(dropPos);

            // 스폰된 상자의 컴포넌트를 가져와 추출된 짐(메모리)을 강제 주입
            Box droppedBox = boxObj.GetComponent<Box>();
            if (droppedBox != null)
            {
                droppedBox.InitOverflowItems(overflowItems);
            }
        }

        _view.UpdateBagCapacityUI(newCapacity);
        UpdateAllSlot(SlotType.Bag);
    }

    public void OnEquipmentDestroyed(int slotIndex)
    {
        // 1. 모델(Model)에서 해당 인덱스의 아이템을 추출
        Item destroyedItem = _equipModel.GetItem(slotIndex);

        if (destroyedItem != null)
        {
            // 2. 단일 아이템 소모 로직을 통해 배열의 해당 인덱스를 null로 메모리 해제
            _equipModel.ConsumeItem(SlotType.Equip, destroyedItem._data.ID, 1);
            // (만약 인덱스로 직접 지우는 RemoveItem(slotIndex) 메서드가 있다면 그것을 사용하는 것이 더 안전합니다)

            Debug.Log($"[시스템] 장비창 {slotIndex}번 슬롯의 장비가 파괴되었습니다.");
        }

        // 3. 화면(View) 강제 동기화
        UpdateAllSlot(SlotType.Equip);

        // 4. 가방(2번)이 파괴되었을 경우를 대비한 엣지 케이스 방어
        if (slotIndex == 2) UpdateBagCapacityState();

        // 5. 장비가 벗겨졌으므로 PlayerStat에 스탯을 다시 계산하라고 브로드캐스팅
        Subject<IEquipmentSlotHandler>.Publish(h => h.OnEquipmentSlot(_equipModel));
    }

    public void OnInitBoxModel(InventoryModel model)
    {
        _boxModel = model;
    }
}