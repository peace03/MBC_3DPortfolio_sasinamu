using System.Collections.Generic;
using UnityEngine;

public class BootStrapper : MonoBehaviour
{
    [SerializeField] private ItemManager _itemManager;

    [Header("User 인벤토리 의존성 주입")] //BootStrapper 역할: P에 MV 연결해줌
    public FacadeView _facadeView; //인스펙터에서 연결
    private InventoryModel _equipInvenModel;    //장비    Model
    private InventoryModel _bagInvenModel;      //가방    Model
    private InventoryModel _storageInvenModel;  //창고    Model
    private InventoryModel _quickInvenModel;    //퀵슬롯  Model
    private InventoryPresenter _InvenPresent;
    [SerializeField] Transform boxesObj;       //박스 오브젝트가 들어잇는 빈 오브젝트
    [SerializeField] int boxNum;                //박스 개수

    [Header("인벤토리 용량")]
    [SerializeField] private int _bagCapacity = 20;     //가방
    [SerializeField] private int _quickCapacity = 6;    //퀵슬롯
    [SerializeField] private int _storageCapacity = 60; //창고
    [SerializeField] private int _boxCapacity = 5;     //상자
                     private int _equipCapacity = 5;    //장비

    private void Awake()
    {
        //ItemManager 초기화
        _itemManager.Init();

        //View별 용량 초기화
        _facadeView.InitViews(_bagCapacity, _quickCapacity, _storageCapacity, _boxCapacity);

        //모델 생성
        _equipInvenModel = new InventoryModel();
        _bagInvenModel = new InventoryModel();
        _storageInvenModel = new InventoryModel();
        _quickInvenModel = new InventoryModel();

        //User Presenter에 M, V 연결
        _InvenPresent = new InventoryPresenter(_facadeView, 
            _equipInvenModel, _bagInvenModel, _storageInvenModel, _quickInvenModel,
            _itemManager);

        //모델별 슬롯 용량 초기화
        _equipInvenModel.Init(_equipCapacity);
        _bagInvenModel.Init(_bagCapacity);
        _storageInvenModel.Init(_storageCapacity);
        _quickInvenModel.Init(_quickCapacity);
        InitBoxes();

        //Test

    }
    private void Start()
    {
        
        _InvenPresent.InitializePresenter();
    }

    //이벤트 구독, 해제
    private void OnEnable()
    {
        Subject<ISlotExchangeHandler>.Attach(_InvenPresent);    //View에서 교환     발생
        Subject<ISlotChanged>.Attach(_InvenPresent);            //Model에서 교환    발생
        Subject<ISlotClickHandler>.Attach(_InvenPresent);       //슬롯 클릭         발생
        Subject<IPlayerInteractHandler>.Attach(_InvenPresent);  //플레이어 상호작용  발생
        Subject<ISlotClickRightHandler>.Attach(_facadeView);    //슬롯 우클릭       발생
        Subject<IButtonHandler>.Attach(_InvenPresent);          //버리기 누름       발생
        Subject<ICusorPointerHandler>.Attach(_InvenPresent);    //커서 인아웃       발생
        Subject<IBoxHandler>.Attach(_InvenPresent);             //상자 상호작용     발생
        Subject<IWorkStation>.Attach(_InvenPresent);            //제작대 상호작용     발생
        Subject<ICraftItemHandler>.Attach(_InvenPresent);       //제작대 생성버튼 상호작용 발생
    }
    private void OnDisable()
    {
        Subject<ISlotExchangeHandler>.Detach(_InvenPresent);
        Subject<ISlotChanged>.Detach(_InvenPresent);
        Subject<ISlotClickHandler>.Detach(_InvenPresent);
        Subject<IPlayerInteractHandler>.Detach(_InvenPresent);
        Subject<ISlotClickRightHandler>.Detach(_facadeView);
        Subject<IButtonHandler>.Detach(_InvenPresent);
        Subject<ICusorPointerHandler>.Detach(_InvenPresent);
        Subject<IBoxHandler>.Detach(_InvenPresent);
        Subject<IWorkStation>.Detach(_InvenPresent);   
        Subject<ICraftItemHandler>.Detach(_InvenPresent);  
    }

    public void InitBoxes()
    {
        for (int i = 0; i < boxNum; i++)
        {
            //모델 생성
            InventoryModel boxmodel = new InventoryModel();
            boxesObj.GetChild(i).GetComponent<Box>().SetModel(boxmodel);
            //Debug.Log($"박스 모델{boxmodel} 생성");
            //모델 초기화(아이템 랜덤생성)
            _InvenPresent.InitBoxModel(boxmodel, _boxCapacity);
        }
    }
}