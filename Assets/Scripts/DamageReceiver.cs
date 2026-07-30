using UnityEngine;

public sealed class DamageReceiver : MonoBehaviour, IDamageable
{
    [SerializeField] private Character owner;
    [SerializeField] private CombatTeam team;
    [SerializeField] private DamageFlash damageFlash;

    public CombatTeam Team => team;

    public void TakeDamage(float damage)
    {
        if (owner == null || owner.IsDead || damage <= 0f)
            return;

        owner.TakeDamage(damage);
        damageFlash?.Play();
    }

    private void OnValidate()
    {
        if (owner == null)
            owner = GetComponentInParent<Character>();

        if (damageFlash == null)
            damageFlash = GetComponentInParent<DamageFlash>();
    }
}
