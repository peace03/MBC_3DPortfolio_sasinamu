using UnityEngine;

public class SandBag : MonoBehaviour, IDamageable
{
    [Header("스탯")]
    [SerializeField] private float maxDamagedCount;     // 최대 피격 횟수

    private float damagedCount = 0f;

    public void Damaged(string name, float amount)
    {
        // 피격 횟수 증가
        damagedCount++;

        // 피격 횟수가 최대 피격 횟수를 넘었다면
        if (damagedCount >= maxDamagedCount)
            // 비활성화
            gameObject.SetActive(false);
    }
}