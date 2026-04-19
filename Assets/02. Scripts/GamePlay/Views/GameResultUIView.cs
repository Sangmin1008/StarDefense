using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUIView : MonoBehaviour
{
    [Header("Canvas")] 
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Result Screens")]
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI resultText;
    
    public IObservable<Unit> OnRetryClicked => retryButton.onClick.AsObservable();
    public IObservable<Unit> OnNextClicked => nextButton.onClick.AsObservable();
    public IObservable<Unit> OnExitClicked => exitButton.onClick.AsObservable();

    private void Awake()
    {
        HideResultScreens();
    }

    public void ShowResultScreen()
    {
        canvas.enabled = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideResultScreens()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvas.enabled = false;
    }

    public void SetupResult(bool victory)
    {
        if (victory)
        {
            resultText.text = "Victory!";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "Defeat!";
            resultText.color = Color.red;
        }
        
        nextButton.gameObject.SetActive(victory);
        retryButton.gameObject.SetActive(!victory);
        
        exitButton.gameObject.SetActive(true);
    }
}
