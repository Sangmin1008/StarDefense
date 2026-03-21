using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroModel
{
    public HeroConfig Config { get; }
    public int CurrentAttackPower { get; }
    public Vector3Int CellPos { get; }

    public HeroModel(HeroConfig config, Vector3Int cellPos)
    {
        Config = config;
        CurrentAttackPower = config.AttackPower;
        CellPos = cellPos;
    }
}
