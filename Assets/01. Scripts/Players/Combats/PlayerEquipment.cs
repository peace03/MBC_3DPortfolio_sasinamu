using UnityEngine;

public class PlayerEquipment : MonoBehaviour, IEquipmentSlotHandler, IPlayerQuickSlotHandler
{
    [SerializeField] private int equipmentCount;
    [SerializeField] private Item[] equipments;        // 0, 1 : 총 / 2 : 가방 / 3 : 방어구

    private PlayerStat stat;
    private PlayerQuickSlot quickSlot;

    private void Awake()
    {
        equipments = new Item[equipmentCount];
    }

    private void OnEnable()
    {
        // 장전 이벤트 구독
        
        Subject<IEquipmentSlotHandler>.Attach(this);
        Subject<IPlayerQuickSlotHandler>.Attach(this);
    }

    private void OnDisable()
    {
        // 장전 이벤트 구독 해제

        Subject<IEquipmentSlotHandler>.Detach(this);
        Subject<IPlayerQuickSlotHandler>.Detach(this);
    }

    public void Init(PlayerStat stat, PlayerQuickSlot slot)
    {
        this.stat = stat;
        quickSlot = slot;
        OnQuickSlot(slot.CurSelectedSlot);
        UpdateDefensePower();
    }

    public void OnEquipmentSlot(InventoryModel model)
    {
        for (int i = 0; i < equipmentCount; i++)
            equipments[i] = model.GetItem(i);

        OnQuickSlot(quickSlot.CurSelectedSlot);

        if (equipments[2] != null)
            Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerVisual(PlayerVisualType.Inv, PlayerVisualState.LargeInv));
        else
            Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerVisual(PlayerVisualType.Inv, PlayerVisualState.None));

        UpdateDefensePower();
    }

    public void OnQuickSlot(int slotNumber)
    {
        if (slotNumber > 2)
            return;

        if (equipments[slotNumber - 1] != null)
        {
            var item = equipments[slotNumber - 1] as GunItem;
            var data = item._data as GunData;
            stat.UpdateAttackPower(data.BulletData.Damage);

            if (data.BulletData.BulletType == BulletType.S)
                Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerVisual(PlayerVisualType.Arm, PlayerVisualState.Glock));
            else
                Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerVisual(PlayerVisualType.Arm, PlayerVisualState.AK47));
        }
        else
        {
            stat.UpdateAttackPower(0f);
            Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerVisual(PlayerVisualType.Arm, PlayerVisualState.None));
        }
    }

    private void UpdateDefensePower()
    {
        if (equipments[3] != null)
        {
            var item = equipments[3] as VestItem;
            var data = item._data as VestData;
            stat.UpdateDefensePower(data.Defensive);

            if (data.Defensive == 10f)
                Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerVisual(PlayerVisualType.Vest, PlayerVisualState.OldVest));
            else
                Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerVisual(PlayerVisualType.Vest, PlayerVisualState.NewVest));
        }
        else
        {
            stat.UpdateDefensePower(0f);
            Subject<IPlayerVisualHandler>.Publish(h => h.OnPlayerVisual(PlayerVisualType.Vest, PlayerVisualState.None));
        }
    }

    // 장전 관리 함수
}