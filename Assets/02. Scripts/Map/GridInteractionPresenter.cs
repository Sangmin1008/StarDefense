using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class GridInteractionPresenter : IInitializable, IDisposable
{
    private readonly GridManager _gridManager;
    private readonly HeroManager _heroManager;
    private readonly GridClickDetector _clickDetector;
    private readonly GameUIView _uiView;
    
    private Vector3Int _selectedCellPos;
    private Vector3 _selectedWorldPos;
    private HeroPresenter _selectedHero;
    private CompositeDisposable _disposables = new CompositeDisposable();
    

    public GridInteractionPresenter(GridManager gridManager, GridClickDetector clickDetector, GameUIView uiView, HeroManager heroManager)
    {
        _gridManager = gridManager;
        _clickDetector = clickDetector;
        _uiView = uiView;
        _heroManager = heroManager;
    }
    
    public void Initialize()
    {
        _clickDetector.OnGridClicked += HandleGridClicked;
        
        _uiView.OnSummonClicked
            .Subscribe(_ =>
            {
                _heroManager.SpawnHero(HeroGrade.Normal, _selectedCellPos, _selectedWorldPos);
                _uiView.HideGridPopup();
            })
            .AddTo(_disposables);
        
        _uiView.OnUpgradeClicked
            .Subscribe(_ =>
            {
                if (_selectedHero != null)
                {
                    _heroManager.TryUpgradeHero(_selectedHero);
                }
                _uiView.HideGridPopup();
            })
            .AddTo(_disposables);
    }

    private void HandleGridClicked(Vector3Int cellPos, Vector3 worldPos)
    {
        _selectedCellPos = cellPos;
        _selectedWorldPos = worldPos;

        if (_gridManager.IsEmpty(cellPos))
        {
            _selectedHero = null;
            _uiView.ShowGridPopup(worldPos, isSummon: true);
        }
        else
        {
            _selectedHero = _gridManager.GetHero(cellPos);
            _uiView.ShowGridPopup(worldPos, isSummon: false);
        }
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
