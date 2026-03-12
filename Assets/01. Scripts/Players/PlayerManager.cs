using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("플레이어 스크립트들")]
    [SerializeField] private PlayerMove move;       // 이동

    [SerializeField] private PlayerStat stat;       // 스탯
    public PlayerStat Stat => stat;

    private void Awake()
    {
        // 초기화
        move.Init(this);
    }
}