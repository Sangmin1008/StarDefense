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
    [SerializeField] private Button repairButton;
    [SerializeField] private TextMeshProUGUI needSummonCostText;
    [SerializeField] private TextMeshProUGUI needUpgradeCostText;
    [SerializeField] private TextMeshProUGUI repairCostText;
    
    [Header("Coin UI")]
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Result Screens")]
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private TextMeshProUGUI defeatText;
    
    public IObservable<Unit> OnSummonClicked => summonButton.onClick.AsObservable();
    public IObservable<Unit> OnUpgradeClicked => upgradeButton.onClick.AsObservable();
    public IObservable<Unit> OnRepairClicked => repairButton.onClick.AsObservable();

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
        waveCountText.text = $"{currentWaveIndex + 1} / {maxWaves}";
    }
    
    public void UpdateAliveEnemies(int current, int total)
    {
        if (!aliveEnemyCountText) return;
        aliveEnemyCountText.text = $"{current} / {total}";
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

    public void UpdateCoin(int amount)
    {
        if (!coinText) return;
        coinText.text = $"{amount}";
    }
    
    public void ShowGridPopup(Vector3 worldPos, bool isSummon, int cost)
    {
        if (!gridPopupContainer) return;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        gridPopupContainer.position = screenPos;

        gridPopupContainer.gameObject.SetActive(true);
        
        summonButton.gameObject.SetActive(isSummon);
        upgradeButton.gameObject.SetActive(!isSummon);
        repairButton.gameObject.SetActive(false);
        
        if (isSummon && needSummonCostText)
        {
            needSummonCostText.text = $"{cost}";
        }
        else if (!isSummon && needUpgradeCostText)
        {
            needUpgradeCostText.text = $"{cost}";
        }
    }


    public void ShowRepairGridPopup(Vector3 worldPos, int cost)
    {
        if (!gridPopupContainer) return;
        
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        gridPopupContainer.position = screenPos;

        gridPopupContainer.gameObject.SetActive(true);
        summonButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
        repairButton.gameObject.SetActive(true);
        
        repairCostText.text = $"{cost}";
    }

    public void HideGridPopup()
    {
        if (!gridPopupContainer) return;
        gridPopupContainer.gameObject.SetActive(false);
    }

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
