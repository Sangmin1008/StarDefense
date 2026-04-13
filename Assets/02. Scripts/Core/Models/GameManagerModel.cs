using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerModel
{
    public IReadOnlyList<StageConfig> AllStages { get; }
    public StageConfig CurrentStageConfig { get; set; }
    public int CurrentStageIndex { get; set; } = -1;

    public GameManagerModel(IReadOnlyList<StageConfig> allStages)
    {
        AllStages = allStages;
    }

    private bool HasNextStage()
    {
        Debug.Log($"{CurrentStageIndex}");
        return CurrentStageIndex >= 0 && CurrentStageIndex < AllStages.Count - 1;
    }

    private void SetNextStage()
    {
        if (HasNextStage())
        {
            CurrentStageIndex++;
            CurrentStageConfig = AllStages[CurrentStageIndex];
        }
    }

    public void LoadStage(int index)
    {
        if (index < 0 || index >= AllStages.Count) return;
                
        CurrentStageConfig = AllStages[index];
        CurrentStageIndex = index;
        SceneManager.LoadScene(SceneNames.Game);
    }

    public void LoadNextStage()
    {
        if (HasNextStage())
        {
            SetNextStage();
            SceneManager.LoadScene(SceneNames.Game);
        }
        else
        {
            SceneManager.LoadScene(SceneNames.Lobby);
        }
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene(SceneNames.Lobby);
    }
}
