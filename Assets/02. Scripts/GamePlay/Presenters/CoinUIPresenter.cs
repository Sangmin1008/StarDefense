using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class CoinUIPresenter : IInitializable, IDisposable
{
    private readonly CoinModel _coinModel;
    private readonly CoinUIView _uiView;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    public CoinUIPresenter(CoinModel coinModel, CoinUIView uiView)
    {
        _coinModel = coinModel;
        _uiView = uiView;
    }

    public void Initialize()
    {
        _coinModel.CurrentCoin
            .Subscribe(amount => _uiView.UpdateCoin(amount))
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
