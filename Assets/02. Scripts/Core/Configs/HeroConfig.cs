using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum HeroGrade
{
    Normal = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3
}

public enum HeroType
{
    TypeA,
    TypeB,
}

[CreateAssetMenu(fileName = "HeroConfig", menuName = "ScriptableObject/HeroConfig")]
public class HeroConfig : ScriptableObject
{
    [Header("Settings")]
    [SerializeField, FormerlySerializedAs("HeroName")]
    private string heroName;
    public string HeroName => heroName;

    [SerializeField, FormerlySerializedAs("Grade")]
    private HeroGrade grade;
    public HeroGrade Grade => grade;

    [SerializeField, FormerlySerializedAs("Type")]
    private HeroType type;
    public HeroType Type => type;

    [Header("Prefabs")]
    [SerializeField, FormerlySerializedAs("Prefab")]
    private GameObject prefab;
    public GameObject Prefab => prefab;

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
