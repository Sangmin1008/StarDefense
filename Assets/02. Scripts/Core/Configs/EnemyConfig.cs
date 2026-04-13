using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "ScriptableObject/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Enemy Name")]
    [SerializeField, FormerlySerializedAs("EnemyName")]
    private string enemyName;
    public string EnemyName => enemyName;
    
    [Header("Settings")]
    [SerializeField, FormerlySerializedAs("MoveSpeed")]
    private float moveSpeed;
    public float MoveSpeed => moveSpeed;

    [SerializeField, FormerlySerializedAs("AttackPower")]
    private int attackPower;
    public int AttackPower => attackPower;

    [SerializeField, FormerlySerializedAs("MaxHealth")]
    private int maxHealth;
    public int MaxHealth => maxHealth;

    [Header("Reward")]
    [SerializeField, FormerlySerializedAs("Reward")]
    private int reward;
    public int Reward => reward;
}
