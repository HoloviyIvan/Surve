using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] protected float moveSpeed;
    public float Health {get; private set;}
    
    public bool IsDead => Health <= 0;

    protected virtual void Awake()
    {
        RestoreHealth();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        Health = Mathf.Max(Health - damage, 0f);

        if (IsDead)
            OnDeath();
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        Health = Mathf.Min(Health + amount, maxHealth);
    }

    public void RestoreHealth() => Health = maxHealth;

    protected virtual void OnDeath()
    {
    }
}
