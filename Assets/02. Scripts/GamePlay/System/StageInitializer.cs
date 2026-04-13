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
    private readonly ProjectileSpawner _projectileSpawner;
    private readonly GridModel _gridModel;
    private readonly HeroSpawner _heroSpawner;
    private readonly GameUIView _gameUIView;
    private readonly CoinModel _coinModel;
    
    private CommanderPresenter _commanderPresenter;
    private GridInteractionPresenter _gridInteractionPresenter;
    private GameObject _currentMapInstance;

    public StageInitializer(
        StageConfig stageConfig, CommanderView commanderView, CommanderModel commanderModel, 
        EnemyRegistry registry, ProjectileSpawner projectileSpawner,
        GridModel gridModel, HeroSpawner heroSpawner, GameUIView gameUIView, CoinModel coinModel)
    {
        _stageConfig = stageConfig;
        _commanderView = commanderView;
        _commanderModel = commanderModel;
        _registry = registry;
        _projectileSpawner = projectileSpawner;
        
        _gridModel = gridModel;
        _heroSpawner = heroSpawner;
        _gameUIView = gameUIView;
        _coinModel = coinModel;
    }
    
    public void Initialize()
    {
        if (_stageConfig.TileMapPrefab != null)
        {
            _currentMapInstance = UnityEngine.Object.Instantiate(_stageConfig.TileMapPrefab, Vector3.zero, Quaternion.identity);
            
            var clickDetector = _currentMapInstance.GetComponentInChildren<GridClickDetector>();
            if (clickDetector == null)
            {
                Debug.LogError("타일맵 프리팹에 GridClickDetector를 못 찾음!!!");
                return;
            }
            
            _gridInteractionPresenter = new GridInteractionPresenter(_gridModel, _gameUIView, _heroSpawner, _coinModel, clickDetector);
            _gridInteractionPresenter.Initialize();
            
            foreach (var brokenPos in clickDetector.GetBrokenCells())
            {
                _gridModel.RegisterBrokenCell(brokenPos);
            }
            
            Debug.Log("맵 및 그리드 시스템 초기화 완료");
        }
        
        CommanderView view = UnityEngine.Object.Instantiate(_commanderView, _stageConfig.CommanderPosition, Quaternion.identity);
        _commanderPresenter = new CommanderPresenter(_commanderModel, view, _registry, _projectileSpawner);
        _commanderPresenter.Initialize();
        
        Debug.Log("지휘관 소환 완료");
    }
    
    public void Dispose()
    {
        _gridInteractionPresenter?.Dispose();
        _commanderPresenter?.Dispose();

        if (_currentMapInstance != null)
        {
            UnityEngine.Object.Destroy(_currentMapInstance);
            _currentMapInstance = null;
        }
    }
}
