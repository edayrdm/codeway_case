using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// This script is used for all tiles in the game
/// </summary>
public class TileScript : MonoBehaviour
{
    /// <summary>
    /// The tiles gris position
    /// </summary>
    public Point GridPosition { get; private set; }

    public bool IsEmpty { get; set; }

    private Tower myTower;

    /// <summary>
    /// the color of the tile, when its full, this is used while hovering the tile with the mouse
    /// </summary>
    private Color32 fullColor = new Color32(255, 118, 118, 255);

    /// <summary>
    /// The color of the tile when is empty, this is used when hover the tile with the mouse
    /// </summary>
    private Color32 emptyColor = new Color32(96, 255, 90, 255);

    private SpriteRenderer spriteRenderer;

    public bool Walkable { get; set; }

    public bool Debugging { get; set; }

    /// <summary>
    /// The tile's center world position
    /// </summary>
    public Vector2 WorldPosition
    {
        get
        {
            return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 2));
        }
    }

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Sets up the tile, this is an alternative to a constructor
    /// </summary>
    /// <param name="gridPos">The tiles grid position</param>
    /// <param name="worldPos">The tiles world postion</param>
    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        Walkable = true;
        IsEmpty = true;
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    /// <summary>
    /// Mouseover, this is executed when the player mouse over the tile
    /// </summary>
    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
        {
            if (IsEmpty && !Debugging)//Colors the tile green
            {
                ColorTile(emptyColor);
            }
            if (!IsEmpty && !Debugging)//Colors the tile red
            {
                ColorTile(fullColor);
            }
            else if (Input.GetMouseButtonDown(0))//Places a tower if there is room on the tile
            {
                PlaceTower();
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn == null && Input.GetMouseButtonDown(0))
        {
            if (myTower != null)
            {
                GameManager.Instance.SelectTower(myTower);
            }
            else
            {
                GameManager.Instance.DeselectTower();
            }
        }

    }

    private void OnMouseExit()
    {
        if (!Debugging)
        {
            ColorTile(Color.white);
        }
        
    }

    /// <summary>
    /// Places a tower on the tile
    /// </summary>
    private void PlaceTower()
    {

        //Creates the tower
        GameObject tower = (GameObject)Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, transform.position, Quaternion.identity);

        //Set the sorting layer order on the tower
        tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y;

        //Sets the tile as transform parent to the tower
        tower.transform.SetParent(transform);

        tower.name = GameManager.Instance.ClickedBtn.TowerPrefab.name;


        this.myTower = tower.transform.GetChild(0).GetComponent<Tower>(); ;

        //Makes sure that it isn't empty
        IsEmpty = false;

        //Sets the color back to white
        ColorTile(Color.white);

        Walkable = false;

        //Check the if path is created succesfully
        Stack<Node> fullPath = AStar.GetPath(new Point(0, 0), new Point(11, 6));
        if (fullPath.Count == 0)
        {
            Debug.Log("tower cannot be placed, try another spot");
            Destroy(tower);
            this.myTower = null;

            //Makes sure that it isn't empty
            IsEmpty = true;

            //Sets the color back to white
            ColorTile(Color.white);

            Walkable = true;

        }
        else
        {
            Debug.Log("its okay " + fullPath);

            myTower.Price = GameManager.Instance.ClickedBtn.Price;

            //Buys the tower
            GameManager.Instance.BuyTower();
        }

    }

    /// <summary>
    /// Sets the color on the tile
    /// </summary>
    /// <param name="newColor"></param>
    private void ColorTile(Color newColor)
    {
        //Sets the color on the tile
        spriteRenderer.color = newColor;
    }
}
