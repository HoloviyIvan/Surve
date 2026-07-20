using System;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private float shootInterval;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    
    private float shootTimer;
    private Func<Vector3, Enemy> findClosestEnemy;

    public void Initialize(Func<Vector3, Enemy> enemyProvider)
    {
        findClosestEnemy = enemyProvider;
    }

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
        if (findClosestEnemy == null)
            return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            var closestEnemy = findClosestEnemy(transform.position);
            if (closestEnemy == null)
                return;

            shootTimer = 0f;
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var rBody = bullet.GetComponent<Rigidbody2D>();
            var direction = (closestEnemy.transform.position - transform.position).normalized;
            rBody.AddForce(direction * bulletSpeed);
        }
    }
}
