using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUIView : MonoBehaviour
{
    [Header("Coin UI")]
    [SerializeField] private TextMeshProUGUI coinText;
    
    public void UpdateCoin(int amount)
    {
        if (!coinText) return;
        coinText.text = $"{amount}";
    }
}
