using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EnemyModel
{
    private ReactiveProperty<int> _currentHp;
    public IReadOnlyReactiveProperty<int> CurrentHp => _currentHp;
    
    private ReactiveProperty<bool> _isDead;
    public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
    
    public Subject<Unit> OnEscaped { get; } = new Subject<Unit>();
    
    public EnemyConfig Config { get; private set; }

    public EnemyModel()
    {
        _currentHp = new ReactiveProperty<int>(0);
        _isDead = new ReactiveProperty<bool>(false);
    }
    
    public void ResetData(EnemyConfig newConfig)
    {
        Config = newConfig;
        _currentHp.Value = newConfig.MaxHealth;
        _isDead.Value = false;
    }
    
    public void TakeDamage(int damage)
    {
        if (IsDead.Value) return;
        
        _currentHp.Value = Mathf.Max(0, _currentHp.Value - damage);
        
        if (_currentHp.Value <= 0)
        {
            _isDead.Value = true;
        }
    }
}
