using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroConfig", menuName = "ScriptableObject/HeroConfig")]
public class HeroConfig : ScriptableObject
{
    [Header("Health Settings")]
    public int MaxHealth;
    
    [Header("Attack Settings")] 
    public int AttackPower;
    public float AttackRange;
    public float AttackCooldown;
}
