using System.Collections.Generic;
using UnityEngine;

public sealed class AuraWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 3f;
    [SerializeField, Min(0.05f)] private float damageInterval = 0.5f;
    [SerializeField, Min(0f)] private float radius = 2.5f;

    private readonly List<Collider2D> overlapBuffer = new();
    private readonly HashSet<DamageReceiver> uniqueTargets = new();
    private float timer;

    public override void Tick(float deltaTime)
    {
        timer += deltaTime;
        if (timer < damageInterval)
            return;
        timer -= damageInterval;
        uniqueTargets.Clear();
        CombatDamage.ApplyCircle(Owner.position, radius, damage, OwnerTeam, overlapBuffer, uniqueTargets);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
