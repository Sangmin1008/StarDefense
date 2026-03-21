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
    private readonly Dictionary<HeroConfig, ObjectPool<HeroView>> _heroPools = new Dictionary<HeroConfig, ObjectPool<HeroView>>();    
    public HeroManager(GridManager gridManager, EnemyRegistry enemyRegistry, List<HeroConfig> heroDatabase)
    {
        _gridManager = gridManager;
        _enemyRegistry = enemyRegistry;
        _heroDatabase = heroDatabase;
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

    public void SpawnHero(HeroGrade grade, Vector3Int cellPos, Vector3 worldPos)
    {
        Debug.Log("Spawning hero: " + grade);
        List<HeroConfig> availableHeroes = _heroDatabase.Where(h => h.Grade == grade).ToList();
        if (availableHeroes.Count == 0)
        {
            Debug.Log("No heroes available");
            return;
        }
        
        HeroConfig randomConfig = availableHeroes[Random.Range(0, availableHeroes.Count)];

        HeroView view = GetHeroView(randomConfig);
        view.transform.position = worldPos;

        HeroModel model = new HeroModel(randomConfig, cellPos);
        HeroPresenter presenter = new HeroPresenter(model, view, _enemyRegistry);
        presenter.Initialize();

        _activeHeroes.Add(presenter);
        _gridManager.PlaceHero(cellPos, presenter);
        
        Debug.Log($"[{cellPos}] 위치에 {randomConfig.Grade} 등급 {randomConfig.HeroName} 소환 완료!");
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

    public void TryUpgradeHero(HeroPresenter targetHero)
    {
        HeroGrade currentGrade = targetHero.Model.Config.Grade;
        if (currentGrade == HeroGrade.Legendary) return;
        
        HeroPresenter materialHero = _activeHeroes.FirstOrDefault(h => h.Model.Config.Grade == currentGrade && h != targetHero);
        if (materialHero == null) return;

        Vector3Int upgradeCellPos = targetHero.Model.CellPos;
        Vector3 upgradeWorldPos = targetHero.View.transform.position;
        
        RemoveHero(targetHero);
        RemoveHero(materialHero);
        
        HeroGrade nextGrade = currentGrade + 1;
        SpawnHero(nextGrade, upgradeCellPos, upgradeWorldPos);
    }
}
