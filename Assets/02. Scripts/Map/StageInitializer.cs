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
    private readonly GridManager _gridManager;
    private readonly HeroManager _heroManager;
    private readonly GameUIView _gameUIView;
    private readonly CoinModel _coinModel;
    
    private CommanderPresenter _commanderPresenter;
    private GridInteractionPresenter _gridInteractionPresenter;
    private GameObject _currentMapInstance;

    public StageInitializer(
        StageConfig stageConfig, CommanderView commanderView, CommanderModel commanderModel, 
        EnemyRegistry registry, ProjectileManager projectileManager,
        GridManager gridManager, HeroManager heroManager, GameUIView gameUIView, CoinModel coinModel)
    {
        _stageConfig = stageConfig;
        _commanderView = commanderView;
        _commanderModel = commanderModel;
        _registry = registry;
        _projectileManager = projectileManager;
        
        _gridManager = gridManager;
        _heroManager = heroManager;
        _gameUIView = gameUIView;
        _coinModel = coinModel;
    }
    
    public void Initialize()
    {
        if (_stageConfig.TileMapPrefab != null)
        {
            _currentMapInstance = UnityEngine.Object.Instantiate(_stageConfig.TileMapPrefab, Vector3.zero, Quaternion.identity);
            
            var clickDetector = _currentMapInstance.GetComponentInChildren<GridClickDetector>();
            
            _gridInteractionPresenter = new GridInteractionPresenter(_gridManager, clickDetector, _gameUIView, _heroManager, _coinModel);
            _gridInteractionPresenter.Initialize();
            
            Debug.Log("맵 및 그리드 시스템 초기화 완료");
        }
        
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
