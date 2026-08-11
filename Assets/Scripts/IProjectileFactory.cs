using UnityEngine;

public interface IProjectileFactory
{
    Projectile Create(
        Vector3 position,
        Vector2 direction,
        float speed,
        ProjectileShotData shotData);

    Projectile Create(
        Projectile prefab,
        Vector3 position,
        Vector2 direction,
        float speed,
        ProjectileShotData shotData);
}
