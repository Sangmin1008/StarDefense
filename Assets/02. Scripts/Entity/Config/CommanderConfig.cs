using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommanderConfig", menuName = "ScriptableObject/CommanderConfig")]
public class CommanderConfig : ScriptableObject
{
    [Header("Health Settings")]
    public int MaxHealth;
    
    [Header("Attack Settings")] 
    public int AttackPower;
    public float AttackRange;
    public float AttackCooldown;
    
    [Header("Projectile Settings")]
    public ProjectileView ProjectilePrefab;
    public float ProjectileSpeed;
    public float ProjectileMaxDistance;
}
