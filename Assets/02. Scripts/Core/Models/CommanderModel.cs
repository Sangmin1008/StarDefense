using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CommanderModel
{
    private ReactiveProperty<int> _currentHp;
    public IReadOnlyReactiveProperty<int> CurrentHp => _currentHp;
    
    private ReactiveProperty<bool> _isDead;
    public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
    
    public CommanderConfig Config { get; }

    public CommanderModel(CommanderConfig config)
    {
        Config = config;
        _currentHp = new ReactiveProperty<int>(config.MaxHealth);
        _isDead = new ReactiveProperty<bool>(false);
    }
    
    public void TakeDamage(int damage)
    {
        if (_isDead.Value) return;
        
        _currentHp.Value = Mathf.Max(0, _currentHp.Value - damage);
        Debug.Log("현재 체력: " + _currentHp.Value);
        
        if (_currentHp.Value <= 0)
        {
            _isDead.Value = true;
        }
    }
}
