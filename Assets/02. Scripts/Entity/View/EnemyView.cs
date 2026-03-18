using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class EnemyView : MonoBehaviour
{
    [SerializeField] private Transform hpBar;
    
    private Subject<Unit> _onReachedDestination = new Subject<Unit>();
    public IObservable<Unit> OnReachedDestination => _onReachedDestination;

    private List<Vector3> _waypoints = new List<Vector3>();
    private int _currentWaypointIndex = 0;
    private float _moveSpeed;
    private bool _canMove = false;
    
    private float _initialScaleX = 1f;

    private void Awake()
    {
        if (hpBar)
        {
            _initialScaleX = hpBar.localScale.x;
        }
    }

    private void Start()
    {
        BindMovement();
    }

    public void SetupMovement(List<Vector3> waypoints, float moveSpeed)
    {
        _moveSpeed = moveSpeed;
        _waypoints = waypoints;
        _currentWaypointIndex = 0;

        if (_waypoints != null && _waypoints.Count > 0)
        {
            transform.position = _waypoints[_currentWaypointIndex];
            _canMove = true;
        }
    }

    private void BindMovement()
    {
        this.UpdateAsObservable()
            .Where(_ => _canMove)
            .Subscribe(_ => MoveAlongPath());
    }

    private void MoveAlongPath()
    {
        if (_currentWaypointIndex >= _waypoints.Count) return;
        
        Vector3 targetPos = _waypoints[_currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            _currentWaypointIndex++;
            
            if (_currentWaypointIndex >= _waypoints.Count)
            {
                _canMove = false;
                _onReachedDestination.OnNext(Unit.Default);
            }
        }
    }

    public void UpdateHpBar(float health)
    {
        if (!hpBar) return;
        Vector3 scale = hpBar.localScale;
        scale.x = health * _initialScaleX;;
        hpBar.localScale = scale;
    }

    private void OnDestroy()
    {
        _onReachedDestination.Dispose();
    }
}
