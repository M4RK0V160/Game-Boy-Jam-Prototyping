
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;



public class room
{
    public HashSet<Cell> cells;
    public string ID;

    public room(string ID)
    {
        this.ID = ID;
        cells = new HashSet<Cell>();
    }
    override public string ToString()
    {
        return ("ID: " + ID + " | Size: " + cells.Count);
    }
}

public class MapManager : MonoBehaviour
{

    private MainManager mainManager;

    [Header("Map Dimensions")]
    [SerializeField] public int mapWidth;
    [SerializeField] public int mapHeight;
    public Tilemap map;
    public Tilemap ColliderMap;



    [Header("Generation Parameters")]
    [Header("0-10")][SerializeField] float threshold;
    public float scale = 1.0F;

    [Space(20)]


    [Header("Tiles")]
    [SerializeField] private Tile[] Walls;
    [SerializeField] public Tile[] Floors;
    [SerializeField] public Tile Red;
    [SerializeField] public Tile[] FloodTiles;





    public Cell[,] cells;
    public List<room> rooms;

    public HashSet<Cell> floorCells;
    private PlayerController playerController;
    public GameObject loadingScreen;
    public GameObject endScreen;
    public GameObject Canvas;

    [Header("Prefabs")]
    public GameObject[] Enemies;
    public int enemyFactor;

    public GameObject O2prefab;
    public GameObject HealthPrefab;

    [SerializeField] int O2ammount;
    [SerializeField] int HealthAmmount;

    // The origin of the sampled area in the perlin noise plane, randomized each generation to get a diferent map
    float x0;
    float y0;
    public GameObject[] components;

    private void Awake()
    {

        cells = new Cell[mapWidth, mapHeight];

        floorCells = new HashSet<Cell>();

        rooms = new List<room>();

        map = GameObject.Find("Grid").transform.Find("map").GetComponent<Tilemap>();
        ColliderMap = GameObject.Find("Grid").transform.Find("ColliderMap").GetComponent<Tilemap>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        mainManager = MainManager.Instance;
        loadingScreen = GameObject.Find("Loading Screen");
        endScreen = GameObject.Find("End Screen");
        Canvas = GameObject.Find("Canvas");


    }

    public void Update()
    {

    }

    public void generateTerrain()
    {

        //do required initial steps 
        InitTerrainGeneration();


        //iterate mapWidth and Height
        float y = 0.0F;
        while (y < mapHeight)
        {
            float x = 0.0F;
            while (x < mapWidth)
            {
                //exclude edges
                if (x < 7.0F || x > mapWidth - 8 || y < 7.0F || y > mapHeight - 8)
                {
                    map.SetTile(new Vector3Int((int)x, (int)y), Walls[UnityEngine.Random.Range(0, Walls.Length)]);
                    cells[(int)x, (int)y] = new Cell(new Vector2Int((int)x, (int)y), true, false);
                }
                else
                {
                    //calculate the noise value for the corresponding cell and decide if its ground or not.
                    CalcNoise(x, y);
                }
                x++;
            }
            y++;
        }
        // add neighbours to the cells
        for (int y2 = 2; y2 < mapHeight - 2; y2++)
        {
            for (int x2 = 2; x2 < mapWidth - 2; x2++)
            {
                FindNeighbors(cells[x2, y2]);
            }
        }

        FloodRooms();

        placePlayer();
        placeEnemies(enemyFactor);

        placeO2();
        placeHealth();

        placeComponents();
        loadingScreen.SetActive(false);
        endScreen.SetActive(false);
        Canvas.gameObject.SetActive(true);
        mainManager.uiManager = GameObject.Find("Canvas").GetComponent<UiManager>();
        mainManager.uiManager.initialize();

    }

