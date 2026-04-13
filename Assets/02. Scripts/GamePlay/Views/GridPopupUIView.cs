using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GridPopupUIView : MonoBehaviour
{
    [Header("Grid Popup UI")]
    [SerializeField] private RectTransform gridPopupContainer; 
    [SerializeField] private Button summonButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button repairButton;
    [SerializeField] private TextMeshProUGUI needSummonCostText;
    [SerializeField] private TextMeshProUGUI needUpgradeCostText;
    [SerializeField] private TextMeshProUGUI repairCostText;
    
    public IObservable<Unit> OnSummonClicked => summonButton.onClick.AsObservable();
    public IObservable<Unit> OnUpgradeClicked => upgradeButton.onClick.AsObservable();
    public IObservable<Unit> OnRepairClicked => repairButton.onClick.AsObservable();
    
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
}
