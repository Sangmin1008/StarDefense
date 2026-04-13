using System;
using UniRx;
using VContainer.Unity;

public class CommanderUIPresenter : IInitializable, IDisposable
{
    private readonly CommanderModel _commanderModel;
    private readonly CommanderUIView _uiView;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    
    public CommanderUIPresenter(CommanderModel commanderModel, CommanderUIView uiView)
    {
        _commanderModel = commanderModel;
        _uiView = uiView;
    }
    
    public void Initialize()
    {
        _commanderModel.CurrentHp
            .Subscribe(hp => _uiView.UpdateCommanderHp(hp, _commanderModel.Config.MaxHealth))
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
