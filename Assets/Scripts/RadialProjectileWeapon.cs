using UnityEngine;

public sealed class RadialProjectileWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 6f;
    [SerializeField, Min(0.05f)] private float attackInterval = 2f;
    [SerializeField, Min(0f)] private float projectileSpeed = 350f;
    [SerializeField, Min(1)] private int projectileCount = 8;
    private float timer;
    private float angleOffset;

    protected override void OnInitialized() => timer = attackInterval;

    public override void Tick(float deltaTime)
    {
        timer = Mathf.Min(timer + deltaTime, attackInterval);
        if (timer < attackInterval)
            return;

        timer = 0f;
        var step = 360f / projectileCount;
        var shot = new ProjectileShotData(damage, OwnerTeam);
        for (var i = 0; i < projectileCount; i++)
        {
            var angle = (angleOffset + step * i) * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            ProjectileFactory.Create(Owner.position, direction, projectileSpeed, shot);
        }
        angleOffset = (angleOffset + step * 0.5f) % 360f;
    }
}
