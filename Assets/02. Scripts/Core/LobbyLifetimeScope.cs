using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LobbyLifetimeScope : LifetimeScope
{
    [Header("View")]
    [SerializeField] private LobbyView lobbyView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(lobbyView);
        builder.RegisterEntryPoint<LobbyPresenter>();
        
    }
}
