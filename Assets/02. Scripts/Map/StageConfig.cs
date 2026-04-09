using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageConfig", menuName = "ScriptableObject/StageConfig")]
public class StageConfig : ScriptableObject
{
    [Header("TileMap Settings")]
    public GameObject TileMapPrefab;
    
    [Header("Economy Settings")]
    public int InitialCoin;
    
    [Header("Commander Settings")]
    public CommanderConfig CommanderConfig;
    public Vector3 CommanderPosition;
    
    [Header("Wave Settings")]
    public List<WaveConfig> Waves;
}
