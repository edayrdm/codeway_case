using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// This class handles the level generation
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    /// <summary>
    /// An array of tilePrefabs, these are used for creating the tiles in the game
    /// </summary>
    [SerializeField]
    private GameObject[] tilePrefabs;

    /// <summary>
    /// A reference to the cameramovement script
    /// </summary>
    [SerializeField]
    private CameraMovement cameraMovement;

    /// <summary>
    /// The maps transform, this is needed for adding new tiles
    /// </summary>
    [SerializeField]
    private Transform map;

    /// <summary>
    /// Spawn points for the portals
    /// </summary>
    private Point blueSpawn, redSpawn;

    /// <summary>
    /// Prefab for spawning the blue portal
    /// </summary>
    [SerializeField]
    private GameObject bluePortalPrefab;

    /// <summary>
    /// Prefab for spawning the red portal
    /// </summary>
    [SerializeField]
    private GameObject redPortalPrefab;

    public Portal BluePortal { get; set; }

    /// <summary>
    /// The full path from start to goal
    /// </summary>
    private Stack<Node> fullPath;

    private Point mapSize;

    /// <summary>
    /// A dictionary that conatins all tiles in our game
    /// </summary>
    public Dictionary<Point, TileScript> Tiles { get; set; }

    /// <summary>
    /// A property for returning the size of a tile
    /// </summary>
    public float TileSize
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    /// <summary>
    /// Property for accessing the path
    /// </summary>
    public Stack<Node> Path
    {
        get
        {
            if (fullPath == null)
            {
                GeneratePath();
            }

            return new Stack<Node>(new Stack<Node>(fullPath));
        }
    }

    public Point BlueSpawn
    {
        get
        {
            return blueSpawn;
        }
    }

    // Use this for initialization
    void Start()
    {
        //Executes the create level function
        CreateLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Creates our level
    /// </summary>
    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>();

        //A tmp instantioation of the tile map, we will use a text document to load this later.
        string[] mapData = ReadLevelText();

        mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);

        //Calculates the x map size
        int mapX = mapData[0].ToCharArray().Length;

        //Calculates the y map size
        int mapY = mapData.Length;

        Vector3 maxTile = Vector3.zero;

        //Calculates the world start point, this is the top left corner of the screen
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));

        for (int y = 0; y < mapY; y++) //The y positions
        {
            char[] newTiles = mapData[y].ToCharArray(); //Gets all the tiles, that we need to place on the current horizontal line

            for (int x = 0; x < mapX; x++) //The x positions
            {
                //Places the tile in the world
                PlaceTile(newTiles[x].ToString(), x, y, worldStart);
            }
        }

        maxTile = Tiles[new Point(mapX - 1, mapY - 1)].transform.position;

        //Sets the camera limits to the max tile position
        cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));

        SpawnPortals();

    }

    /// <summary>
    /// Places a tile in the gameworld
    /// </summary>
    /// <param name="tileType">The type of tile to palce for example 0</param>
    /// <param name="x">x position of the tile</param>
    /// <param name="y">y position of the tile</param>
    /// <param name="worldStart">The world start position</param>
    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        //Parses the tiletype to an int, so that we can use it as an indexer when we create a new tile
        int tileIndex = int.Parse(tileType);

        //Creates a new tile and makes a reference to that tile in the newTile variable
        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();

        //Uses the new tile variable to change the position of the tile
        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0),map);


    }

    /// <summary>
    /// Reads the level text
    /// </summary>
    /// <returns>A string array with indicators of the tiles to place</returns>
    private string[] ReadLevelText()
    {
        //Loads the text asset from the resources folder
        TextAsset bindata = Resources.Load("Level") as TextAsset;

        //Get the string
        string data = bindata.text.Replace(Environment.NewLine, string.Empty);

        //Splits the string into an array
        return data.Split('-');
    }

    /// <summary>
    /// Spawns the portals in the game
    /// </summary>
    private void SpawnPortals()
    {
        //Spawns the blue portal
        blueSpawn = new Point(0, 0);
        GameObject tmp = (GameObject)Instantiate(bluePortalPrefab, Tiles[BlueSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
        BluePortal = tmp.GetComponent<Portal>();
        BluePortal.name = "BluePortal";

        //Spawns the red portal
        redSpawn = new Point(11, 6);
        Instantiate(redPortalPrefab, Tiles[redSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
    }

    public bool InBounds(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y < mapSize.Y;
    }

    /// <summary>
    /// Generates a path with the AStar algorithm
    /// </summary>
    public void GeneratePath()
    {
        //Generates a path from start to finish and stores it in fullPath
        fullPath = AStar.GetPath(BlueSpawn, redSpawn);
    }
}
