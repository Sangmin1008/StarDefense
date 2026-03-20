using UnityEngine;
using VContainer;
using VContainer.Unity;

public class InGameLifetimeScope : LifetimeScope
{
    [Header("Configurations")]
    [SerializeField] private EnemyConfig normalEnemyConfig;
    [SerializeField] private StageConfig stageConfig;

    [Header("Prefabs")]
    [SerializeField] private EnemyView enemyViewPrefab;
    [SerializeField] private CommanderView commanderViewPrefab;
    
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(normalEnemyConfig);
        builder.RegisterInstance(stageConfig);
        
        builder.RegisterInstance(enemyViewPrefab);
        builder.RegisterInstance(commanderViewPrefab);
        
        builder.RegisterEntryPoint<WavePresenter>();
        builder.Register<WaveModel>(Lifetime.Scoped);
        builder.Register<CommanderModel>(Lifetime.Scoped).WithParameter(stageConfig.CommanderConfig);
        builder.Register<EnemyRegistry>(Lifetime.Scoped);
        builder.RegisterEntryPoint<EnemySpawner>().AsSelf();
        builder.RegisterEntryPoint<StageInitializer>();
    }
}
