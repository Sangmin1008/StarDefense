using UnityEngine;
using VContainer;
using VContainer.Unity;

public class InGameLifetimeScope : LifetimeScope
{
    [Header("Configurations")]
    [SerializeField] private EnemyConfig normalEnemyConfig;

    [Header("Prefabs")]
    [SerializeField] private EnemyView enemyViewPrefab;
    
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(normalEnemyConfig);
        builder.RegisterInstance(enemyViewPrefab);
        
        builder.Register<EnemyRegistry>(Lifetime.Scoped);
        builder.RegisterEntryPoint<EnemySpawner>().AsSelf();
    }
}
