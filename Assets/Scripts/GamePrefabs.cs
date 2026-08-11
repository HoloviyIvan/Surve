using UnityEngine;

[CreateAssetMenu(fileName = "GamePrefabs", menuName = "Surve/Game Prefabs")]
public sealed class GamePrefabs : ScriptableObject
{
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public Enemy Enemy { get; private set; }
    [field: SerializeField] public Projectile Projectile { get; private set; }
    [field: SerializeField] public Weapon[] StartingWeapons { get; private set; }

    public void Validate()
    {
        if (Player == null)
            throw new MissingReferenceException("GamePrefabs: Player prefab is not assigned.");

        if (Enemy == null)
            throw new MissingReferenceException("GamePrefabs: Enemy prefab is not assigned.");

        if (Projectile == null)
            throw new MissingReferenceException("GamePrefabs: Projectile prefab is not assigned.");

        if (StartingWeapons == null || StartingWeapons.Length == 0)
            throw new MissingReferenceException("GamePrefabs: no Starting Weapons are assigned.");

        for (var i = 0; i < StartingWeapons.Length; i++)
        {
            if (StartingWeapons[i] == null)
                throw new MissingReferenceException($"GamePrefabs: Starting Weapon at index {i} is not assigned.");
        }
    }
}