    private void placeComponents()
    {
        var ComponentFloorCells = new HashSet<Cell>();
        foreach (Cell cell in floorCells)
        {
            if (Vector3Int.Distance(playerController.occupiedCell.GetPosition(), cell.GetPosition()) > 10)
            {
                ComponentFloorCells.Add(cell);
            }
        }
        foreach (GameObject component in components)
        {
            if (ComponentFloorCells.Count <= 0)
            {
                break;
            }
            var instance = Instantiate(component, new Vector3(-10, -10), new Quaternion(0, 0, 0, 0)).GetComponent<ComponentController>();

            var componentCell = ComponentFloorCells.ElementAt(UnityEngine.Random.Range(0, ComponentFloorCells.Count - 1));
            instance.gameObject.transform.position = componentCell.GetPositionCenter();
            foreach (Cell cell in floorCells)
            {
                if (Vector3Int.Distance(cell.GetPosition(), componentCell.GetPosition()) < 8)
                {

                    ComponentFloorCells.Remove(cell);

                }
            }
        }
    }




    /* TODO
      private room FillSmallRooms()
      {
         Tuple<int,room>[] roomCellCounts = new Tuple<int, room>[rooms.Count];

          int count = 0;
          foreach(room room in rooms)
          {
              roomCellCounts[count] = new Tuple<int,room>(room.cells.Count, room);
              count++;
          }
          int max = 0;
          int indexOfBigRoom = 0;
          for(int i = 0; i < roomCellCounts.Length; i++)
          {
              if (roomCellCounts[i].Item1 > max)
              {
                  fillInRoom(roomCellCounts[indexOfBigRoom].Item2);
                  indexOfBigRoom = i;
                  max = 0;
                  max += roomCellCounts[i].Item1;
              }
          }
          Debug.Log(max);
          return roomCellCounts[indexOfBigRoom].Item2;
      }

      private void fillInRoom(room room)
      {
          foreach(Cell cell in room.cells)
          {
              var x = cell.GetPosition().x;
              var y = cell.GetPosition().y;
              cells[(int)x, (int)y] = new Cell(new Vector2Int(x, y), true, true);
              map.SetTile(new Vector3Int(x, y), Walls[UnityEngine.Random.Range(0, Walls.Length)]);
          }
      }
    */


    private void placeEnemies( int enemyFactor )
    {
        var EnemyfloorCells = new HashSet<Cell>();
        foreach (Cell cell in floorCells)
        {
            if (Vector3Int.Distance(playerController.occupiedCell.GetPosition(), cell.GetPosition()) > 6)
            {
                EnemyfloorCells.Add(cell);
            }
        }
        for (int i = 0; i < floorCells.Count /enemyFactor; i++)
        {
            if (EnemyfloorCells.Count <= 0)
            {
                break;
            }
            var enemy = Instantiate(Enemies[UnityEngine.Random.Range(0,2)], new Vector3(-10, -10), new Quaternion(0, 0, 0, 0)).GetComponent<EnemyController>();

            var Enemycell = EnemyfloorCells.ElementAt(UnityEngine.Random.Range(0, EnemyfloorCells.Count - 1));
            enemy.gameObject.transform.position = Enemycell.GetPositionCenter();

            foreach (Cell cell in floorCells)
            {
                if (Vector3Int.Distance(cell.GetPosition(), Enemycell.GetPosition()) < 8)
                {
                    
                        EnemyfloorCells.Remove(cell);
                    
                }
            }
        }
    }

    private void placePlayer()
    {
        //find a tile to start the player at
        playerController.occupiedCell = floorCells.ElementAt(UnityEngine.Random.Range(0, floorCells.Count - 1));
        //move the player to the starting cell
        playerController.occupiedCell.Ocupy();
        playerController.gameObject.transform.position = new Vector3(playerController.occupiedCell.GetPosition().x + 0.5f, playerController.occupiedCell.GetPosition().y + 0.5f);
    }


    private void placeO2()
    {
        var O2floorCells = new HashSet<Cell>();
        foreach (Cell cell in floorCells)
        {
            O2floorCells.Add(cell);
        }
        for (int i = 0; i < floorCells.Count/12; i++)
        {
            if(O2floorCells.Count <= 0)
            {
                break;
            }
            var O2 = Instantiate(O2prefab, new Vector3(-10, -10), new Quaternion(0, 0, 0, 0)).GetComponent<O2>();

            var O2cell = O2floorCells.ElementAt(UnityEngine.Random.Range(0, O2floorCells.Count - 1));
            O2.gameObject.transform.position = O2cell.GetPositionCenter();
           
            foreach (Cell cell in floorCells)
            {
                if (Vector3Int.Distance(cell.GetPosition(), O2cell.GetPosition()) < 6)
                {
                    if (O2floorCells.Contains(cell))
                    {
                        O2floorCells.Remove(cell);
                    }
                }
            }

        }
    }

