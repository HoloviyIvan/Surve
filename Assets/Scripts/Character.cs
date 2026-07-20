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
        if (IsDead)
            return;

        Health -= damage;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        Debug.Log("health changed: " + damage + ", health: " + Health +", " + name);

        if (IsDead)
            OnDeath();
    }

    public void RestoreHealth() => Health = maxHealth;

    protected virtual void OnDeath()
    {
    }
}
