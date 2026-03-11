public interface IDamageable
{
    public int LayerNumber { get; }
    public float HpRatio { get; }
    public void Damaged(DamagedEvent data);
}