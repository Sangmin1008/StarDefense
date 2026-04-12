using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class HeroSpawner
{
    private readonly Dictionary<HeroModel, HeroPresenter> _activeHeroes = new Dictionary<HeroModel, HeroPresenter>();
    private readonly List<HeroConfig> _heroDatabase;
    private readonly GridModel _gridModel;
    private readonly EnemyRegistry _enemyRegistry;
    private readonly CoinModel _coinModel;
    private readonly ProjectileManager _projectileManager;
    private readonly Dictionary<HeroConfig, ObjectPool<HeroView>> _heroPools = new Dictionary<HeroConfig, ObjectPool<HeroView>>();

    private const float _offset = 0.3f; 
    
    public HeroSpawner(GridModel gridModel, EnemyRegistry enemyRegistry, List<HeroConfig> heroDatabase, CoinModel coinModel, ProjectileManager projectileManager)
    {
        _gridModel = gridModel;
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
                createFunc: () => Object.Instantiate(config.Prefab).GetComponent<HeroView>(),
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
        HeroView view = presenter.View;
        
        presenter.Dispose();

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

        _activeHeroes.Add(model, presenter);
        _gridModel.PlaceHero(cellPos, model);
        
        Debug.Log($"[{cellPos}] 위치에 {config.Grade} 등급 {config.HeroName} 소환 완료!");
    }
}
