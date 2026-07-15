using UnityEngine;

public interface IEnemyFactory
{
    int ActiveCount { get; }
    Enemy Create(Vector3 position);
    Enemy FindClosest(Vector3 position);
}
