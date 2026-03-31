using UnityEngine;

public class PlayerVisual : MonoBehaviour, IPlayerVisualHandler
{
    [Header("프리팹")]
    [SerializeField] private GameObject glockRightArm;
    [SerializeField] private GameObject ak47Arms;
    [SerializeField] private GameObject newVest;
    [SerializeField] private GameObject oldVest;
    [SerializeField] private GameObject largeInv;

    [Header("정보")]
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject rightArmTorus;
    [SerializeField] private GameObject inventory;

    private PlayerVisualState armState;
    private PlayerVisualState vestState;
    private PlayerVisualState invState;

    private void OnEnable()
    {
        Subject<IPlayerVisualHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IPlayerVisualHandler>.Detach(this);
    }

    public void OnPlayerVisual(PlayerVisualType type, PlayerVisualState state)
    {
        switch (type)
        {
            case PlayerVisualType.Arm:
                if (state == armState)
                    return;

                armState = state;
                ChangeArm();
                break;
            case PlayerVisualType.Vest:
                if (state == vestState)
                    return;

                vestState = state;
                ChangeVest();
                break;
            case PlayerVisualType.Inv:
                if (state == invState)
                    return;

                invState = state;
                ChangeInventory();
                break;
        }
    }

    private void ChangeArm()
    {
        leftArm.SetActive(false);
        rightArm.SetActive(false);
        rightArmTorus.SetActive(false);
        glockRightArm.SetActive(false);
        ak47Arms.SetActive(false);

        switch (armState)
        {
            case PlayerVisualState.None:
                leftArm.SetActive(true);
                rightArm.SetActive(true);
                rightArmTorus.SetActive(true);
                break;
            case PlayerVisualState.Glock:
                glockRightArm.SetActive(true);
                break;
            case PlayerVisualState.AK47:
                ak47Arms.SetActive(true);
                break;
        }
    }

    private void ChangeVest()
    {
        newVest.SetActive(false);
        oldVest.SetActive(false);

        switch (vestState)
        {
            case PlayerVisualState.None:
                return;
            case PlayerVisualState.NewVest:
                newVest.SetActive(true);
                break;
            case PlayerVisualState.OldVest:
                oldVest.SetActive(true);
                break;
        }
    }

    private void ChangeInventory()
    {
        inventory.SetActive(false);
        largeInv.SetActive(false);

        switch (invState)
        {
            case PlayerVisualState.None:
                inventory.SetActive(true);
                break;
            case PlayerVisualState.LargeInv:
                largeInv.SetActive(true);
                break;
        }
    }
}