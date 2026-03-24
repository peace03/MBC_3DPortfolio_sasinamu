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

    private PlayerAnimationChanger anim;                        // 애니메이션
    private PlayerMove move;                                    // 이동
    private PlayerRotate rotate;                                // 회전
    private PlayerInteract interact;                            // 상호작용
    private PlayerFire fire;                                    // 발사

    private void Awake()
    {
        // 초기화
        InitPlayer();
    }

    private void InitPlayer()
    {
        // 플레이어 컴포넌트 받아오기
        stat = player.GetComponent<PlayerStat>();
        anim = player.GetComponent<PlayerAnimationChanger>();
        move = player.GetComponent<PlayerMove>();
        rotate = player.GetComponent<PlayerRotate>();
        interact = player.GetComponent<PlayerInteract>();
        fire = player.GetComponent<PlayerFire>();

        // 컴포넌트 초기화
        move.Init(stat, anim, inputManager);
        rotate.Init(inputManager);
        interact.Init(stat);
        fire.Init(stat, bulletFactory);

        //equip.Init(mvp);
        //mvp.Init(equip);
    }
}