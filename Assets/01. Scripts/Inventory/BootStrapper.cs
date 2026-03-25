using System.Collections.Generic;
using UnityEngine;

public class BootStrapper : MonoBehaviour
{
    [SerializeField] private ItemManager _itemManager;

    [Header("User 인벤토리 의존성 주입")] //BootStrapper 역할: P에 MV 연결해줌
    public FacadeView _facadeView; //인스펙터에서 연결
    private InventoryModel _userInvenModel;
    private InventoryModel _equipInvenModel;
    private InventoryPresenter _InvenPresent;

    [Header("인벤토리 용량")]
    [SerializeField] private int _equipCapacity = 5;    //장비
    [SerializeField] private int _bagCapacity = 20;     //가방
    [SerializeField] private int _quickCapacity = 6;    //퀵슬롯
    [SerializeField] private int _storageCapacity = 60; //창고
    [SerializeField] private int _boxCapacity = 10;     //상자

    private void Awake()
    {
        //User Presenter에 M, V 연결
        _equipInvenModel = new InventoryModel();
        _userInvenModel = new InventoryModel();
        _InvenPresent = new InventoryPresenter(_facadeView, _userInvenModel, _equipInvenModel, _itemManager);

        //모델별 슬롯 용량 초기화
        _equipInvenModel.SetCapacity(_equipCapacity);
        _userInvenModel.SetCapacity(_bagCapacity);
        _equipInvenModel.Init();
        _userInvenModel.Init();
    }
    private void Start()
    {
        
        _InvenPresent.InitializePresenter();
    }

    private void OnEnable()
    {
        Subject<ISlotExchangeHandler>.Attach(_InvenPresent);
    }
    private void OnDisable()
    {
        Subject<ISlotExchangeHandler>.Detach(_InvenPresent);
    }
}
