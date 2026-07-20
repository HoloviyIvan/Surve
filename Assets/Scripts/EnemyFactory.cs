using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public sealed class EnemyFactory : IEnemyFactory, IDisposable
{
    private readonly Enemy enemyPrefab;
    private readonly Transform target;
    private readonly Transform container;
    private readonly ObjectPool<Enemy> pool;
    private readonly HashSet<Enemy> activeEnemies = new();

    public int ActiveCount => activeEnemies.Count;

    public EnemyFactory(
        Enemy enemyPrefab,
        Transform target,
        Transform container,
        int initialPoolSize,
        int maxPoolSize)
    {
        this.enemyPrefab = enemyPrefab;
        this.target = target;
        this.container = container;

        pool = new ObjectPool<Enemy>(
            CreateInstance,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: initialPoolSize,
            maxSize: maxPoolSize);

        Prewarm(initialPoolSize);
    }

    public Enemy Create(Vector3 position)
    {
        var enemy = pool.Get();
        enemy.transform.SetPositionAndRotation(position, Quaternion.identity);
        enemy.Initialize(target, Release);
        return enemy;
    }

    public Enemy FindClosest(Vector3 position)
    {
        Enemy closest = null;
        var closestDistance = float.MaxValue;

        foreach (var enemy in activeEnemies)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            var distance = (enemy.transform.position - position).sqrMagnitude;
            if (distance >= closestDistance)
                continue;

            closest = enemy;
            closestDistance = distance;
        }

        return closest;
    }

    public void Dispose() => pool.Dispose();

    private Enemy CreateInstance()
    {
        var enemy = Object.Instantiate(enemyPrefab, container);
        enemy.gameObject.SetActive(false);
        return enemy;
    }

    private void OnTakeFromPool(Enemy enemy)
    {
        activeEnemies.Add(enemy);
        enemy.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        enemy.gameObject.SetActive(false);
    }

    private static void OnDestroyPoolObject(Enemy enemy)
    {
        if (enemy != null)
            Object.Destroy(enemy.gameObject);
    }

    private void Release(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
            pool.Release(enemy);
    }

    private void Prewarm(int count)
    {
        var enemies = new Enemy[count];
        for (var i = 0; i < count; i++)
            enemies[i] = pool.Get();

        for (var i = 0; i < count; i++)
            pool.Release(enemies[i]);
    }
}
