using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class GameUIView : MonoBehaviour
{
    [Header("Commander UI")]
    [SerializeField] private Slider commanderHpSlider;
    [SerializeField] private TextMeshProUGUI commanderHpText;

    [Header("Wave Info UI")]
    [SerializeField] private TextMeshProUGUI waveCountText;
    [SerializeField] private TextMeshProUGUI aliveEnemyCountText;
    [SerializeField] private TextMeshProUGUI waveDelayText;
    
    [Header("Grid Popup UI")]
    [SerializeField] private RectTransform gridPopupContainer; 
    [SerializeField] private Button summonButton;
    [SerializeField] private Button upgradeButton;

    [Header("Result Screens")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject defeatScreen;
    
    public IObservable<Unit> OnSummonClicked => summonButton.onClick.AsObservable();
    public IObservable<Unit> OnUpgradeClicked => upgradeButton.onClick.AsObservable();

    private void Awake()
    {
        HideResultScreens();
        HideGridPopup();
        if (waveDelayText != null) waveDelayText.gameObject.SetActive(false);
    }
    
    public void UpdateCommanderHp(float current, float max)
    {
        if (commanderHpSlider != null)
        {
            commanderHpSlider.maxValue = max;
            commanderHpSlider.value = current;
        }
        if (commanderHpText != null)
        {
            commanderHpText.text = $"{current} / {max}";
        }
    }
    
    public void UpdateWaveInfo(int currentWaveIndex, int maxWaves)
    {
        if (!waveCountText) return;
        waveCountText.text = $"Wave: {currentWaveIndex + 1} / {maxWaves}";
    }
    
    public void UpdateAliveEnemies(int current, int total)
    {
        if (!aliveEnemyCountText) return;
        aliveEnemyCountText.text = $"Enemies: {current} / {total}";
    }

    public void UpdateDelayCountdown(float secondsLeft)
    {
        if (!waveDelayText) return;
        
        if (secondsLeft > 0)
        {
            waveDelayText.gameObject.SetActive(true);
            waveDelayText.text = $"Next Wave in:\n{secondsLeft}s";
        }
        else
        {
            waveDelayText.gameObject.SetActive(false);
        }
    }
    
    public void ShowGridPopup(Vector3 worldPos, bool isSummon)
    {
        if (!gridPopupContainer) return;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        gridPopupContainer.position = screenPos;

        gridPopupContainer.gameObject.SetActive(true);
        
        summonButton.gameObject.SetActive(isSummon);
        upgradeButton.gameObject.SetActive(!isSummon);
    }

    public void HideGridPopup()
    {
        if (!gridPopupContainer) return;
        gridPopupContainer.gameObject.SetActive(false);
    }

    public void ShowVictoryScreen()
    {
        if (!victoryScreen) return;
        victoryScreen.SetActive(true);
    }

    public void ShowDefeatScreen()
    {
        if (!defeatScreen) return;
        defeatScreen.SetActive(true);
    }

    private void HideResultScreens()
    {
        if (victoryScreen != null) victoryScreen.SetActive(false);
        if (defeatScreen != null) defeatScreen.SetActive(false);
    }
}
