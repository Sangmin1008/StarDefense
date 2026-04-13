using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class HeroSpawner : IDisposable
{
    private readonly Dictionary<HeroModel, HeroPresenter> _activeHeroes = new Dictionary<HeroModel, HeroPresenter>();
    private readonly List<HeroConfig> _heroDatabase;
    private readonly GridModel _gridModel;
    private readonly EnemyRegistry _enemyRegistry;
    private readonly CoinModel _coinModel;
    private readonly ProjectileSpawner _projectileSpawner;
    
    private readonly Dictionary<HeroConfig, ObjectPool<HeroPresenter>> _heroPools = new Dictionary<HeroConfig, ObjectPool<HeroPresenter>>();

    private const float _offset = 0.3f; 
    
    public HeroSpawner(GridModel gridModel, EnemyRegistry enemyRegistry, List<HeroConfig> heroDatabase, CoinModel coinModel, ProjectileSpawner projectileSpawner)
    {
        _gridModel = gridModel;
        _enemyRegistry = enemyRegistry;
        _heroDatabase = heroDatabase;
        _coinModel = coinModel;
        _projectileSpawner = projectileSpawner;
    }
    
    private HeroPresenter CreatePresenter(HeroConfig config)
    {
        HeroView view = Object.Instantiate(config.Prefab).GetComponent<HeroView>();
        HeroModel model = new HeroModel();
        
        return new HeroPresenter(model, view, _enemyRegistry, _projectileSpawner);
    }
    
    private HeroPresenter GetHeroPresenter(HeroConfig config)
    {
        if (!_heroPools.ContainsKey(config))
        {
            _heroPools[config] = new ObjectPool<HeroPresenter>(
                createFunc: () => CreatePresenter(config),
                actionOnGet: p => p.View.gameObject.SetActive(true),
                actionOnRelease: p => 
                {
                    p.View.gameObject.SetActive(false);
                    p.Release();
                },
                actionOnDestroy: p => 
                {
                    p.Dispose();
                    if (p.View) Object.Destroy(p.View.gameObject);
                }
            );
        }
        return _heroPools[config].Get();
    }

    public bool TrySpawnHero(HeroGrade grade, Vector3Int cellPos, Vector3 worldPos)
    {
        List<HeroConfig> availableHeroes = _heroDatabase.Where(h => h.Grade == grade).ToList();
        if (availableHeroes.Count == 0) return false;
        
        HeroConfig randomConfig = availableHeroes[Random.Range(0, availableHeroes.Count)];

        if (!_coinModel.TrySpendCoin(HeroCostHelper.GetCost(randomConfig.Grade))) return false;

        worldPos.y += _offset;

        ForceSpawnHero(randomConfig, cellPos, worldPos);
        return true;
    }

    public bool TryUpgradeHero(HeroModel targetModel)
    {
        HeroGrade currentGrade = targetModel.Config.Grade;
        HeroType currentType = targetModel.Config.Type;
        
        if (currentGrade == HeroGrade.Legendary) return false;
        
        HeroModel otherModel = _activeHeroes.Keys.FirstOrDefault(h => 
            h.Config.Grade == currentGrade &&
            h.Config.Type == currentType && 
            h != targetModel);
        
        if (otherModel == null) return false;

        if (!_coinModel.TrySpendCoin(HeroCostHelper.GetCost(targetModel.Config.Grade))) return false;
        
        Vector3Int upgradeCellPos = targetModel.CellPos;
        Vector3 upgradeWorldPos = _activeHeroes[targetModel].View.transform.position;
        
        RemoveHero(targetModel);
        RemoveHero(otherModel);
        
        HeroGrade nextGrade = currentGrade + 1;
        List<HeroConfig> availableHeroes = _heroDatabase.Where(h => 
            h.Grade == nextGrade && 
            h.Type == currentType).ToList();
        
        if (availableHeroes.Count > 0)
        {
            HeroConfig randomConfig = availableHeroes[Random.Range(0, availableHeroes.Count)];
            ForceSpawnHero(randomConfig, upgradeCellPos, upgradeWorldPos); 
        }
        
        return true;
    }

    public void RemoveHero(HeroModel targetModel)
    {
        if (!_activeHeroes.TryGetValue(targetModel, out HeroPresenter presenter)) return;
        
        _activeHeroes.Remove(targetModel);
        _gridModel.ClearCell(targetModel.CellPos);
        
        HeroConfig config = targetModel.Config;
        
        if (_heroPools.ContainsKey(config))
        {
            _heroPools[config].Release(presenter);
        }
        else
        {
            presenter.Dispose();
            Object.Destroy(presenter.View.gameObject);
        }
    }

    private void ForceSpawnHero(HeroConfig config, Vector3Int cellPos, Vector3 worldPos)
    {
        HeroPresenter presenter = GetHeroPresenter(config);
        
        presenter.View.transform.position = worldPos;
        presenter.ResetAndStart(config, cellPos);

        _activeHeroes.Add(presenter.Model, presenter);
        _gridModel.PlaceHero(cellPos, presenter.Model);
        
        Debug.Log($"[{cellPos}] 위치에 {config.Grade} 등급 {config.HeroName} 소환 완료!");
    }

    public void Dispose()
    {
        foreach (var presenter in _activeHeroes.Values)
        {
            presenter.Dispose(); 
        }
        _activeHeroes.Clear();

        foreach (var pool in _heroPools.Values)
        {
            pool.Dispose();
        }
        _heroPools.Clear();
    }
}
