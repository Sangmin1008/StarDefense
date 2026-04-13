using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUIView : MonoBehaviour
{
    [Header("Result Screens")]
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private TextMeshProUGUI defeatText;
    
    public IObservable<Unit> OnRetryClicked => retryButton.onClick.AsObservable();
    public IObservable<Unit> OnNextClicked => nextButton.onClick.AsObservable();
    public IObservable<Unit> OnExitClicked => exitButton.onClick.AsObservable();
    
    public void ShowVictoryScreen()
    {
        if (!resultScreen) return;
        resultScreen.SetActive(true);
        ShowResult(true);
    }

    public void ShowDefeatScreen()
    {
        if (!resultScreen) return;
        resultScreen.SetActive(true);
        ShowResult(false);
    }

    private void HideResultScreens()
    {
        if (resultScreen != null) resultScreen.SetActive(false);
    }

    private void ShowResult(bool victory)
    {
        victoryText.gameObject.SetActive(victory);
        defeatText.gameObject.SetActive(!victory);
        
        nextButton.gameObject.SetActive(victory);
        retryButton.gameObject.SetActive(!victory);
        
        exitButton.gameObject.SetActive(true);
    }
}
