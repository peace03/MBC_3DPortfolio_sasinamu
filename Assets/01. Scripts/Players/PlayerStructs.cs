// 플레이어 스탯 정보 구조체
[System.Serializable]
public struct PlayerStatInfo
{
    private float attackPower;      // 공격력
    public float AttackPower => attackPower;

    private float defensePower;     // 방어력
    public float DefensePower => defensePower;

    public PlayerStatInfo(float attack, float defense)
    {
        attackPower = attack;
        defensePower = defense;
    }
}