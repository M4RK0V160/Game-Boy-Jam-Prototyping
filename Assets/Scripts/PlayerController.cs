using System;
using System.Collections;
using System.Collections.Generic;
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

    public bool movementDone = true;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void move(movementDirection direction) 
    {
        switch (direction)
        {
            case movementDirection.up:
                transform.Translate(Vector2.up);
                mapManager.moveOccupiedCell(Vector2Int.up);
                break;
            case movementDirection.down:
                transform.Translate(Vector2.down);
                mapManager.moveOccupiedCell(Vector2Int.down);
                break;
            case movementDirection.left:
                transform.Translate(Vector2.left);
                mapManager.moveOccupiedCell(Vector2Int.left);
                break;
            case movementDirection.right:
                transform.Translate(Vector2.right);
                mapManager.moveOccupiedCell(Vector2Int.right);
                break;
        }   
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
    //=============
}

