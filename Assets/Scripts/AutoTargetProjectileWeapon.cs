using UnityEngine;

public sealed class AutoTargetProjectileWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.05f)] private float attackInterval = 1f;
    [SerializeField, Min(0f)] private float projectileSpeed = 400f;

    private float attackTimer;

    protected override void OnInitialized()
    {
        attackTimer = attackInterval;
    }

    public override void Tick(float deltaTime)
    {
        attackTimer = Mathf.Min(attackTimer + deltaTime, attackInterval);
        if (attackTimer < attackInterval)
            return;

        var enemy = FindClosestEnemy?.Invoke(Owner.position);
        if (enemy == null)
            return;

        attackTimer = 0f;
        var direction = (enemy.transform.position - Owner.position).normalized;
        ProjectileFactory.Create(
            Owner.position,
            direction,
            projectileSpeed,
            new ProjectileShotData(damage, OwnerTeam));
    }
}
