using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using VContainer.Unity;
using UnityEngine;
using Unit = UniRx.Unit;

public class EnemyPresenter : IInitializable, IDisposable
{
    private readonly EnemyModel _model;
    private readonly EnemyView _view;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    public event Action<EnemyPresenter> OnDeath;
    public event Action<EnemyPresenter> OnEscapedEvent;
    
    public EnemyView View => _view;
    public EnemyModel Model => _model;

    public EnemyPresenter(EnemyModel model, EnemyView view)
    {
        _model = model;
        _view = view;
    }
    
    public void Initialize() { }
    
    public void ResetAndStart(EnemyConfig config, List<Vector3> waypoints)
    {
        Release();
        
        _model.ResetData(config);
        _view.SetupMovement(waypoints, config.MoveSpeed);

        _model.CurrentHp
            .Subscribe(hp =>
            {
                float normalizedHp = (float)hp / _model.Config.MaxHealth;
                _view.UpdateHpBar(normalizedHp);
            })
            .AddTo(_disposables);
        
        _view.OnReachedDestination
            .Subscribe(_ => HandleReachedDestination())
            .AddTo(_disposables);

        _model.IsDead
            .Where(isDead => isDead)
            .Subscribe(_ => HandleDeath())
            .AddTo(_disposables);
    }

    private void HandleDeath()
    {
        OnDeath?.Invoke(this);
    }

    private void HandleReachedDestination()
    {
        _model.OnEscaped.OnNext(Unit.Default);
        OnEscapedEvent?.Invoke(this);
    }
    
    public void Release()
    {
        _disposables.Clear();
    }

    public void Dispose()
    {
        Release();
        _disposables.Dispose();
    }
}
