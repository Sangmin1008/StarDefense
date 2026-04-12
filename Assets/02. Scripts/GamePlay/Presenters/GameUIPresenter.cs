using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class GameUIPresenter : IInitializable, IDisposable
{
    private readonly StageConfig _stageConfig;
    private readonly WaveModel _waveModel;
    private readonly CommanderModel _commanderModel;
    private readonly GameUIView _uiView;
    private readonly GameManagerModel _gameManagerModel;
    private readonly CoinModel _coinModel;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    public GameUIPresenter(StageConfig stageConfig, WaveModel waveModel, CommanderModel commanderModel, GameUIView uiView, GameManagerModel gameManagerModel, CoinModel coinModel)
    {
        _stageConfig = stageConfig;
        _waveModel = waveModel;
        _commanderModel = commanderModel;
        _uiView = uiView;
        _gameManagerModel = gameManagerModel;
        _coinModel = coinModel;
    }
    
    public void Initialize()
    {
        _commanderModel.CurrentHp
            .Subscribe(hp => _uiView.UpdateCommanderHp(hp, _commanderModel.Config.MaxHealth))
            .AddTo(_disposables);

        _waveModel.CurrentWaveIndex
            .Subscribe(index => _uiView.UpdateWaveInfo(index, _stageConfig.Waves.Count))
            .AddTo(_disposables);

        Observable.CombineLatest(
                _waveModel.AliveEnemiesCount,
                _waveModel.TotalEnemiesInCurrentWave,
                (alive, total) => new { alive, total }
            )
            .Subscribe(data => _uiView.UpdateAliveEnemies(data.alive, data.total))
            .AddTo(_disposables);
        
        _waveModel.NextWaveDelayCountdown
            .Subscribe(time => _uiView.UpdateDelayCountdown(time))
            .AddTo(_disposables);

        _waveModel.IsDefeat
            .Where(isDefeat => isDefeat)
            .Subscribe(_ => _uiView.ShowDefeatScreen())
            .AddTo(_disposables);

        _waveModel.IsVictory
            .Where(isVictory => isVictory)
            .Subscribe(_ => _uiView.ShowVictoryScreen())
            .AddTo(_disposables);
        
        _uiView.OnRetryClicked
            .Subscribe(_ =>
            {
                Debug.Log("스테이지 재시작!");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
            })
            .AddTo(_disposables);

        _uiView.OnNextClicked
            .Subscribe(_ =>
            {
                Debug.Log("다음 스테이지!");
                
                if (_gameManagerModel.HasNextStage())
                {
                    _gameManagerModel.SetNextStage();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else
                {
                    SceneManager.LoadScene("01. Scenes/LobbyScene");
                }
            })
            .AddTo(_disposables);

        _uiView.OnExitClicked
            .Subscribe(_ =>
            {
                Debug.Log("로비로 돌아가기!");
                SceneManager.LoadScene("01. Scenes/LobbyScene");
            })
            .AddTo(_disposables);
        
            _coinModel.CurrentCoin
                .Subscribe(amount => _uiView.UpdateCoin(amount))
                .AddTo(_disposables);
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
}
