using System.Collections.Generic;
using UnityEngine;

public class BootStrapper : MonoBehaviour
{
    [SerializeField] private ItemManager _itemManager;

    [Header("User 인벤토리 의존성 주입")] //BootStrapper 역할: P에 MV 연결해줌
    public UserInventoryView _userInvenView; //인스펙터에서 연결
    private UserInventoryModel _userInvenModel;
    private UserInventoryPresenter _userInvenPresent;

    //[Header("Box 인벤토리 의존성 주입")]
    //public BoxInventoryView _boxInvenView; //인스펙터에서 연결
    //private BoxInventoryModel _boxInvenModel;
    //private BoxInventoryPresenter _boxInvenPresent;

    private void Awake()
    {
        //User Presenter에 M, V 연결
        _userInvenModel = new UserInventoryModel();
        _userInvenPresent = new UserInventoryPresenter(_userInvenView, _userInvenModel, _itemManager);

        ////Box Presenter에 M, V 연결
        //_boxInvenModel = new BoxInventoryModel();
        //_boxInvenPresent = new BoxInventoryPresenter(_boxInvenView, _boxInvenModel, _itemManager);
    }
    private void Start()
    {
        _userInvenPresent.InitializePresenter();
    }
}
