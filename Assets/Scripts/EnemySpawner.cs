using UnityEngine;

public sealed class EnemySpawner
{
    private readonly IEnemyFactory factory;
    private readonly Transform center;
    private readonly EnemyWaveSettings settings;
    private readonly float minSpawnRadius;
    private readonly float maxSpawnRadius;

    private int waveIndex;
    private float waveTimer;
    private float spawnTimer;
    private float breakTimer;
    private State state;

    public int CurrentWaveNumber => waveIndex + 1;
    public bool IsCompleted => state == State.Completed;

    public EnemySpawner(
        IEnemyFactory factory,
        Transform center,
        EnemyWaveSettings settings,
        float minSpawnRadius,
        float maxSpawnRadius)
    {
        this.factory = factory;
        this.center = center;
        this.settings = settings;
        this.minSpawnRadius = minSpawnRadius;
        this.maxSpawnRadius = maxSpawnRadius;
        state = State.Spawning;
    }

    public void Tick(float deltaTime)
    {
        if (state == State.Completed)
            return;

        if (state == State.Break)
        {
            TickBreak(deltaTime);
            return;
        }

        var wave = settings.Waves[waveIndex];
        waveTimer += deltaTime;

        if (waveTimer >= wave.Duration)
        {
            if (!wave.WaitForEnemiesCleared || factory.ActiveCount == 0)
                BeginBreak(wave.BreakDuration);
            return;
        }

        spawnTimer += deltaTime;
        while (spawnTimer >= wave.SpawnInterval && factory.ActiveCount < wave.MaxAliveEnemies)
        {
            spawnTimer -= wave.SpawnInterval;
            var availableSlots = wave.MaxAliveEnemies - factory.ActiveCount;
            var spawnCount = Mathf.Min(wave.EnemiesPerSpawn, availableSlots);
            for (var i = 0; i < spawnCount; i++)
                factory.Create(GetSpawnPosition());
        }
    }

    private Vector3 GetSpawnPosition()
    {
        var direction = Random.insideUnitCircle.normalized;
        var distance = Random.Range(minSpawnRadius, maxSpawnRadius);
        return center.position + (Vector3)(direction * distance);
    }

    private void BeginBreak(float duration)
    {
        state = State.Break;
        breakTimer = duration;
    }

    private void TickBreak(float deltaTime)
    {
        breakTimer -= deltaTime;
        if (breakTimer > 0f)
            return;

        if (waveIndex < settings.Waves.Length - 1)
            waveIndex++;
        else if (!settings.LoopLastWave)
        {
            state = State.Completed;
            return;
        }

        waveTimer = 0f;
        spawnTimer = 0f;
        state = State.Spawning;
    }

    private enum State
    {
        Spawning,
        Break,
        Completed
    }
}
