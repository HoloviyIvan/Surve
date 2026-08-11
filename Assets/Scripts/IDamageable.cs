public interface IDamageable
{
    CombatTeam Team { get; }
    void TakeDamage(float damage);
}
