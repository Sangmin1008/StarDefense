using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "StageConfig", menuName = "ScriptableObject/StageConfig")]
public class StageConfig : ScriptableObject
{
    [Header("TileMap Settings")]
    [SerializeField, FormerlySerializedAs("TileMapPrefab")]
    private GameObject tileMapPrefab;
    public GameObject TileMapPrefab => tileMapPrefab;
    
    [Header("Economy Settings")]
    [SerializeField, FormerlySerializedAs("InitialCoin")]
    private int initialCoin;
    public int InitialCoin => initialCoin;

    [Header("Commander Settings")]
    [SerializeField, FormerlySerializedAs("CommanderConfig")]
    private CommanderConfig commanderConfig;
    public CommanderConfig CommanderConfig => commanderConfig;

    [SerializeField, FormerlySerializedAs("CommanderPosition")]
    private Vector3 commanderPosition;
    public Vector3 CommanderPosition => commanderPosition;

    [Header("Wave Settings")]
    [SerializeField, FormerlySerializedAs("Waves")]
    private List<WaveConfig> waves;
    public IReadOnlyList<WaveConfig> Waves => waves;
}
