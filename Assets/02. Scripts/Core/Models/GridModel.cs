using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridModel
{
    private readonly Dictionary<Vector3Int, HeroModel> _gridData = new Dictionary<Vector3Int, HeroModel>();
    private readonly HashSet<Vector3Int> _brokenCells = new HashSet<Vector3Int>();
    public void RegisterBrokenCell(Vector3Int cellPos) => _brokenCells.Add(cellPos);
    public bool IsBroken(Vector3Int cellPos) => _brokenCells.Contains(cellPos);
    public bool IsEmpty(Vector3Int cellPos) => !_gridData.ContainsKey(cellPos) && !_brokenCells.Contains(cellPos);

    public HeroModel GetHero(Vector3Int cellPos)
    {
        return _gridData.TryGetValue(cellPos, out var model) ? model : null;
    }
    public void PlaceHero(Vector3Int cellPos, HeroModel hero) => _gridData[cellPos] = hero;
    public void ClearCell(Vector3Int cellPos) => _gridData.Remove(cellPos);
    public void RepairCell(Vector3Int cellPos) => _brokenCells.Remove(cellPos);
}
