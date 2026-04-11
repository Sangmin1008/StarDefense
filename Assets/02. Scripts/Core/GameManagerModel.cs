using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerModel
{
    public IReadOnlyList<StageConfig> AllStages { get; }
    public StageConfig CurrentStageConfig { get; set; }
    public int CurrentStageIndex { get; set; } = -1;

    public GameManagerModel(IReadOnlyList<StageConfig> allStages)
    {
        AllStages = allStages;
    }

    public bool HasNextStage()
    {
        Debug.Log($"{CurrentStageIndex}");
        return CurrentStageIndex >= 0 && CurrentStageIndex < AllStages.Count - 1;
    }

    public void SetNextStage()
    {
        if (HasNextStage())
        {
            CurrentStageIndex++;
            CurrentStageConfig = AllStages[CurrentStageIndex];
        }
    }
}
