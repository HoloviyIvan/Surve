using System.Collections.Generic;
using UnityEngine;

public sealed class BulletProjectile : Projectile
{
    private readonly HashSet<DamageReceiver> hitTargets = new();
    private readonly List<Collider2D> overlapBuffer = new();
    private int remainingPierces;

    protected override void OnLaunched()
    {
        remainingPierces = Mathf.Max(0, ShotData.PierceCount);
        hitTargets.Clear();
    }

    protected override bool HandleCollision(Collider2D other)
    {
        if (!other.TryGetComponent<DamageReceiver>(out var receiver))
            return false;

        if (receiver.Team == ShotData.SourceTeam || !hitTargets.Add(receiver))
            return false;

        receiver.TakeDamage(ShotData.Damage);

        if (ShotData.ExplosionRadius > 0f)
        {
            CombatDamage.ApplyCircle(
                transform.position,
                ShotData.ExplosionRadius,
                ShotData.Damage,
                ShotData.SourceTeam,
                overlapBuffer,
                hitTargets);
        }

        if (remainingPierces <= 0)
            return true;

        remainingPierces--;
        return false;
    }

    protected override void OnResetState()
    {
        remainingPierces = 0;
        hitTargets.Clear();
        overlapBuffer.Clear();
    }
}
