using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry
{
    private readonly List<EnemyModel> _activeEnemies;

    public void Register(EnemyModel enemy)
    {
        
    }

    public void Unregister(EnemyModel enemy)
    {
        
    }

    public EnemyModel GetClosestEnemyInRange(Vector3 centerPos, float radius)
    {
        return new EnemyModel(new EnemyConfig()); // TEST용
    }
}
