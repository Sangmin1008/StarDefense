using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class GridInteractionPresenter : IInitializable, IDisposable
{
    private readonly GridModel _gridModel;
    private readonly HeroSpawner _heroSpawner;
    private readonly GameUIView _uiView;
    private readonly CoinModel _coinModel;
    private readonly GridClickDetector _clickDetector;
    
    private Vector3Int _selectedCellPos;
    private Vector3 _selectedWorldPos;
    private HeroModel _selectedModel;
    
    private CompositeDisposable _disposables = new CompositeDisposable();
    

    public GridInteractionPresenter(GridModel gridModel, GameUIView uiView, HeroSpawner heroSpawner, CoinModel coinModel, GridClickDetector clickDetector)
    {
        _gridModel = gridModel;
        _uiView = uiView;
        _heroSpawner = heroSpawner;
        _coinModel = coinModel;
        _clickDetector = clickDetector;
    }
    
    public void Initialize()
    {
        _clickDetector.OnGridClicked += HandleGridClicked;
        
        _uiView.OnSummonClicked
            .Subscribe(_ => HandleSummonRequest())
            .AddTo(_disposables);
        
        _uiView.OnUpgradeClicked
            .Subscribe(_ => HandleUpgradeRequest())
            .AddTo(_disposables);
        
        _uiView.OnRepairClicked
            .Subscribe(_ => HandleRepairRequest())
            .AddTo(_disposables);
    }

    private void HandleGridClicked(Vector3Int cellPos, Vector3 worldPos)
    {
        _selectedCellPos = cellPos;
        _selectedWorldPos = worldPos;

        if (_gridModel.IsBroken(cellPos))
        {
            int cost = GameConstants.GridRepairCost;
            _uiView.ShowRepairGridPopup(worldPos, cost);
        }
        else if (_gridModel.IsEmpty(cellPos))
        {
            _selectedModel = null;
            int cost = HeroCostHelper.GetCost(HeroGrade.Normal);
            _uiView.ShowGridPopup(worldPos, isSummon: true, cost);
        }
        else
        {
            _selectedModel = _gridModel.GetHero(cellPos);
            if (_selectedModel == null ||_selectedModel.Config.Grade == HeroGrade.Legendary) return;
            
            int cost = HeroCostHelper.GetCost(_selectedModel.Config.Grade);
            _uiView.ShowGridPopup(worldPos, isSummon: false, cost);
        }
    }
    
    private void HandleSummonRequest()
    {
        _heroSpawner.TrySpawnHero(HeroGrade.Normal, _selectedCellPos, _selectedWorldPos);
        _uiView.HideGridPopup();
    }

    private void HandleUpgradeRequest()
    {
        if (_selectedModel != null)
        {
            _heroSpawner.TryUpgradeHero(_selectedModel); 
        }
        _uiView.HideGridPopup();
    }

    private void HandleRepairRequest()
    {
        int repairCost = GameConstants.GridRepairCost;
        if (_coinModel.TrySpendCoin(repairCost))
        {
            _gridModel.RepairCell(_selectedCellPos);
            _clickDetector.ChangeToNormalTile(_selectedCellPos);
        }
        _uiView.HideGridPopup();
    }

    public void Dispose()
    {
        if (_clickDetector != null)
        {
            _clickDetector.OnGridClicked -= HandleGridClicked;
        }
        _disposables.Dispose();
    }
}
