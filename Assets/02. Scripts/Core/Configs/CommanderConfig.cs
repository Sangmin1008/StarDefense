using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CommanderConfig", menuName = "ScriptableObject/CommanderConfig")]
public class CommanderConfig : ScriptableObject
{
    [Header("Health Settings")]
    [SerializeField, FormerlySerializedAs("MaxHealth")]
    private int maxHealth;
    public int MaxHealth => maxHealth;

    [Header("Attack Settings")] 
    [SerializeField, FormerlySerializedAs("AttackPower")]
    private int attackPower;
    public int AttackPower => attackPower;

    [SerializeField, FormerlySerializedAs("AttackRange")]
    private float attackRange;
    public float AttackRange => attackRange;

    [SerializeField, FormerlySerializedAs("AttackCooldown")]
    private float attackCooldown;
    public float AttackCooldown => attackCooldown;

    [Header("Projectile Settings")]
    [SerializeField, FormerlySerializedAs("ProjectilePrefab")]
    private GameObject projectilePrefab;
    public GameObject ProjectilePrefab => projectilePrefab;

    [SerializeField, FormerlySerializedAs("ProjectileSpeed")]
    private float projectileSpeed;
    public float ProjectileSpeed => projectileSpeed;

    [SerializeField, FormerlySerializedAs("ProjectileMaxDistance")]
    private float projectileMaxDistance;
    public float ProjectileMaxDistance => projectileMaxDistance;
}
