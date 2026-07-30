using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected Transform Owner { get; private set; }
    protected Func<Vector3, Enemy> FindClosestEnemy { get; private set; }
    protected IProjectileFactory ProjectileFactory { get; private set; }
    protected CombatTeam OwnerTeam { get; private set; }

    public void Initialize(
        Transform owner,
        CombatTeam ownerTeam,
        Func<Vector3, Enemy> enemyProvider,
        IProjectileFactory projectileFactory)
    {
        Owner = owner;
        OwnerTeam = ownerTeam;
        FindClosestEnemy = enemyProvider;
        ProjectileFactory = projectileFactory;
        OnInitialized();
    }

    public abstract void Tick(float deltaTime);

    protected virtual void OnInitialized()
    {
    }
}
