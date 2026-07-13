using System;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hasHealth = collision.TryGetComponent<Character>(out var character);
        var otherHealth = !collision.CompareTag(tag);
        
        if (hasHealth && otherHealth)
        {
            character.TakeDamage(damage);
        }
    }
}
