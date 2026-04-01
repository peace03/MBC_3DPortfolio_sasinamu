using UnityEngine;

// 씬 전환 시 파괴되지 않는 전역 데이터 보관소
public class GameDataManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static GameDataManager _instance;
    public static GameDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameDataManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameDataManager");
                    _instance = go.AddComponent<GameDataManager>();
                    DontDestroyOnLoad(go); // 핵심: 씬이 넘어가도 파괴되지 않음
                }
            }
            return _instance;
        }
    }

    // 영구 보존할 데이터 모델들 (프로퍼티로 캡슐화)
    public InventoryModel EquipModel { get; private set; }
    public InventoryModel BagModel { get; private set; }
    public InventoryModel StorageModel { get; private set; }
    public InventoryModel QuickModel { get; private set; }
    // [추가된 부분] 게임 실행 후 최초 아이템 지급 여부를 추적하는 플래그
    public bool HasSpawnedInitialItems { get; set; } = false;

    private bool isInitialized = false; // 초기화 중복 방지 플래그

    private void Awake()
    {
        // 씬 로드 시 중복 생성 방지
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 최초 1회만 모델들을 메모리에 할당하고 초기화합니다.
    public void InitInventoriesOnce(int equipCap, int bagCap, int storageCap, int quickCap)
    {
        // 이미 초기화되었다면(씬을 이동해서 온 거라면) 생성 로직을 무시합니다.
        if (isInitialized) return;

        EquipModel = new InventoryModel();
        EquipModel.Init(equipCap);

        BagModel = new InventoryModel();
        BagModel.Init(bagCap);

        StorageModel = new InventoryModel();
        StorageModel.Init(storageCap);

        QuickModel = new InventoryModel();
        QuickModel.Init(quickCap);

        isInitialized = true;
        Debug.Log("[GameDataManager] 런타임 인벤토리 모델들이 힙 메모리에 영구 할당되었습니다.");
    }
}