
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State
{
    game,
    waiting,
    menu,
    init
}

public enum Turn
{
    player,
    enviroment,
    enemies,
    waiting
}
public class MainManager : MonoBehaviour
{

    //Game Managing stuff
    public MainManager Instance;
    public State state;
    public Turn turn;


    //Map Stuff
    private MapManager mapManager;
    public Grid grid;



    //Input Stuff
    InputActions inputActions;
    PlayerController playerController;
    public UiManager uiManager;


    //DebugMode
    public bool DebugMode = false;
    public bool initialized = false;

   

    //shooting 
    [SerializeField] private GameObject bulletPrefab;


 

    private void Awake()
    {
        inputActions = new InputActions();

    }

    private void OnEnable()
    {
        inputActions.BaseInput.Enable();
        assignInputCallbacks();
    }

    private void OnDisable()
    {
        inputActions.BaseInput.Disable();
    }

    void Start()
    {
        if (Instance == null)
        {
            state = State.menu;
            turn = Turn.player;

            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        switch (state)
        {
            case State.init:

                initialize();
                mapManager.generateTerrain();

                state = State.game;

                break;

            case State.game:

                HandleDebugInput();

                switch (turn)
                {

                    case Turn.player:
                        

                        break;

                    case Turn.enviroment:


                        passTurn();
                        break;

                    case Turn.enemies:
                        foreach(var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                        {
                            
                             enemy.GetComponent<EnemyController>().Act();
                        }

                        passTurn();
                        break;   
                }

                break;

            case State.menu:
                if (SceneManager.GetActiveScene().name == "GameScene")
                {                
                    state = State.init;
                    turn = Turn.player;
                }
                break;

            case State.waiting:

                break;

        }
    }



    //========================
    //Input Handlers
    //========================
    private void HandleDebugInput()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            mapManager.generateTerrain();
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            DebugMode = !DebugMode;
        }

        if(Input.GetKeyUp(KeyCode.P)) 
        {
            passTurn();
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            playerController.gameObject.GetComponent<Animator>().SetTrigger("Shoot");
        }
    }
   

    public void enemyShoot(GameObject enemy)
    {
        var bullet = Instantiate(bulletPrefab, enemy.GetComponent<EnemyController>().occupiedCell.GetPositionCenter(), new Quaternion(0, 0, 0, 0));
        bullet.GetComponent<BulletController>().Target = playerController.gameObject;
        bullet.GetComponent<BulletController>().initialize();
    }


    private void handleDirectionalInput(Vector2 direction)
    {

        if ( turn == Turn.player &&
            checkCollision(new Vector2Int((int)direction.x, (int)direction.y)) && 
            playerController.movementDone == true)
        {
            if (state == State.game)
            {
                playerController.movementDone = false;
                if (direction.x > 0)
                {
                    playerController.moveEvent.Invoke(movementDirection.right);
                }
                else if (direction.x < 0)
                {
                    playerController.moveEvent.Invoke(movementDirection.left);
                }
                else if (direction.y > 0)
                {
                    playerController.moveEvent.Invoke(movementDirection.up);
                }
                else if (direction.y < 0)
                {
                    playerController.moveEvent.Invoke(movementDirection.down);
                }
            }

        }
    }




    //================
    //AUX FUNCTIONS
    //================
    private bool checkCollision(Vector2Int direction)
    {
        if (mapManager.cells[playerController.occupiedCell.GetPosition().x + direction.x, playerController.occupiedCell.GetPosition().y + direction.y].IsWalkable() && (direction != Vector2Int.zero))
        {
            return true;
        }
        else
        {
            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if(mapManager.cells[playerController.occupiedCell.GetPosition().x + direction.x, playerController.occupiedCell.GetPosition().y + direction.y] == enemy.GetComponent<EnemyController>().occupiedCell)
                {
                    playerController.GetComponent<Animator>().SetTrigger("Melee");
                    Attack(enemy.GetComponent<EnemyController>());
                    passTurn();

                }
            }
            return false;
        }
    }

    private void Attack(EnemyController enemy)
    {
        enemy.takeAHit();
    }

    private void initialize()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        
        initialized = true;
    }

    private void assignInputCallbacks()
    {
        inputActions.BaseInput.DirectionalKeys.performed += ctx => handleDirectionalInput(ctx.ReadValue<Vector2>());
    }

    public void passTurn()
    {
        switch (turn)
        {
            case Turn.player:

                StartCoroutine(passTurnWaitCoroutine());
                turn = Turn.enviroment;
                playerController.reduceO2();
                break;

            case Turn.enviroment:
            
                //StartCoroutine(passTurnWaitCoroutine());
                turn = Turn.enemies;
                break;

            case Turn.enemies:
               
                //StartCoroutine(passTurnWaitCoroutine());
                turn = Turn.player;
                break;

        }
    }

    private IEnumerator passTurnWaitCoroutine()
    {
        state = State.waiting;
        yield return new WaitForSeconds(0.2f);
        state = State.game;
    }

    internal void EndGame()
    {
        turn = Turn.player;
        state = State.menu;
        playerController.reset();
        SceneManager.LoadScene("MainMenu");
    }
}
