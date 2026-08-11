using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class WeaponController : MonoBehaviour
{
    [SerializeField] private DamageReceiver ownerDamageReceiver;

    private readonly List<Weapon> weapons = new();

    private Func<Vector3, Enemy> findClosestEnemy;
    private IProjectileFactory projectileFactory;
    private bool isInitialized;

    public void Initialize(
        Func<Vector3, Enemy> enemyProvider,
        IProjectileFactory newProjectileFactory)
    {
        findClosestEnemy = enemyProvider ?? throw new ArgumentNullException(nameof(enemyProvider));
        projectileFactory = newProjectileFactory ?? throw new ArgumentNullException(nameof(newProjectileFactory));

        if (ownerDamageReceiver == null)
            throw new MissingReferenceException("WeaponController: Owner Damage Receiver is not assigned.");

        isInitialized = true;
    }

    public Weapon Equip(Weapon weaponPrefab)
    {
        if (!isInitialized)
            throw new InvalidOperationException("WeaponController must be initialized before equipping a weapon.");

        if (weaponPrefab == null)
            throw new ArgumentNullException(nameof(weaponPrefab));

        var weapon = Object.Instantiate(weaponPrefab, transform);
        weapon.name = weaponPrefab.name;
        weapon.Initialize(
            transform,
            ownerDamageReceiver.Team,
            findClosestEnemy,
            projectileFactory);
        weapons.Add(weapon);
        return weapon;
    }

    private void Update()
    {
        for (var i = 0; i < weapons.Count; i++)
            weapons[i].Tick(Time.deltaTime);
    }
}
