using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class GameResultUIPresenter : IInitializable, IDisposable
{
    private readonly WaveModel _waveModel;
    private readonly GameResultUIView _uiView;
    private readonly GameManagerModel _gameManagerModel;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    public GameResultUIPresenter(WaveModel waveModel, GameResultUIView gameResultUIView, GameManagerModel gameManagerModel)
    {
        _waveModel = waveModel;
        _uiView = gameResultUIView;
        _gameManagerModel = gameManagerModel;
    }

    public void Initialize()
    {
        _waveModel.IsDefeat
            .Where(isDefeat => isDefeat)
            .Subscribe(_ => _uiView.ShowDefeatScreen())
            .AddTo(_disposables);

        _waveModel.IsVictory
            .Where(isVictory => isVictory)
            .Subscribe(_ => _uiView.ShowVictoryScreen())
            .AddTo(_disposables);
        
        _uiView.OnRetryClicked
            .Subscribe(_ => _gameManagerModel.LoadStage(_gameManagerModel.CurrentStageIndex))
            .AddTo(_disposables);

        _uiView.OnNextClicked
            .Subscribe(_ => _gameManagerModel.LoadNextStage())
            .AddTo(_disposables);

        _uiView.OnExitClicked
            .Subscribe(_ => _gameManagerModel.LoadLobby())
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
