using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public sealed class OrbitingBlade : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float spinSpeed = -720f;

    private readonly Dictionary<DamageReceiver, float> nextDamageTimes = new();
    private float damage;
    private float damageInterval;
    private CombatTeam sourceTeam;
    private bool isInitialized;

    public void Initialize(float newDamage, float newDamageInterval, CombatTeam newSourceTeam)
    {
        if (body == null)
            throw new MissingReferenceException("OrbitingBlade: Rigidbody2D is not assigned.");

        damage = newDamage;
        damageInterval = newDamageInterval;
        sourceTeam = newSourceTeam;
        nextDamageTimes.Clear();
        isInitialized = true;
    }

    public void MoveAndRotate(Vector2 position, float deltaTime)
    {
        body.position = position;
        transform.Rotate(0f, 0f, spinSpeed * deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDealDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDealDamage(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var receiver = other.GetComponentInParent<DamageReceiver>();
        if (receiver != null)
            nextDamageTimes.Remove(receiver);
    }

    private void OnDisable()
    {
        nextDamageTimes.Clear();
        isInitialized = false;
    }

    private void TryDealDamage(Collider2D other)
    {
        if (!isInitialized)
            return;

        var receiver = other.GetComponentInParent<DamageReceiver>();
        if (receiver == null || receiver.Team == sourceTeam)
            return;

        if (nextDamageTimes.TryGetValue(receiver, out var nextDamageTime) &&
            Time.time < nextDamageTime)
            return;

        receiver.TakeDamage(damage);
        nextDamageTimes[receiver] = Time.time + damageInterval;
    }
}
