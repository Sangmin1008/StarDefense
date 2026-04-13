using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommanderUIView : MonoBehaviour
{
    [Header("Commander UI")]
    [SerializeField] private Slider commanderHpSlider;
    [SerializeField] private TextMeshProUGUI commanderHpText;
    
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
}
