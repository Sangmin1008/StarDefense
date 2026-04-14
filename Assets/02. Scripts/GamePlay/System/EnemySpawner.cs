using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

public class EnemySpawner : IInitializable, IDisposable
{
    private readonly StageConfig _stageConfig;
    private readonly WaveModel _waveModel;
    private readonly CommanderModel _commanderModel;
    
    private readonly EnemyView _enemyView;
    private readonly EnemyRegistry _registry;
    private readonly CoinModel _coinModel;
    
    private ObjectPool<EnemyPresenter> _enemyPool;
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    private CancellationTokenSource _cts;
    

    public EnemySpawner(StageConfig stageConfig, WaveModel waveModel, CommanderModel commanderModel, EnemyView enemyView, EnemyRegistry registry, CoinModel coinModel)
    {
        _stageConfig = stageConfig;
        _waveModel = waveModel;
        _commanderModel = commanderModel;
        
        _enemyView = enemyView;
        _registry = registry;
        _coinModel = coinModel;
    }
    
    public void Initialize()
    {
        InitializePool();
        _cts = new CancellationTokenSource();
        WaveRoutineAsync(_cts.Token).Forget();
    }

    private void InitializePool()
    {
        _enemyPool = new ObjectPool<EnemyPresenter>(
            createFunc: () => CreatePresenter(),
            actionOnGet: p => p.View.gameObject.SetActive(true),
            actionOnRelease: p => 
            {
                p.View.gameObject.SetActive(false);
                p.Release();
                _registry.Unregister(p.Model);
            },
            actionOnDestroy: p => 
            {
                p.Dispose();
                if (p.View) UnityEngine.Object.Destroy(p.View.gameObject);
            },
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    private EnemyPresenter CreatePresenter()
    {
        EnemyView view = UnityEngine.Object.Instantiate(_enemyView);
        EnemyModel model = new EnemyModel();
        EnemyPresenter presenter = new EnemyPresenter(model, view);

        presenter.OnDeath
            .Subscribe(p => 
            {
                _waveModel.AliveEnemiesCount.Value--;
                _coinModel.AddCoin(p.Model.Config.Reward);
                _enemyPool.Release(p);
            })
            .AddTo(_disposables);

        presenter.OnEscaped
            .Subscribe(p =>
            {
                _waveModel.AliveEnemiesCount.Value--;
                _commanderModel.TakeDamage(p.Model.Config.AttackPower);
                _enemyPool.Release(p);
            })
            .AddTo(_disposables);

        return presenter;
    }
    

    
    
    private async UniTaskVoid WaveRoutineAsync(CancellationToken cancellationToken)
    {
        try 
        {
            for (int i = 0; i < _stageConfig.Waves.Count; i++)
            {
                if (_waveModel.IsGameOver.Value) return;

                _waveModel.CurrentWaveIndex.Value = i;
                WaveConfig currentWave = _stageConfig.Waves[i];
                
                _waveModel.TotalEnemiesInCurrentWave.Value = currentWave.SpawnCount;
                _waveModel.AliveEnemiesCount.Value = currentWave.SpawnCount;

                int delaySeconds = Mathf.CeilToInt(currentWave.DelayTimeBeforeWave);
                for (int time = delaySeconds; time > 0; time--)
                {
                    if (_waveModel.IsGameOver.Value) return;
                    
                    _waveModel.NextWaveDelayCountdown.Value = time; 
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                }
                
                _waveModel.NextWaveDelayCountdown.Value = 0;

                for (int j = 0; j < currentWave.SpawnCount; j++)
                {
                    if (_waveModel.IsGameOver.Value) return;
                    
                    SpawnEnemy(currentWave.EnemyType, currentWave.PathData);
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(currentWave.SpawnInterval), cancellationToken: cancellationToken);
                }

                await UniTask.WaitUntil(() => _waveModel.AliveEnemiesCount.Value == 0 || _waveModel.IsGameOver.Value, cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
#if UNITY_EDITOR
            Debug.Log("게임 오버돼서 스폰 종료");
#endif
        }
    }
    
    
    private void SpawnEnemy(EnemyConfig enemyConfig, PathDataSO pathData)
    {
        EnemyPresenter presenter = _enemyPool.Get();
        
        _registry.Register(presenter.Model, presenter.View);

        presenter.ResetAndStart(enemyConfig, pathData.PathPositions);
    }


    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _disposables.Dispose();
        _enemyPool?.Dispose();
    }
}
