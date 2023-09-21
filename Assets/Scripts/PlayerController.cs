using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum movementDirection
{
    up, down, left, right
}
public class PlayerController : MonoBehaviour
{

    public UnityEvent<movementDirection> moveEvent;
    public MapManager mapManager;
    public MainManager mainManager;

    public Cell occupiedCell;

    public bool movementDone = true;

    private AudioSource audioSource;

    public int HP;
    public int O2;
    public int O2Multiplier;
    private int O2MultiplierBack;

    public bool facingRight;


    [SerializeField] private AudioClip getHit;

    [SerializeField] private AudioClip step1;
    [SerializeField] private AudioClip step2;

    [SerializeField] private GameObject bulletPrefab;

    private bool step;
    private void OnEnable()
    {
        addEventListeners();
    }

    private void OnDisable()
    {
        removeEventListeners();
    }

    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>().Instance;
        audioSource = gameObject.GetComponent<AudioSource>();
        O2MultiplierBack = 0;
        O2MultiplierBack += O2Multiplier;
        facingRight = true;
    }

    internal void reset()
    {
        HP = 4;
        O2 = 4;
        facingRight = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            gameObject.GetComponent<Animator>().SetTrigger("Die");
        }

        
    }

    private void move(movementDirection direction) 
    {
        if (step)
        {
            audioSource.PlayOneShot(step1);
            step = false;
        }
        else
        {
            audioSource.PlayOneShot(step2);
            step = true;
        }
        if (O2 <= 0)
        {
            takeAHit();
        }

        switch (direction)
        {
            case movementDirection.up:
                transform.Translate(Vector2.up);
                mapManager.moveOccupiedCell(Vector2Int.up);
                mainManager.passTurn();
                break;

            case movementDirection.down:
                transform.Translate(Vector2.down);
                mapManager.moveOccupiedCell(Vector2Int.down);
                mainManager.passTurn();
                break;
            case movementDirection.left:
                transform.Translate(Vector2.left);
                mapManager.moveOccupiedCell(Vector2Int.left);
                mainManager.passTurn();
                if (facingRight)
                {
                    flip();
                    facingRight = false;
                }
                break;
                
            case movementDirection.right:
                transform.Translate(Vector2.right);
                mapManager.moveOccupiedCell(Vector2Int.right);
                mainManager.passTurn();
                if (!facingRight)
                {
                    flip();
                    facingRight = true;
                }
                
                break;
        }   
    }

    private void flip()
    {
        var facing = gameObject.GetComponent<SpriteRenderer>().flipX;
        gameObject.GetComponent<SpriteRenderer>().flipX = !facing;
    }





    //==============
    //AUX FUNCTIONS:
    private void addEventListeners()
    {
        moveEvent.AddListener(move);
    }

    private void removeEventListeners()
    {
        moveEvent.RemoveAllListeners();
    }

    internal void takeAHit()
    {
        HP--;
        gameObject.GetComponent<Animator>().ResetTrigger("GetHit");
        audioSource.PlayOneShot(getHit);
    }

    private void shoot()
    {
        gameObject.GetComponent<Animator>().ResetTrigger("Shoot");
        var bullet = Instantiate(bulletPrefab, occupiedCell.GetPositionCenter(), new Quaternion(0, 0, 0, 0));
        bullet.GetComponent<BulletController>().initialize();
        bullet.GetComponent<BulletController>().shootingAtEnemy = true;
    }

    private void pickup()
    {
        gameObject.GetComponent<Animator>().ResetTrigger("PickUp");
    }

    private void die()
    {
        mainManager.EndGame();
    }

    private void melee()
    {
        gameObject.GetComponent<Animator>().ResetTrigger("Melee");
    }
    internal void reduceO2()
    {
       if(O2Multiplier == 0)
        {
            O2Multiplier += O2MultiplierBack;
            O2--;
        }
        else
        {
            O2Multiplier--;
        }


    }

    

    //=============
}

