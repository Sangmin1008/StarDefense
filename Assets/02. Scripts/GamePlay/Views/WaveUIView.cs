using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveUIView : MonoBehaviour
{
    [Header("Wave Info UI")]
    [SerializeField] private TextMeshProUGUI waveCountText;
    [SerializeField] private TextMeshProUGUI aliveEnemyCountText;
    [SerializeField] private TextMeshProUGUI waveDelayText;
    
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
}
