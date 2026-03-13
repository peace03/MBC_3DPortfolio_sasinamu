using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform player;      // 플레이어

    private PlayerStat stat;                        // 스탯
    public PlayerStat Stat => stat;

    private PlayerMove move;                        // 이동
    private PlayerInteract interact;                // 상호작용
    private PlayerFire fire;                        // 발사

    private void Awake()
    {
        // 초기화
        stat = player.GetComponent<PlayerStat>();
        move = player.GetComponent<PlayerMove>();
        move.Init(this);
        interact = player.GetComponent<PlayerInteract>();
        interact.Init(this);
        fire = player.GetComponent<PlayerFire>();
        fire.Init(this);
    }
}