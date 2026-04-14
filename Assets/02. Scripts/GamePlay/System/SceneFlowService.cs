using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class SceneFlowService : IInitializable, IDisposable
{
    private readonly GameManagerModel _gameManagerModel;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public SceneFlowService(GameManagerModel gameManagerModel)
    {
        _gameManagerModel = gameManagerModel;
    }

    public void Initialize()
    {
        _gameManagerModel.OnRequestLoadGameScene
            .Subscribe(_ => SceneManager.LoadScene(SceneNames.Game))
            .AddTo(_disposables);

        _gameManagerModel.OnRequestLoadLobbyScene
            .Subscribe(_ => SceneManager.LoadScene(SceneNames.Lobby))
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
