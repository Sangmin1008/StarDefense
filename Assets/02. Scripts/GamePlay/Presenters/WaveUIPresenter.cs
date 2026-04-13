using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class WaveUIPresenter : IInitializable, IDisposable
{
    private readonly WaveModel _waveModel;
    private readonly WaveUIView _uiView;
    private readonly StageConfig _stageConfig;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    public WaveUIPresenter(WaveModel waveModel, WaveUIView uiView, StageConfig stageConfig)
    {
        _waveModel = waveModel;
        _uiView = uiView;
        _stageConfig = stageConfig;
    }
    
    public void Initialize()
    {
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
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
