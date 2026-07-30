using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private CombatTeam sourceTeam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<DamageReceiver>(out var receiver))
            return;

        if (receiver.Team == sourceTeam)
            return;

        receiver.TakeDamage(damage);
    }
}
