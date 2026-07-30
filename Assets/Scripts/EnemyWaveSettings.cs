using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaves", menuName = "Surve/Enemy Waves")]
public sealed class EnemyWaveSettings : ScriptableObject
{
    [Serializable]
    public sealed class Wave
    {
        [field: SerializeField, Min(1f)] public float Duration { get; private set; } = 20f;
        [field: SerializeField, Min(0.05f)] public float SpawnInterval { get; private set; } = 1f;
        [field: SerializeField, Min(1)] public int EnemiesPerSpawn { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int MaxAliveEnemies { get; private set; } = 20;
        [field: SerializeField, Min(0f)] public float BreakDuration { get; private set; } = 5f;
        [field: SerializeField] public bool WaitForEnemiesCleared { get; private set; } = true;
    }

    [field: SerializeField] public Wave[] Waves { get; private set; }
    [field: SerializeField] public bool LoopLastWave { get; private set; } = true;

    public void Validate()
    {
        if (Waves == null || Waves.Length == 0)
            throw new MissingReferenceException("EnemyWaveSettings: no waves are configured.");

        for (var i = 0; i < Waves.Length; i++)
        {
            if (Waves[i] == null)
                throw new MissingReferenceException($"EnemyWaveSettings: wave at index {i} is missing.");
        }
    }
}
