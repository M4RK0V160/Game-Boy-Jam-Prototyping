using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    private MapManager mapManager;
    private Cell target;
    private Vector3Int targetPosition;
    private List<Cell> path;
    private AStarPath pathfinder;
    public Cell occupiedCell;
    private MainManager mainManager;


    public int HP;

    private bool shake;
    private bool Aggro;
    private bool inRange;

    private bool facingRight;


    public PlayerController playerController;


    public float aggroDistance = 6;

    private void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        pathfinder = new AStarPath();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        occupiedCell = mapManager.cells[(int)transform.position.x, (int)transform.position.y];
        Aggro = false;
        target = CheckAggro();
        targetPosition = new Vector3Int(target.GetPosition().x, target.GetPosition().y);
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>().Instance;

        facingRight = true;
        
    }

    private void Update()
    {
        if (shake)
        {
            transform.position.Set(Mathf.Sin(Time.time * 1) * 1, transform.position.y, 0);
        }
        {
            
        }
        if (HP <= 0)
        {
            occupiedCell.Desocupy();
            Destroy(gameObject);
        }
    }
    private void paintPath(Tile colour)
    {
        if (path != null)
        {
            foreach (Cell cell in path)
            {
                if (cell != path.Last())
                {
                    mapManager.map.SetTile(cell.GetPosition(), colour);
                }
            }
        }
    }

    private Cell CheckAggro()
    {
        if((playerController.occupiedCell.GetPosition()-occupiedCell.GetPosition()).magnitude < aggroDistance)
        {
            target = playerController.occupiedCell;
            Aggro = true;
        }
        else
        {
            target = occupiedCell;
            Aggro = false;
        }

        return target;
        
    }
    private bool CheckRange()
    {
        if (occupiedCell.GetNeighbors().Contains(playerController.occupiedCell))
        { 
            return true;
        }
        return false;
    }

    private void Step()
    {
        occupiedCell.blocked = false;

        path.Remove(path.First());
        var nextCell = path.First();
        var moveVector = nextCell.GetPosition() - occupiedCell.GetPosition();
        
        if (facingRight)
        {
            if(moveVector.x < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
                facingRight = false;
            }
        }
        else
        {
            if (moveVector.x > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
                facingRight = true;
            }
        }
        transform.Translate(moveVector);

        occupiedCell = mapManager.cells[(int)transform.position.x, (int)transform.position.y];
        occupiedCell.blocked = true;

    }




    public void Act()
    {
        occupiedCell = mapManager.cells[(int)transform.position.x, (int)transform.position.y];
        target = CheckAggro();
        inRange = CheckRange();

        if (Aggro)
        {
           if(mainManager.DebugMode) paintPath(mapManager.Floors[Random.Range(0, mapManager.Floors.Length)]);
            path = pathfinder.CalculatePath(occupiedCell, target);

            if (mainManager.DebugMode)paintPath(mapManager.FloodTiles[4]);
            if ( path != null && path.Count > 0 && !inRange)
            {
                Step();
            }
            else
            {
                Attack();
            }
        } 
    }

    private void Attack()
    {
        shake = true;
        playerController.GetComponent<Animator>().SetTrigger("GetHit");
        StartCoroutine(attackDuration());
        shake = false;
    }

    private IEnumerator attackDuration()
    {
        yield return new WaitForSeconds(1);
    }

    public void takeAHit()
    {
        HP--;
        //audioSource.playOneShot(getHit);
    }
}
