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
    [SerializeField] private CommanderUIView commanderUIView;
    [SerializeField] private WaveUIView waveUIView;
    [SerializeField] private CoinUIView coinUIView;
    [SerializeField] private GridPopupUIView gridPopupUIView;
    [SerializeField] private GameResultUIView gameResultUIView;
    
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<StageConfig>(resolver => 
        {
            var gameManager = resolver.Resolve<GameManagerModel>();
            return gameManager.CurrentStageConfig;
        }, Lifetime.Scoped);
        
        builder.RegisterInstance(heroConfigs);
        builder.RegisterInstance(enemyViewPrefab);
        builder.RegisterInstance(commanderViewPrefab);
        builder.Register<WaveModel>(Lifetime.Scoped);
        builder.Register<CommanderModel>(Lifetime.Scoped);
        builder.Register<EnemyRegistry>(Lifetime.Scoped);
        builder.Register<GridModel>(Lifetime.Scoped);
        builder.Register<HeroSpawner>(Lifetime.Scoped);
        builder.Register<CoinModel>(Lifetime.Scoped);
        builder.Register<ProjectileSpawner>(Lifetime.Scoped);
        builder.RegisterEntryPoint<WavePresenter>();
        builder.RegisterEntryPoint<EnemySpawner>().AsSelf();
        builder.RegisterEntryPoint<StageInitializer>();
        builder.RegisterEntryPoint<CommanderUIPresenter>();
        builder.RegisterEntryPoint<WaveUIPresenter>();
        builder.RegisterEntryPoint<CoinUIPresenter>();
        builder.RegisterEntryPoint<GameResultUIPresenter>();
        builder.RegisterComponent(commanderUIView);
        builder.RegisterComponent(waveUIView);
        builder.RegisterComponent(coinUIView);
        builder.RegisterComponent(gridPopupUIView);
        builder.RegisterComponent(gameResultUIView);
    }
}
