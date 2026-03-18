using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform player;                  // 플레이어

    [Header("매니저")]
    [SerializeField] private InputManager inputManager;         // 인풋 매니저

    [Header("총알")]
    [SerializeField] private BulletFactory bulletFactory;       // 총알 공장

    private PlayerStat stat;                                    // 스탯
    public PlayerStat Stat => stat;

    private PlayerMove move;                                    // 이동
    private PlayerRotate rotate;                                // 회전
    private PlayerInteract interact;                            // 상호작용
    private PlayerFire fire;                                    // 발사

    private void Awake()
    {
        // 초기화
        stat = player.GetComponent<PlayerStat>();
        move = player.GetComponent<PlayerMove>();
        rotate = player.GetComponent<PlayerRotate>();
        interact = player.GetComponent<PlayerInteract>();
        fire = player.GetComponent<PlayerFire>();

        move.Init(stat, inputManager);
        rotate.Init(inputManager);
        interact.Init(stat);
        fire.Init(stat, bulletFactory);
    }
}