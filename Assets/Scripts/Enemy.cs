using System;
using UnityEngine;

public class Enemy : Character
{
    private Transform target;
    private Action<Enemy> release;

    public void Initialize(Transform newTarget, Action<Enemy> releaseAction)
    {
        target = newTarget;
        release = releaseAction;
        RestoreHealth();
    }

    private void Update()
    {
        if (target == null || IsDead)
            return;

        var direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    protected override void OnDeath()
    {
        release?.Invoke(this);
    }
} 
