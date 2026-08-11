using UnityEngine;

public readonly struct ProjectileShotData
{
    public ProjectileShotData(
        float damage,
        CombatTeam sourceTeam,
        int pierceCount = 0,
        float explosionRadius = 0f,
        Vector2 targetPosition = default,
        float flightDuration = 0f,
        float arcHeight = 0f)
    {
        Damage = damage;
        SourceTeam = sourceTeam;
        PierceCount = pierceCount;
        ExplosionRadius = explosionRadius;
        TargetPosition = targetPosition;
        FlightDuration = flightDuration;
        ArcHeight = arcHeight;
    }

    public float Damage { get; }
    public CombatTeam SourceTeam { get; }
    public int PierceCount { get; }
    public float ExplosionRadius { get; }
    public Vector2 TargetPosition { get; }
    public float FlightDuration { get; }
    public float ArcHeight { get; }
}
