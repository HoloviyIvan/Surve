using System;
using UnityEngine;

public class Enemy : Character
{
    private Transform target;
    private void Start()
    {
        target = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        var direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
} 
