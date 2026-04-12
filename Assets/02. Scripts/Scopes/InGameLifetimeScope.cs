using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class InGameLifetimeScope : LifetimeScope
{
    [Header("Configurations")]
    [SerializeField] private List<HeroConfig> heroConfigs;

    [Header("Prefabs")]
    [SerializeField] private EnemyView enemyViewPrefab;
    [SerializeField] private CommanderView commanderViewPrefab;
    
    [Header("UI Views")]
    [SerializeField] private GameUIView gameUIView;
    
    
    protected override void Configure(IContainerBuilder builder)
    {
        var gameManagerModel = Parent.Container.Resolve<GameManagerModel>();
        StageConfig currentStageConfig = gameManagerModel.CurrentStageConfig;
        
        builder.RegisterInstance(currentStageConfig);
        builder.RegisterInstance(heroConfigs);
        
        builder.RegisterInstance(enemyViewPrefab);
        builder.RegisterInstance(commanderViewPrefab);
        
        builder.Register<WaveModel>(Lifetime.Scoped);
        builder.Register<CommanderModel>(Lifetime.Scoped).WithParameter(currentStageConfig.CommanderConfig);
        builder.Register<EnemyRegistry>(Lifetime.Scoped);
        builder.Register<GridModel>(Lifetime.Scoped);
        builder.Register<HeroSpawner>(Lifetime.Scoped);
        builder.Register<CoinModel>(Lifetime.Scoped).WithParameter(currentStageConfig.InitialCoin);
        builder.Register<ProjectileManager>(Lifetime.Scoped);
        builder.RegisterEntryPoint<WavePresenter>();
        builder.RegisterEntryPoint<EnemySpawner>().AsSelf();
        builder.RegisterEntryPoint<StageInitializer>();
        builder.RegisterEntryPoint<GameUIPresenter>();
        builder.RegisterComponent(gameUIView);
    }
}
