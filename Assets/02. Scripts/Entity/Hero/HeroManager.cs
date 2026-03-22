using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class HeroManager
{
    private readonly HashSet<HeroPresenter> _activeHeroes = new HashSet<HeroPresenter>();
    private readonly List<HeroConfig> _heroDatabase;
    private readonly GridManager _gridManager;
    private readonly EnemyRegistry _enemyRegistry;
    private readonly CoinModel _coinModel;
    private readonly ProjectileManager _projectileManager;
    private readonly Dictionary<HeroConfig, ObjectPool<HeroView>> _heroPools = new Dictionary<HeroConfig, ObjectPool<HeroView>>();    
    public HeroManager(GridManager gridManager, EnemyRegistry enemyRegistry, List<HeroConfig> heroDatabase, CoinModel coinModel, ProjectileManager projectileManager)
    {
        _gridManager = gridManager;
        _enemyRegistry = enemyRegistry;
        _heroDatabase = heroDatabase;
        _coinModel = coinModel;
        _projectileManager = projectileManager;
    }
    
    private HeroView GetHeroView(HeroConfig config)
    {
        if (!_heroPools.ContainsKey(config))
        {
            _heroPools[config] = new ObjectPool<HeroView>(
                createFunc: () => Object.Instantiate(config.Prefab),
                actionOnGet: view => view.gameObject.SetActive(true),
                actionOnRelease: view => view.gameObject.SetActive(false),
                actionOnDestroy: view => { if (view) Object.Destroy(view.gameObject); }
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

        ForceSpawnHero(randomConfig, cellPos, worldPos);
        return true;
    }

    public bool TryUpgradeHero(HeroPresenter targetHero)
    {
        HeroGrade currentGrade = targetHero.Model.Config.Grade;
        HeroType currentType = targetHero.Model.Config.Type;
        
        if (currentGrade == HeroGrade.Legendary) return false;
        
        HeroPresenter otherHero = _activeHeroes.FirstOrDefault(h => h.Model.Config.Grade == currentGrade &&
                                                                    h.Model.Config.Type == currentType && h != targetHero);
        if (otherHero == null) return false;

        if (!_coinModel.TrySpendCoin(HeroCostHelper.GetCost(targetHero.Model.Config.Grade))) return false;
        
        Vector3Int upgradeCellPos = targetHero.Model.CellPos;
        Vector3 upgradeWorldPos = targetHero.View.transform.position;
        
        RemoveHero(targetHero);
        RemoveHero(otherHero);
        
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

    public void RemoveHero(HeroPresenter hero)
    {
        _activeHeroes.Remove(hero);
        _gridManager.ClearCell(hero.Model.CellPos);
        
        HeroConfig config = hero.Model.Config;
        HeroView view = hero.View;
        
        hero.Dispose();

        if (_heroPools.ContainsKey(config))
        {
            _heroPools[config].Release(view);
        }
        else
        {
            Object.Destroy(view.gameObject);
        }
    }

    private void ForceSpawnHero(HeroConfig config, Vector3Int cellPos, Vector3 worldPos)
    {
        HeroView view = GetHeroView(config);
        view.transform.position = worldPos;

        HeroModel model = new HeroModel(config, cellPos);
        HeroPresenter presenter = new HeroPresenter(model, view, _enemyRegistry, _projectileManager);
        presenter.Initialize();

        _activeHeroes.Add(presenter);
        _gridManager.PlaceHero(cellPos, presenter);
        
        Debug.Log($"[{cellPos}] 위치에 {config.Grade} 등급 {config.HeroName} 소환 완료!");
    }
}
