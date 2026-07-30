using UnityEngine;

public sealed class PiercingProjectileWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 14f;
    [SerializeField, Min(0.05f)] private float attackInterval = 1.8f;
    [SerializeField, Min(0f)] private float projectileSpeed = 500f;
    [SerializeField, Min(0)] private int pierceCount = 3;
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
            new ProjectileShotData(damage, OwnerTeam, pierceCount));
    }
}
