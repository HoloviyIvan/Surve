using UnityEngine;

public sealed class GameEntryPoint : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;

    [Header("Player spawning")]
    [SerializeField] private Vector3 playerSpawnPosition;

    [Header("Enemy spawning")]
    [SerializeField, Min(0.05f)] private float spawnInterval = 1f;
    [SerializeField, Min(0f)] private float minSpawnRadius = 6f;
    [SerializeField, Min(0.1f)] private float maxSpawnRadius = 10f;
    [SerializeField, Min(1)] private int maxAliveEnemies = 30;

    [Header("Pool")]
    [SerializeField, Min(0)] private int initialPoolSize = 10;
    [SerializeField, Min(1)] private int maxPoolSize = 50;

    private EnemyFactory enemyFactory;
    private EnemySpawner enemySpawner;

    private void Awake()
    {
        ValidateSettings();

        var player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        var enemiesContainer = new GameObject("Enemies").transform;

        enemyFactory = new EnemyFactory(
            enemyPrefab,
            player.transform,
            enemiesContainer,
            initialPoolSize,
            maxPoolSize);

        player.Initialize(enemyFactory.FindClosest);

        enemySpawner = new EnemySpawner(
            enemyFactory,
            player.transform,
            spawnInterval,
            minSpawnRadius,
            maxSpawnRadius,
            maxAliveEnemies);
    }

    private void Update()
    {
        enemySpawner.Tick(Time.deltaTime);
    }

    private void OnDestroy()
    {
        enemyFactory?.Dispose();
    }

    private void ValidateSettings()
    {
        if (playerPrefab == null)
            throw new MissingReferenceException("GameEntryPoint: Player Prefab is not assigned.");

        if (enemyPrefab == null)
            throw new MissingReferenceException("GameEntryPoint: Enemy Prefab is not assigned.");

        if (maxSpawnRadius < minSpawnRadius)
            maxSpawnRadius = minSpawnRadius;

        if (maxPoolSize < initialPoolSize)
            maxPoolSize = initialPoolSize;
    }
}
