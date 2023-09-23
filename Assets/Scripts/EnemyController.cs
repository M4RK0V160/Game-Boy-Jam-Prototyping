
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public MapManager mapManager;
    private Cell target;
    private Vector3Int targetPosition;
    private List<Cell> path;
    private AStarPath pathfinder;
    public Cell occupiedCell;
    private MainManager mainManager;

    public AudioClip moveAudio;
    public AudioClip dieAudio;

    public AudioSource audioSource;


    public int HP;

    public bool Aggro;
    private bool inRange;

    public bool facingRight;

    public bool alive;

    public PlayerController playerController;


    public float aggroDistance = 6;

    private void Start()
    {
        alive = true;
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        pathfinder = new AStarPath();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        occupiedCell = mapManager.cells[(int)transform.position.x, (int)transform.position.y];
        Aggro = false;
        target = CheckAggro();
        if (target != null) { targetPosition = new Vector3Int(target.GetPosition().x, target.GetPosition().y); }
        mainManager = MainManager.Instance;
        audioSource = GameObject.Find("SFX Audio Source").GetComponent<AudioSource>();
        facingRight = true;
        
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
        if (playerController != null && occupiedCell != null)
        {
            if ((playerController.occupiedCell.GetPosition() - occupiedCell.GetPosition()).magnitude < aggroDistance)
            {
                Aggro = true;
                return playerController.occupiedCell;
            }
            else
            {
                Aggro = false;
               return occupiedCell;
                
            }
        }
        return null;

    }
    public virtual bool CheckRange()
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
        audioSource.PlayOneShot(moveAudio);
        path.Remove(path.First());
        var nextCell = path.First();
       
        var moveVector = nextCell == playerController.occupiedCell ? Vector3Int.zero : nextCell.GetPosition() - occupiedCell.GetPosition();
        
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




    public virtual void Act()
    {
        occupiedCell = mapManager.cells[(int)transform.position.x, (int)transform.position.y];
        target = CheckAggro();
        inRange = CheckRange();

        if (Aggro && HP > 0)
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

    public virtual void Attack()
    {
        playerController.audioSource.PlayOneShot(playerController.getHit);
        gameObject.GetComponent<Animator>().SetTrigger("attack");
        playerController.GetComponent<Animator>().SetTrigger("GetHit");
        
    }

    public void takeAHit()
    {
        audioSource.PlayOneShot(dieAudio);
        gameObject.GetComponent<Animator>().SetTrigger("GetHit");
        HP--;
        if (HP <= 0 && alive)
        {
            alive = false;
            occupiedCell.Desocupy();
            gameObject.GetComponent<Animator>().SetBool("Dead", true);
        }
    }

    public void resetHit()
    {
        gameObject.GetComponent<Animator>().ResetTrigger("GetHit");
    }

    public void die()
    {
        //Drop Score Points
        Destroy(gameObject);
    }

    public void resetAttack()
    {
        gameObject.GetComponent<Animator>().ResetTrigger("attack");
    }
}
