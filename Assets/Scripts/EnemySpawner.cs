using UnityEngine;

public sealed class EnemySpawner
{
    private readonly IEnemyFactory factory;
    private readonly Transform center;
    private readonly float spawnInterval;
    private readonly float minSpawnRadius;
    private readonly float maxSpawnRadius;
    private readonly int maxAliveEnemies;

    private float timer;

    public EnemySpawner(
        IEnemyFactory factory,
        Transform center,
        float spawnInterval,
        float minSpawnRadius,
        float maxSpawnRadius,
        int maxAliveEnemies)
    {
        this.factory = factory;
        this.center = center;
        this.spawnInterval = spawnInterval;
        this.minSpawnRadius = minSpawnRadius;
        this.maxSpawnRadius = maxSpawnRadius;
        this.maxAliveEnemies = maxAliveEnemies;
    }

    public void Tick(float deltaTime)
    {
        timer += deltaTime;

        if (timer < spawnInterval || factory.ActiveCount >= maxAliveEnemies)
            return;

        timer -= spawnInterval;
        factory.Create(GetSpawnPosition());
    }

    private Vector3 GetSpawnPosition()
    {
        var direction = Random.insideUnitCircle.normalized;
        var distance = Random.Range(minSpawnRadius, maxSpawnRadius);
        return center.position + (Vector3)(direction * distance);
    }
}
