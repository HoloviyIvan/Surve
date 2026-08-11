using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public sealed class ProjectileFactory : IProjectileFactory, IDisposable
{
    private readonly Transform container;
    private readonly int maxPoolSize;
    private readonly Dictionary<Projectile, ProjectilePool> pools = new();
    private readonly ProjectilePool defaultPool;

    public ProjectileFactory(
        Projectile projectilePrefab,
        Transform container,
        int initialPoolSize,
        int maxPoolSize)
    {
        this.container = container;
        this.maxPoolSize = maxPoolSize;
        defaultPool = CreatePool(projectilePrefab, initialPoolSize);
    }

    public Projectile Create(
        Vector3 position,
        Vector2 direction,
        float speed,
        ProjectileShotData shotData)
    {
        return defaultPool.Spawn(position, direction, speed, shotData);
    }

    public Projectile Create(
        Projectile prefab,
        Vector3 position,
        Vector2 direction,
        float speed,
        ProjectileShotData shotData)
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab));

        if (!pools.TryGetValue(prefab, out var pool))
            pool = CreatePool(prefab, 0);

        return pool.Spawn(position, direction, speed, shotData);
    }

    public void Dispose()
    {
        foreach (var pool in pools.Values)
            pool.Dispose();

        pools.Clear();
    }

    private ProjectilePool CreatePool(Projectile prefab, int initialSize)
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab));

        var pool = new ProjectilePool(prefab, container, initialSize, maxPoolSize);
        pools.Add(prefab, pool);
        return pool;
    }

    private sealed class ProjectilePool : IDisposable
    {
        private readonly Projectile prefab;
        private readonly Transform container;
        private readonly ObjectPool<Projectile> pool;

        public ProjectilePool(
            Projectile prefab,
            Transform container,
            int initialSize,
            int maxSize)
        {
            this.prefab = prefab;
            this.container = container;
            pool = new ObjectPool<Projectile>(
                CreateInstance,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck: true,
                defaultCapacity: initialSize,
                maxSize: maxSize);
            Prewarm(initialSize);
        }

        public Projectile Spawn(
            Vector3 position,
            Vector2 direction,
            float speed,
            ProjectileShotData shotData)
        {
            var projectile = pool.Get();
            projectile.SetSpawnPosition(position);
            projectile.Launch(direction, speed, shotData, Release);
            return projectile;
        }

        public void Dispose() => pool.Dispose();

        private Projectile CreateInstance()
        {
            var projectile = Object.Instantiate(prefab, container);
            projectile.gameObject.SetActive(false);
            return projectile;
        }

        private static void OnTakeFromPool(Projectile projectile) =>
            projectile.gameObject.SetActive(true);

        private static void OnReturnedToPool(Projectile projectile)
        {
            projectile.ResetState();
            projectile.gameObject.SetActive(false);
        }

        private static void OnDestroyPoolObject(Projectile projectile)
        {
            if (projectile != null)
                Object.Destroy(projectile.gameObject);
        }

        private void Release(Projectile projectile)
        {
            if (projectile != null && projectile.gameObject.activeSelf)
                pool.Release(projectile);
        }

        private void Prewarm(int count)
        {
            var projectiles = new Projectile[count];
            for (var i = 0; i < count; i++)
                projectiles[i] = pool.Get();

            for (var i = 0; i < count; i++)
                pool.Release(projectiles[i]);
        }
    }
}
