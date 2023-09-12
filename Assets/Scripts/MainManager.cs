
using System;
using Unity.VisualScripting;
using UnityEngine;



public enum State
{
    game,
    menu
}
public class MainManager : MonoBehaviour
{

    //Game Managing stuff
    public MainManager Instance;
    private State state = State.game;



    //Map Stuff
    private MapManager mapManager;
    public Grid grid;



    //Input Stuff
    InputActions inputActions;
    PlayerController playerController;


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
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        grid = GameObject.Find("Grid").GetComponent<Grid>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            mapManager.generateTerrain();
        }

    }


    private void assignInputCallbacks()
    {
        inputActions.BaseInput.DirectionalKeys.performed += ctx => handleDirectionalInput(ctx.ReadValue<Vector2>());
    }


    private void handleDirectionalInput(Vector2 direction)
    {

        if (state == State.game && checkCollision(new Vector2Int((int)direction.x, (int)direction.y)) && playerController.movementDone == true)
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




    //==============
    //AUX FUNCTIONS
    private bool checkCollision(Vector2Int direction)
    {
        if (mapManager.cells[mapManager.ocupiedCell.GetPosition().x + direction.x, mapManager.ocupiedCell.GetPosition().y + direction.y].IsWalkable() && (direction != Vector2Int.zero))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
