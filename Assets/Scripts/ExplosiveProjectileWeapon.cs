using UnityEngine;

public sealed class ExplosiveProjectileWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 12f;
    [SerializeField, Min(0.05f)] private float attackInterval = 2.5f;
    [SerializeField, Min(0f)] private float explosionRadius = 2f;
    [SerializeField, Min(0.05f)] private float flightDuration = 0.9f;
    [SerializeField, Min(0f)] private float arcHeight = 1.5f;
    [SerializeField] private GrenadeProjectile grenadePrefab;
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

        if (grenadePrefab == null)
            throw new MissingReferenceException("ExplosiveProjectileWeapon: Grenade Prefab is not assigned.");

        timer = 0f;
        var targetPosition = (Vector2)enemy.transform.position;
        ProjectileFactory.Create(
            grenadePrefab,
            Owner.position,
            Vector2.zero,
            0f,
            new ProjectileShotData(
                damage,
                OwnerTeam,
                explosionRadius: explosionRadius,
                targetPosition: targetPosition,
                flightDuration: flightDuration,
                arcHeight: arcHeight));
    }
}