    private void placeHealth()
    {
        var HPfloorCells = new HashSet<Cell>();
        foreach (Cell cell in floorCells)
        {
            HPfloorCells.Add(cell);
        }
        for (int i = 0; i < floorCells.Count / 12; i++)
        {
            if (HPfloorCells.Count <= 0)
            {
                break;
            }
            var HP = Instantiate(HealthPrefab, new Vector3(-10, -10), new Quaternion(0, 0, 0, 0)).GetComponent<Health>();

            var HPcell = HPfloorCells.ElementAt(UnityEngine.Random.Range(0, HPfloorCells.Count - 1));
            HP.gameObject.transform.position = HPcell.GetPositionCenter();
            
            foreach (Cell cell in floorCells)
            {
                if (Vector3Int.Distance(cell.GetPosition(), HPcell.GetPosition()) < 6)
                {
                    if (HPfloorCells.Contains(cell))
                    {
                        HPfloorCells.Remove(cell);
                    }
                }
            }

        }
    }



    public void InitTerrainGeneration()
    {
        floorCells.Clear();
        rooms.Clear();
        loadingScreen.SetActive(false);
        endScreen.SetActive(false);
        Canvas.SetActive(false);

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
        foreach (var O2 in GameObject.FindGameObjectsWithTag("O2"))
        {
            Destroy(O2);
        }
        foreach (var HP in GameObject.FindGameObjectsWithTag("HP"))
        {
            Destroy(HP);
        }
        //threshold max Value is 10, as its meant to truncate at a point bwtween 0.4 and 0.5
        if (Mathf.Abs(threshold) > 10)
        {
            threshold = 10;
        }

        //randomize starting point in the perlin noise 1.0 by 1.0 plane 
        x0 = UnityEngine.Random.value * scale;
        y0 = UnityEngine.Random.value * scale;


        //initially fill the map with black cells
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                cells[(int)x, (int)y] = new Cell(new Vector2Int(x, y), true, true);
                map.SetTile(new Vector3Int(x, y), Walls[UnityEngine.Random.Range(0, Walls.Length)]);

            }
        }
        ColliderMap.ClearAllTiles();
    }


    private void CalcNoise(float x, float y)
    {

        //using the origin coordinates and the scale value compute the real grid position of the cell
        float xCoord = x0 + x / mapWidth * scale;
        float yCoord = y0 + y / mapHeight * scale;

        //use the threshold value tu truncate the noise
        if (Mathf.PerlinNoise(xCoord, yCoord) > (0.4f + (threshold / 100)))
        {
            //set the tile
            map.SetTile(new Vector3Int((int)x, (int)y), Floors[UnityEngine.Random.Range(0, Floors.Length)]);
            //create the asociated cell
            cells[(int)x, (int)y] = new Cell(new Vector2Int((int)x, (int)y), false, false);
            floorCells.Add(cells[(int)x, (int)y]);

        }
        else
        {
            ColliderMap.SetTile(new Vector3Int((int)x, (int)y), FloodTiles[4]);
        }

    }

    private void FindNeighbors(Cell cell)
    {
        //|-1,-1| 0,-1| +1,-1|

        //|-1, 0| 0, 0| +1, 0|

        //|-1,+1| 0,+1| +1,+1|


        /*| -1,-1 |*/
        //if (cells[cell.GetPosition().x - 1, cell.GetPosition().y - 1].IsWalkable()) cell.AddNeighbor(cells[cell.GetPosition().x - 1, cell.GetPosition().y - 1]);

        /*|  0,-1 |*/
        if (cells[cell.GetPosition().x, cell.GetPosition().y - 1].IsWalkable()) cell.AddNeighbor(cells[cell.GetPosition().x, cell.GetPosition().y - 1]);

        /*| +1,-1 |*/
        //if (cells[cell.GetPosition().x + 1, cell.GetPosition().y - 1].IsWalkable()) cell.AddNeighbor(cells[cell.GetPosition().x + 1, cell.GetPosition().y - 1]);

        /*| -1, 0 |*/
        if (cells[cell.GetPosition().x - 1, cell.GetPosition().y].IsWalkable()) cell.AddNeighbor(cells[cell.GetPosition().x - 1, cell.GetPosition().y]);

        /*| +1, 0 |*/
        if (cells[cell.GetPosition().x + 1, cell.GetPosition().y].IsWalkable()) cell.AddNeighbor(cells[cell.GetPosition().x + 1, cell.GetPosition().y]);

        /*| -1,+1 |*/
        // if (cells[cell.GetPosition().x - 1, cell.GetPosition().y + 1].IsWalkable()) cell.AddNeighbor(cells[cell.GetPosition().x - 1, cell.GetPosition().y + 1]);

        /*|  0,+1 |*/
        if (cells[cell.GetPosition().x, cell.GetPosition().y + 1].IsWalkable()) cell.AddNeighbor(cells[cell.GetPosition().x, cell.GetPosition().y + 1]);

        /*| +1,+1 |*/
        //  if (cells[cell.GetPosition().x + 1, cell.GetPosition().y + 1].IsWalkable()) cell.AddNeighbor(cells[cell.GetPosition().x + 1, cell.GetPosition().y + 1]);
    }



    internal void moveOccupiedCell(Vector2Int direction)
    {

        if (mainManager.DebugMode)
        {
            map.SetTile(new Vector3Int(playerController.occupiedCell.GetPosition().x, playerController.occupiedCell.GetPosition().y), Floors[UnityEngine.Random.Range(0, Floors.Length)]);
        }

        playerController.occupiedCell.Desocupy();
        playerController.occupiedCell = cells[playerController.occupiedCell.GetPosition().x + direction.x, playerController.occupiedCell.GetPosition().y + direction.y];

        if (mainManager.DebugMode)
        {
            map.SetTile(new Vector3Int(playerController.occupiedCell.GetPosition().x, playerController.occupiedCell.GetPosition().y), Red);
        }
        playerController.occupiedCell.Ocupy();
        playerController.movementDone = true;
    }

    private void FloodRooms()
    {
        rooms.Clear();
        List<Cell> openList = new List<Cell>();

        foreach (Cell cell in cells)
        {
            if (cell.IsWalkable())
            {
                openList.Add(cell);
            }
        }

        while (openList.Count > 0)
        {

            room room = new room("room" + rooms.Count);
            HashSet<Cell> workingSet = new HashSet<Cell>();

            Cell initialCell = openList[UnityEngine.Random.Range(0, openList.Count - 1)];
            openList.Remove(initialCell);
            room.cells.Add(initialCell);
            var roomFlooded = false;

            while (roomFlooded == false)
            {
                var initialCount = room.cells.Count;
                foreach (Cell cell in room.cells)
                {
                    workingSet.UnionWith(cell.GetNeighbors());
                }

                room.cells.UnionWith(workingSet);

                foreach (Cell cell in room.cells)
                {
                    openList.Remove(cell);
                }
                var passCount = room.cells.Count;
                if (passCount == initialCount)
                {
                    rooms.Add(room);
                    roomFlooded = true;

                }
            }
        }


        if (mainManager.DebugMode)
        {
            List<int> indexArray = new List<int>();
            for (int i = 0; i < FloodTiles.Length; i++)
            {
                indexArray.Add(i);
            }
            int tileIndex = 0;
            foreach (var room in rooms)
            {
                if (indexArray.Count == 0)
                {
                    for (int i = 0; i < FloodTiles.Length; i++)
                    {
                        indexArray.Add(i);
                    }
                }
                else
                {
                    tileIndex = indexArray.ElementAt<int>(UnityEngine.Random.Range(0, indexArray.Count));
                    indexArray.Remove(tileIndex);
                }

                foreach (Cell cell in room.cells)
                {
                    map.SetTile(new Vector3Int(cell.GetPosition().x, cell.GetPosition().y), FloodTiles[tileIndex]);
                }
            }
        }
    }



}