using System.Collections.Generic;
using UnityEngine;

public static class CombatDamage
{
    private static readonly ContactFilter2D EnemyFilter = new()
    {
        useLayerMask = true,
        layerMask = 1 << 7,
        useTriggers = true
    };

    public static int ApplyCircle(
        Vector2 center,
        float radius,
        float damage,
        CombatTeam sourceTeam,
        List<Collider2D> buffer,
        HashSet<DamageReceiver> uniqueTargets = null)
    {
        buffer.Clear();
        Physics2D.OverlapCircle(center, radius, EnemyFilter, buffer);

        var damaged = 0;
        for (var i = 0; i < buffer.Count; i++)
        {
            if (!buffer[i].TryGetComponent<DamageReceiver>(out var receiver))
                continue;

            if (receiver.Team == sourceTeam)
                continue;

            if (uniqueTargets != null && !uniqueTargets.Add(receiver))
                continue;

            receiver.TakeDamage(damage);
            damaged++;
        }

        return damaged;
    }

    public static int ApplyBox(
        Vector2 center,
        Vector2 size,
        float angle,
        float damage,
        CombatTeam sourceTeam,
        List<Collider2D> buffer,
        HashSet<DamageReceiver> uniqueTargets = null)
    {
        buffer.Clear();
        Physics2D.OverlapBox(center, size, angle, EnemyFilter, buffer);

        var damaged = 0;
        for (var i = 0; i < buffer.Count; i++)
        {
            if (!buffer[i].TryGetComponent<DamageReceiver>(out var receiver))
                continue;

            if (receiver.Team == sourceTeam)
                continue;

            if (uniqueTargets != null && !uniqueTargets.Add(receiver))
                continue;

            receiver.TakeDamage(damage);
            damaged++;
        }

        return damaged;
    }
}
