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
    
    [Header("Canvas Reference")]
    [SerializeField] private Transform mainCanvasTransform;
    
    
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
        
        builder.Register<WaveModel>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        builder.Register<CommanderModel>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        builder.Register<EnemyRegistry>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        builder.Register<GridModel>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        builder.Register<CoinModel>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        
        builder.Register<HeroSpawner>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        builder.Register<ProjectileSpawner>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

        builder.RegisterEntryPoint<WavePresenter>();
        builder.RegisterEntryPoint<EnemySpawner>().AsSelf(); 
        builder.RegisterEntryPoint<StageInitializer>();
        builder.RegisterEntryPoint<CommanderUIPresenter>();
        builder.RegisterEntryPoint<WaveUIPresenter>();
        builder.RegisterEntryPoint<CoinUIPresenter>();
        builder.RegisterEntryPoint<GameResultUIPresenter>();

        builder.RegisterComponentInNewPrefab(commanderUIView, Lifetime.Scoped).UnderTransform(mainCanvasTransform);
        builder.RegisterComponentInNewPrefab(waveUIView, Lifetime.Scoped).UnderTransform(mainCanvasTransform);
        builder.RegisterComponentInNewPrefab(coinUIView, Lifetime.Scoped).UnderTransform(mainCanvasTransform);
        builder.RegisterComponentInNewPrefab(gridPopupUIView, Lifetime.Scoped).UnderTransform(mainCanvasTransform);
        builder.RegisterComponentInNewPrefab(gameResultUIView, Lifetime.Scoped).UnderTransform(mainCanvasTransform);
    }
}
