using UniRx;

public interface IDamageable
{
    IReadOnlyReactiveProperty<int> CurrentHp { get; }
    void TakeDamage(int damage);
}
