using UnityEngine;

public sealed class GameEntryPoint : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Enemy enemyPrefab;

    [Header("Scene")]
    [SerializeField] private Player player;

    [Header("Enemy spawning")]
    [SerializeField, Min(0.05f)] private float spawnInterval = 1f;
    [SerializeField, Min(0f)] private float minSpawnRadius = 6f;
    [SerializeField, Min(0.1f)] private float maxSpawnRadius = 10f;
    [SerializeField, Min(1)] private int maxAliveEnemies = 30;

    [Header("Pool")]
    [SerializeField, Min(0)] private int initialPoolSize = 10;
    [SerializeField, Min(1)] private int maxPoolSize = 50;

    [Header("Scene migration")]
    [SerializeField] private bool removeEnemiesPlacedOnScene = true;

    private EnemyFactory enemyFactory;
    private EnemySpawner enemySpawner;

    private void Awake()
    {
        ValidateSettings();

        if (removeEnemiesPlacedOnScene)
            RemoveSceneEnemies();

        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (player == null)
            throw new MissingReferenceException("GameEntryPoint: Player was not found on the scene.");

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

    private void RemoveSceneEnemies()
    {
        var sceneEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var enemy in sceneEnemies)
            Destroy(enemy.gameObject);
    }

    private void ValidateSettings()
    {
        if (enemyPrefab == null)
            throw new MissingReferenceException("GameEntryPoint: Enemy Prefab is not assigned.");

        if (maxSpawnRadius < minSpawnRadius)
            maxSpawnRadius = minSpawnRadius;

        if (maxPoolSize < initialPoolSize)
            maxPoolSize = initialPoolSize;
    }
}
