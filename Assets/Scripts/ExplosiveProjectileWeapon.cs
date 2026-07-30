using UnityEngine;

public sealed class ExplosiveProjectileWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 12f;
    [SerializeField, Min(0.05f)] private float attackInterval = 2.5f;
    [SerializeField, Min(0f)] private float projectileSpeed = 300f;
    [SerializeField, Min(0f)] private float explosionRadius = 2f;
    private float timer;

    protected override void OnInitialized() => timer = attackInterval;

    public override void Tick(float deltaTime)
    {
        timer = Mathf.Min(timer + deltaTime, attackInterval);
        if (timer < attackInterval)
            return;
        var enemy = FindClosestEnemy?.Invoke(Owner.position);
        if (enemy == null)
            return;
        timer = 0f;
        var direction = (enemy.transform.position - Owner.position).normalized;
        ProjectileFactory.Create(Owner.position, direction, projectileSpeed,
            new ProjectileShotData(damage, OwnerTeam, explosionRadius: explosionRadius));
    }
}
