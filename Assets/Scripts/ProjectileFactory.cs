using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public sealed class ProjectileFactory : IProjectileFactory, IDisposable
{
    private readonly Projectile projectilePrefab;
    private readonly Transform container;
    private readonly ObjectPool<Projectile> pool;

    public ProjectileFactory(Projectile projectilePrefab, Transform container, int initialPoolSize, int maxPoolSize)
    {
        this.projectilePrefab = projectilePrefab;
        this.container = container;
        pool = new ObjectPool<Projectile>(
            CreateInstance, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject,
            collectionCheck: true, defaultCapacity: initialPoolSize, maxSize: maxPoolSize);
        Prewarm(initialPoolSize);
    }

    public Projectile Create(
        Vector3 position,
        Vector2 direction,
        float speed,
        ProjectileShotData shotData)
    {
        var projectile = pool.Get();
        projectile.transform.SetPositionAndRotation(position, Quaternion.identity);
        projectile.Launch(direction, speed, shotData, Release);
        return projectile;
    }

    public void Dispose() => pool.Dispose();

    private Projectile CreateInstance()
    {
        var projectile = Object.Instantiate(projectilePrefab, container);
        projectile.gameObject.SetActive(false);
        return projectile;
    }

    private static void OnTakeFromPool(Projectile projectile) => projectile.gameObject.SetActive(true);

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
