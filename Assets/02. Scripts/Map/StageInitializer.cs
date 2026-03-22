using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class StageInitializer : IInitializable, IDisposable
{
    private readonly StageConfig _stageConfig;
    private readonly CommanderView _commanderView;
    private readonly CommanderModel _commanderModel;
    private readonly EnemyRegistry _registry;
    private readonly ProjectileManager _projectileManager;
    
    private CommanderPresenter _commanderPresenter; // 메모리 해제용임

    public StageInitializer(StageConfig stageConfig, CommanderView commanderView, CommanderModel commanderModel, 
        EnemyRegistry registry, ProjectileManager projectileManager)
    {
        _stageConfig = stageConfig;
        _commanderView = commanderView;
        _commanderModel = commanderModel;
        _registry = registry;
        _projectileManager = projectileManager;
    }
    
    public void Initialize()
    {
        CommanderView view = UnityEngine.Object.Instantiate(_commanderView, _stageConfig.CommanderPosition, Quaternion.identity);
        _commanderPresenter = new CommanderPresenter(_commanderModel, view, _registry, _projectileManager);
        _commanderPresenter.Initialize();
        
        Debug.Log("지휘관 소환 완료");
    }
    
    public void Dispose()
    {
        _commanderPresenter.Dispose();
    }
}
