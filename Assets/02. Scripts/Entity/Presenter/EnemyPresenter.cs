using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPresenter : IInitializable, IDisposable
{
    private readonly EnemyModel _model;
    private readonly EnemyView _view;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public EnemyPresenter(EnemyModel model, EnemyView view)
    {
        _model = model;
        _view = view;
    }
    
    public void Initialize()
    {
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
    }

    public void StartMovement(List<Vector3> waypoints)
    {
        _view.SetupMovement(waypoints, _model.Config.MoveSpeed);
    }

    private void HandleReachedDestination()
    {
        UnityEngine.Object.Destroy(_view.gameObject);
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
}
