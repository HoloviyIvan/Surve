using UnityEngine;

public sealed class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private PrefabProvider prefabProvider;
    [SerializeField] private CameraFollower cameraFollower;

    [Header("Player spawning")]
    [SerializeField] private Vector3 playerSpawnPosition;

    [Header("Enemy waves")]
    [SerializeField] private EnemyWaveSettings enemyWaveSettings;
    [SerializeField, Min(0f)] private float minSpawnRadius = 6f;
    [SerializeField, Min(0.1f)] private float maxSpawnRadius = 10f;

    [Header("Enemy pool")]
    [SerializeField, Min(0)] private int initialEnemyPoolSize = 10;
    [SerializeField, Min(1)] private int maxEnemyPoolSize = 50;

    [Header("Projectile pool")]
    [SerializeField, Min(0)] private int initialProjectilePoolSize = 20;
    [SerializeField, Min(1)] private int maxProjectilePoolSize = 100;

    private EnemyFactory enemyFactory;
    private ProjectileFactory projectileFactory;
    private EnemySpawner enemySpawner;

    private void Awake()
    {
        ValidateSettings();
        var player = Instantiate(prefabProvider.GetPlayer(), playerSpawnPosition, Quaternion.identity);
        player.gameObject.SetActive(false);
        cameraFollower.Initialize(player.transform);

        var enemiesContainer = new GameObject("Enemies").transform;
        var projectilesContainer = new GameObject("Projectiles").transform;

        enemyFactory = new EnemyFactory(
            prefabProvider.GetEnemy(),
            player.transform,
            enemiesContainer,
            initialEnemyPoolSize,
            maxEnemyPoolSize);

        projectileFactory = new ProjectileFactory(
            prefabProvider.GetProjectile(),
            projectilesContainer,
            initialProjectilePoolSize,
            maxProjectilePoolSize);

        var weaponController = player.GetComponent<WeaponController>();
        if (weaponController == null)
            throw new MissingComponentException("Player prefab must contain a WeaponController.");

        weaponController.Initialize(enemyFactory.FindClosest, projectileFactory);
        var startingWeapons = prefabProvider.GetStartingWeapons();
        for (var i = 0; i < startingWeapons.Length; i++)
            weaponController.Equip(startingWeapons[i]);

        enemySpawner = new EnemySpawner(
            enemyFactory,
            player.transform,
            enemyWaveSettings,
            minSpawnRadius,
            maxSpawnRadius);

        player.gameObject.SetActive(true);
    }

    private void Update()
    {
        enemySpawner.Tick(Time.deltaTime);
    }

    private void OnDestroy()
    {
        projectileFactory?.Dispose();
        enemyFactory?.Dispose();
    }

    private void ValidateSettings()
    {
        if (prefabProvider == null)
            throw new MissingReferenceException("GameEntryPoint: Prefab Provider is not assigned.");

        if (cameraFollower == null)
            throw new MissingReferenceException("GameEntryPoint: Camera Follower is not assigned.");

        prefabProvider.Validate();

        if (enemyWaveSettings == null)
            throw new MissingReferenceException("GameEntryPoint: Enemy Wave Settings are not assigned.");

        enemyWaveSettings.Validate();

        if (maxSpawnRadius < minSpawnRadius)
            maxSpawnRadius = minSpawnRadius;

        if (maxEnemyPoolSize < initialEnemyPoolSize)
            maxEnemyPoolSize = initialEnemyPoolSize;

        if (maxProjectilePoolSize < initialProjectilePoolSize)
            maxProjectilePoolSize = initialProjectilePoolSize;
    }
}
