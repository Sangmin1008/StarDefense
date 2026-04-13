using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroModel
{
    public HeroConfig Config { get; private set; }
    public int CurrentAttackPower { get; private set; }
    public Vector3Int CellPos { get; private set; }
    
    public HeroModel() { }

    public void ResetData(HeroConfig config, Vector3Int cellPos)
    {
        Config = config;
        CurrentAttackPower = config.AttackPower;
        CellPos = cellPos;
    }
}
