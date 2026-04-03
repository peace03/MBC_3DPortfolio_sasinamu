using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    [Header("정보")]
    [SerializeField] private ItemManager itemManager;
    private InventoryModel _boxModel;
    private int _defaultCapacity = 5;

    // [핵심 로직 추가] 난수 기반 전리품 생성 알고리즘
    private void GenerateRandomLoot()
    {
        if (itemManager == null) return;
        // 1. 이 상자에 몇 개의 아이템을 넣을지 난수 결정
        int randomCount = Random.Range(1, 6);

        for (int i = 0; i < randomCount; i++)
        {
            // 3. 어떤 아이템을 뽑을지 ID 난수 결정 (예: 1번~10번 아이템 중 하나)
            // 실제 프로젝트의 아이템 ID 범위나 기획 확률표에 맞게 수정하십시오.
            int randomItemID = Random.Range(1, 25);

            // 4. ItemManager에게 "ID n번 아이템 인스턴스를 메모리에 찍어내 줘!"라고 요청
            // (학생의 프로젝트 구조에 맞춰 CreateItem, GetItemInstance 등 알맞은 함수명으로 변경하세요)
            Item newLoot = itemManager.CreateItemInstance(randomItemID);

            if (newLoot != null)
            {
                Debug.Log(_boxModel);
                // 5. 생성된 아이템을 상자 인벤토리의 i번째 슬롯에 안전하게 주입
                _boxModel.PutItem(SlotType.Box, i, newLoot);
            }
        }

        Debug.Log($"[Box] 월드 상자 초기화 완료. {randomCount}개의 전리품이 생성되었습니다.");
    }

    public void SetModel(InventoryModel model)
    {
        _boxModel = model;
    }

    // 상호작용 함수
    public void Interact()
    {
        // [지연 초기화] 플레이어가 최초로 E키를 눌렀을 때만 힙(Heap) 메모리를 할당하고 아이템을 채웁니다.
        if (_boxModel == null)
        {
            Debug.Log("들어오니");
            _boxModel = new InventoryModel();
            Debug.Log(_boxModel);
            _boxModel.Init(_defaultCapacity);
            Debug.Log($"{_boxModel.Capacity}");

            StartCoroutine(WaitInitBoxModel());
            // 💡 여기서 아이템이 배열에 꽂히며 확성기(Subject)가 울리지만,
            // 이미 플레이어가 상자와 상호작용하는 흐름 안에 있으므로 전혀 문제없이 화면에 그려집니다.
        }

        // 상자 이벤트 발생(상자 아이템들 정보 전달)
        Subject<IBoxHandler>.Publish(h => h.OnBox(_boxModel));
        Debug.Log("상호작용");
    }

    private IEnumerator WaitInitBoxModel()
    {
        Subject<IBoxModelHandler>.Publish(h => h.OnInitBoxModel(_boxModel));
        yield return null;
        GenerateRandomLoot();
        yield return null;
    }

    // [핵심 추가] 동적 스폰된 상자에 오버플로우 데이터를 주입하는 함수
    public void InitOverflowItems(List<Item> overflowItems)
    {
        // 아이템 개수에 맞춰 상자의 용량을 동적 할당 (최소 5칸 보장)
        int capacity = Mathf.Max(5, overflowItems.Count);

        // 상자만의 개별 모델을 힙(Heap) 메모리에 새로 할당
        _boxModel = new InventoryModel();
        _boxModel.Init(capacity);

        // 주입받은 아이템 데이터를 상자 모델 배열에 차곡차곡 적재
        for (int i = 0; i < overflowItems.Count; i++)
        {
            Debug.Log(overflowItems[i]);
            // 💡 프로젝트의 InventoryModel에 구현된 아이템 삽입 메서드(PutItem, AddItem 등)를 사용하십시오.
            _boxModel.PutItem(SlotType.Box, i, overflowItems[i]);
        }

        Debug.Log($"[Box] 오버플로우 상자 생성 완료. {overflowItems.Count}개의 아이템이 보관되었습니다.");
    }
}