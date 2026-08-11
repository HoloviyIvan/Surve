using UnityEngine;

public sealed class PrefabProvider : MonoBehaviour
{
    [SerializeField] private GamePrefabs storage;

    public Player GetPlayer() => storage.Player;

    public Enemy GetEnemy() => storage.Enemy;

    public Projectile GetProjectile() => storage.Projectile;

    public Weapon[] GetStartingWeapons() => storage.StartingWeapons;

    public void Validate()
    {
        if (storage == null)
            throw new MissingReferenceException("PrefabProvider: Game Prefabs storage is not assigned.");

        storage.Validate();
    }
}
