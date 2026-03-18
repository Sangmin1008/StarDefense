using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "ScriptableObject/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Enemy Name")] 
    public string EnemyName;
    
    [Header("Settings")]
    public float MoveSpeed;
    public int AttackPower;
    public int MaxHealth;
    
}
