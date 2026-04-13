using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class LobbyPresenter : IInitializable, IDisposable
{
    private readonly LobbyView _lobbyView;
    private readonly GameManagerModel _gameManagerModel;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    public LobbyPresenter(LobbyView lobbyView, GameManagerModel gameManagerModel, IReadOnlyList<StageConfig> stageConfigs)
    {
        _lobbyView = lobbyView;
        _gameManagerModel = gameManagerModel;
    }
    
    public void Initialize()
    {
        _lobbyView.OnStageSelected
            .Subscribe(index => _gameManagerModel.LoadStage(index))
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
