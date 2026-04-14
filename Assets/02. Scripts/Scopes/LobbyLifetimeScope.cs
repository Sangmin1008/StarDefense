using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LobbyLifetimeScope : LifetimeScope
{
    [Header("View")]
    [SerializeField] private LobbyView lobbyView;
    
    [Header("Canvas Reference")]
    [SerializeField] private Transform mainCanvasTransform;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<LobbyPresenter>();
        
        builder.RegisterComponentInNewPrefab(lobbyView, Lifetime.Scoped).UnderTransform(mainCanvasTransform);
    }
}
