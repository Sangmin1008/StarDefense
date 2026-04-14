using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerModel
{
    public IReadOnlyList<StageConfig> AllStages { get; }
    public StageConfig CurrentStageConfig { get; set; }
    public int CurrentStageIndex { get; set; } = -1;
    
    public Subject<Unit> OnRequestLoadGameScene { get; } = new Subject<Unit>();
    public Subject<Unit> OnRequestLoadLobbyScene { get; } = new Subject<Unit>();

    public GameManagerModel(IReadOnlyList<StageConfig> allStages)
    {
        AllStages = allStages;
    }

    private bool HasNextStage()
    {
        return CurrentStageIndex >= 0 && CurrentStageIndex < AllStages.Count - 1;
    }

    public void PrepareStage(int index)
    {
        if (index < 0 || index >= AllStages.Count) return;
                
        CurrentStageConfig = AllStages[index];
        CurrentStageIndex = index;
        
        OnRequestLoadGameScene.OnNext(Unit.Default);
    }
    
    public void PrepareNextStage()
    {
        if (HasNextStage())
        {
            CurrentStageIndex++;
            CurrentStageConfig = AllStages[CurrentStageIndex];
            OnRequestLoadGameScene.OnNext(Unit.Default);
        }
        else
        {
            RequestLoadLobby();
        }
    }

    public void RequestLoadLobby()
    {
        OnRequestLoadLobbyScene.OnNext(Unit.Default);
    }
}
