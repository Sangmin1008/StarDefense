using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class InGameLifetimeScope : LifetimeScope
{
    [Header("Configurations")]
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private List<HeroConfig> heroConfigs;

    [Header("Prefabs")]
    [SerializeField] private EnemyView enemyViewPrefab;
    [SerializeField] private CommanderView commanderViewPrefab;
    
    [Header("UI Views")]
    [SerializeField] private GameUIView gameUIView;
    
    [Header("Grid System")]
    [SerializeField] private GridClickDetector gridClickDetector;
    
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(stageConfig);
        builder.RegisterInstance(heroConfigs);
        
        builder.RegisterInstance(enemyViewPrefab);
        builder.RegisterInstance(commanderViewPrefab);
        
        builder.Register<WaveModel>(Lifetime.Scoped);
        builder.Register<CommanderModel>(Lifetime.Scoped).WithParameter(stageConfig.CommanderConfig);
        builder.Register<EnemyRegistry>(Lifetime.Scoped);
        builder.Register<GridManager>(Lifetime.Scoped);
        builder.Register<HeroManager>(Lifetime.Scoped);
        builder.RegisterEntryPoint<WavePresenter>();
        builder.RegisterEntryPoint<EnemySpawner>().AsSelf();
        builder.RegisterEntryPoint<StageInitializer>();
        builder.RegisterEntryPoint<GameUIPresenter>();
        builder.RegisterEntryPoint<GridInteractionPresenter>();
        builder.RegisterComponent(gameUIView);
        builder.RegisterComponent(gridClickDetector);
    }
}
