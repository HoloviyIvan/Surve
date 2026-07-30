public readonly struct ProjectileShotData
{
    public ProjectileShotData(
        float damage,
        CombatTeam sourceTeam,
        int pierceCount = 0,
        float explosionRadius = 0f)
    {
        Damage = damage;
        SourceTeam = sourceTeam;
        PierceCount = pierceCount;
        ExplosionRadius = explosionRadius;
    }

    public float Damage { get; }
    public CombatTeam SourceTeam { get; }
    public int PierceCount { get; }
    public float ExplosionRadius { get; }
}
