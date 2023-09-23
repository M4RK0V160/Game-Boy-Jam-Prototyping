
using System.Collections.Generic;
using UnityEngine;

public class Cell
{


    [Header("Cell Variables")]
    private Vector2Int position;
    private bool occupied = false;
    public bool blocked = false;
    public bool destroyable = false;


    [Space(20)]
    [Header("Pathfinding Variables")]
    public Cell parent;

    private readonly List<Cell> neighborList = new List<Cell>();
    private MainManager manager;


    public Cell(Vector2Int position, bool blocked, bool destroyable)
    {
        this.position = position;
        this.blocked = blocked;
        this.destroyable = destroyable;
    }
    private void Awake()
    {
        manager = MainManager.Instance;
    }


    public override string ToString()
    {
        return base.ToString() + position + "blocked: " + blocked;
    }


    public void AddNeighbor(Cell cell)
    {
        neighborList.Add(cell);
    }


    public List<Cell> GetNeighbors()
    {
        return neighborList;
    }


    public void SetCellBlocked(bool blocked)
    {
        if (this.blocked == blocked || this.occupied == true)
            return;

        this.blocked = blocked;
    }

    public bool GetBlocked()
    {
        return occupied || blocked;
    }

    public bool IsWalkable()
    {
        return !blocked;
    }

    public bool isDestroyable()
    {
        return destroyable;
    }

    public Vector3Int GetPosition()
    {
        return new Vector3Int(position.x, position.y);
    }

    public Vector3 GetPositionCenter()
    {
        return new Vector3(position.x + 0.5f, position.y + 0.5f);
    }
    public void SetPosition(Vector2Int pos)
    {
        position = pos;
    }

    public void Ocupy() { occupied = true; blocked = true; }

    public void Desocupy() { occupied = false; blocked = false; }

}
