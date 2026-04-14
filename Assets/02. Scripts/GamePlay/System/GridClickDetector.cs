using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridClickDetector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public Tile NormalGridTile;
    [SerializeField] public Tile BrokenGridTile;
    private Tilemap _tilemap;

    public event Action<Vector3Int, Vector3> OnGridClicked;

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 worldPos = eventData.pointerCurrentRaycast.worldPosition;
        Vector3Int cellPos = _tilemap.WorldToCell(worldPos);

        if (_tilemap.HasTile(cellPos))
        {
            Vector3 centerWorldPos = _tilemap.GetCellCenterWorld(cellPos);
            OnGridClicked?.Invoke(cellPos, centerWorldPos);
        }
    }
    
    public List<Vector3Int> GetBrokenCells()
    {
        if (!_tilemap)
        {
            _tilemap = GetComponent<Tilemap>();
        }
        List<Vector3Int> brokenCells = new List<Vector3Int>();
        BoundsInt bounds = _tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = _tilemap.GetTile(pos);
            if (tile == BrokenGridTile)
            {
                brokenCells.Add(pos);
            }
        }
        return brokenCells;
    }
    
    public void ChangeToNormalTile(Vector3Int cellPos)
    {
        if (!_tilemap)
        {
            _tilemap = GetComponent<Tilemap>();
        }
        _tilemap.SetTile(cellPos, NormalGridTile);
    }
}
