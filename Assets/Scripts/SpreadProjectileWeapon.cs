using UnityEngine;

public sealed class SpreadProjectileWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 5f;
    [SerializeField, Min(0.05f)] private float attackInterval = 1.5f;
    [SerializeField, Min(0f)] private float projectileSpeed = 400f;
    [SerializeField, Min(1)] private int projectileCount = 5;
    [SerializeField, Range(0f, 180f)] private float spreadAngle = 45f;
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
        var baseDirection = (enemy.transform.position - Owner.position).normalized;
        var startAngle = projectileCount == 1 ? 0f : -spreadAngle * 0.5f;
        var step = projectileCount == 1 ? 0f : spreadAngle / (projectileCount - 1);
        var shot = new ProjectileShotData(damage, OwnerTeam);

        for (var i = 0; i < projectileCount; i++)
        {
            var direction = Quaternion.Euler(0f, 0f, startAngle + step * i) * baseDirection;
            ProjectileFactory.Create(Owner.position, direction, projectileSpeed, shot);
        }
    }
}
