using UnityEngine;

public sealed class OrbitingWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 5f;
    [SerializeField, Min(0.05f)] private float damageInterval = 0.35f;
    [SerializeField, Min(1)] private int bladeCount = 3;
    [SerializeField, Min(0f)] private float orbitRadius = 2f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private OrbitingBlade bladePrefab;

    private OrbitingBlade[] blades;
    private float angle;

    protected override void OnInitialized()
    {
        if (bladePrefab == null)
            throw new MissingReferenceException("OrbitingWeapon: Blade Prefab is not assigned.");

        blades = new OrbitingBlade[bladeCount];
        for (var i = 0; i < blades.Length; i++)
        {
            var blade = Instantiate(bladePrefab, Owner.position, Quaternion.identity);
            blade.name = $"{bladePrefab.name}_{i + 1}";
            blade.transform.SetParent(transform, true);
            blade.Initialize(damage, damageInterval, OwnerTeam);
            blades[i] = blade;
        }

        UpdateBladePositions(0f);
    }

    public override void Tick(float deltaTime)
    {
        angle = (angle + rotationSpeed * deltaTime) % 360f;
        UpdateBladePositions(deltaTime);
    }

    private void UpdateBladePositions(float deltaTime)
    {
        if (blades == null)
            return;

        var angleStep = 360f / blades.Length;
        for (var i = 0; i < blades.Length; i++)
        {
            if (blades[i] == null)
                continue;

            var radians = (angle + angleStep * i) * Mathf.Deg2Rad;
            var offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * orbitRadius;
            blades[i].MoveAndRotate((Vector2)Owner.position + offset, deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, orbitRadius);
    }
}
