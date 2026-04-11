using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectileLifetimeScope : LifetimeScope
{
    [Header("Global Stage Datas")]
    [SerializeField] private List<StageConfig> allStages;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance<IReadOnlyList<StageConfig>>(allStages);
        builder.Register<GameManagerModel>(Lifetime.Singleton);
    }
}
