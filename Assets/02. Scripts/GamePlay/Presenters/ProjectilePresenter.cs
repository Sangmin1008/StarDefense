using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class ProjectilePresenter : IInitializable, IDisposable
{
    private readonly ProjectileModel _model;
    private readonly ProjectileView _view;
    private readonly EnemyRegistry _registry;
    
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    public event Action<ProjectilePresenter> OnComplete;
    public ProjectileView View => _view;
    
    public ProjectilePresenter(ProjectileModel model, ProjectileView view, EnemyRegistry registry)
    {
        _model = model;
        _view = view;
        _registry = registry;
    }
    
    public void Initialize() { }
    
    public void ResetDataAndFire(Vector3 startPos, int damage, float speed, float maxDist, EnemyModel targetModel, GameObject targetObj)
    {
        Release();
        
        _view.transform.position = startPos;
        _model.UpdateData(damage, speed, maxDist, targetModel, targetObj);

        if (_model.TargetView != null)
        {
            _model.CurrentDirection = (_model.TargetView.transform.position - _view.transform.position).normalized;
        }
        
        _view.OnHitEnemy += HandleHitEnemy;
        
        Observable.EveryUpdate()
            .TakeUntilDestroy(_view)
            .Subscribe(_ => UpdateMovement())
            .AddTo(_disposables);
    }
    
    private void UpdateMovement()
    {
        if (_model.TargetModel != null && !_model.TargetModel.IsDead.Value && _model.TargetView != null)
        {
            _model.CurrentDirection = (_model.TargetView.transform.position - _view.transform.position).normalized;
        }

        _view.Move(_model.CurrentDirection, _model.Speed);

        _model.TraveledDistance += _model.Speed * Time.deltaTime;
        if (_model.TraveledDistance >= _model.MaxDistance)
        {
            Complete();
        }
    }

    private void HandleHitEnemy(EnemyView hitView)
    {
        if (_registry.TryGetModel(hitView, out EnemyModel hitEnemyModel))
        {
            if (hitEnemyModel.IsDead.Value) return;

            hitEnemyModel.TakeDamage(_model.Damage);
            Complete();
        }
    }

    private void Complete()
    {
        OnComplete?.Invoke(this);
    }
    
    public void Release()
    {
        if (_view != null) _view.OnHitEnemy -= HandleHitEnemy;
        _disposables.Clear();
    }

    public void Dispose()
    {
        if (_view != null) _view.OnHitEnemy -= HandleHitEnemy;
        _disposables.Dispose();
    }
}
