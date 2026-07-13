using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private float shootInterval;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    
    [SerializeField] private List<Enemy> enemies;

    private float shootTimer;

    private void Update()
    {
       Move();
       Shoot();
    }

    private void Move()
    {
        var forward = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        transform.position += forward * moveSpeed * Time.deltaTime;
    }

    private void Shoot()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            shootTimer = 0;
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var rBody = bullet.GetComponent<Rigidbody2D>();
            var closestEnemy = FindClosesEnemy(); 
            var direction = (closestEnemy.transform.position - transform.position).normalized;
            rBody.AddForce(direction * bulletSpeed);
        }
    }

    private Enemy FindClosesEnemy()
    {
        Enemy closestEnemy = null;
        var minDistance = float.MaxValue;
        
        foreach (var enemy in enemies)
        {
            var distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                closestEnemy = enemy;
                minDistance = distance;
            }
        }
        return closestEnemy;
    }
}
