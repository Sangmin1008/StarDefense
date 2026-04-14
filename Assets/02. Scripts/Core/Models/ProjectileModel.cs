using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModel
{
    public int Damage { get; private set; }
    public float Speed { get; private set; }
    public float MaxDistance { get; private set; }
    
    public Vector3 CurrentDirection { get; set; }
    public float TraveledDistance { get; set; }

    public EnemyModel TargetModel { get; private set; }
    public GameObject TargetView { get; private set; }
    
    public ProjectileModel(int damage, float speed, float maxDistance, EnemyModel targetModel, GameObject targetView)
    {
        UpdateData(damage, speed, maxDistance, targetModel, targetView);
    }

    public void UpdateData(int damage, float speed, float maxDist, EnemyModel targetModel, GameObject targetView)
    {
        Damage = damage;
        Speed = speed;
        MaxDistance = maxDist;
        TargetModel = targetModel;
        TargetView = targetView;
        TraveledDistance = 0f;
    }

    public void ResetTarget()
    {
        TargetModel = null;
        TargetView = null;
    }
}
