using System.Collections.Generic;
using UnityEngine;

public sealed class AuraWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 3f;
    [SerializeField, Min(0.05f)] private float damageInterval = 0.5f;
    [SerializeField, Min(0f)] private float radius = 2.5f;
    [SerializeField] private SpriteRenderer auraRenderer;

    private readonly List<Collider2D> overlapBuffer = new();
    private readonly HashSet<DamageReceiver> uniqueTargets = new();
    private float timer;

    protected override void OnInitialized()
    {
        if (auraRenderer == null || auraRenderer.sprite == null)
            throw new MissingReferenceException("AuraWeapon: Aura Renderer or its Sprite is not assigned.");

        var spriteDiameter = Mathf.Max(
            auraRenderer.sprite.bounds.size.x,
            auraRenderer.sprite.bounds.size.y);
        var parentScale = auraRenderer.transform.parent.lossyScale;
        var worldScale = radius * 2f / spriteDiameter;

        auraRenderer.transform.localScale = new Vector3(
            worldScale / Mathf.Max(Mathf.Abs(parentScale.x), 0.0001f),
            worldScale / Mathf.Max(Mathf.Abs(parentScale.y), 0.0001f),
            1f);
        auraRenderer.transform.localPosition = Vector3.zero;
    }

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
