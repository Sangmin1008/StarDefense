using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "ScriptableObject/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Header("Wave Settings")]
    [SerializeField, FormerlySerializedAs("EnemyType")]
    private EnemyConfig enemyType;
    public EnemyConfig EnemyType => enemyType;

    [SerializeField, FormerlySerializedAs("SpawnCount")]
    private int spawnCount;
    public int SpawnCount => spawnCount;

    [SerializeField, FormerlySerializedAs("SpawnInterval")]
    private float spawnInterval;
    public float SpawnInterval => spawnInterval;

    [SerializeField, FormerlySerializedAs("DelayTimeBeforeWave")]
    private float delayTimeBeforeWave;
    public float DelayTimeBeforeWave => delayTimeBeforeWave;

    [Header("Wave Path")]
    [SerializeField, FormerlySerializedAs("PathData")]
    private PathDataSO pathData;
    public PathDataSO PathData => pathData;
}
