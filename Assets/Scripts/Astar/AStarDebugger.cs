using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarDebugger : MonoBehaviour
{

    private TileScript start, goal;

    [SerializeField]
    private Sprite blankTile;

    [SerializeField]
    private GameObject arrowPrefab;

    [SerializeField]
    private GameObject debugTilePrefab;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {

        //ClickTile();

        ////Generates a path when we click sapce
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    AStar.GetPath(start.GridPosition, goal.GridPosition);
        //}
	}

    /// <summary>
    /// Click a tile in the game
    /// </summary>
    private void ClickTile()
    {
        if (Input.GetMouseButtonDown(1)) //Craete a raycast if we click a tile
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                TileScript tmp = hit.collider.GetComponent<TileScript>();

                if (tmp != null)
                {
                    if (start == null)
                    {
                        start = tmp;
                        CreateDebugTile(start.WorldPosition, new Color32(255, 135, 0, 255));
                      
                    }
                    else if (goal == null)
                    {
                        goal = tmp;
                        CreateDebugTile(goal.WorldPosition, new Color32(255, 0, 0, 255));
                     
                    }
                }
            }
        }
    }

    /// <summary>
    /// Debugs the path, so that we can see what's going on
    /// </summary>
    /// <param name="openList"></param>
    public void DebugPath(HashSet<Node> openList, HashSet<Node> closedList, Stack<Node> path)
    {
        foreach (Node node in  openList)//Colors all the tiles blue so that we can see which tiles are in the open list
        {
            if (node.TileRef != start && node.TileRef != goal)
            {
                CreateDebugTile(node.TileRef.WorldPosition, Color.cyan, node);
            }

            //Points at the parent
            PointToParent(node, node.TileRef.WorldPosition);
           
        }

        foreach (Node node in closedList)//Colors all the tiles blue so that we can see which tiles are in the closed list
        {
            if (node.TileRef != start && node.TileRef != goal && !path.Contains(node))
            {
                CreateDebugTile(node.TileRef.WorldPosition, Color.blue,node);
            }

            //Points at the parent
            PointToParent(node, node.TileRef.WorldPosition);
        }

        foreach (Node node in path)
        {
            if (node.TileRef != start && node.TileRef != goal)
            {
                CreateDebugTile(node.TileRef.WorldPosition, Color.green, node);
            }
        }
    }

    /// <summary>
    /// Creates an arrow, that points at the parent
    /// </summary>
    /// <param name="node">Parent</param>
    /// <param name="position">world pos</param>
    private void PointToParent(Node node, Vector2 position)
    {
        if (node.Parent != null) //Checks if the node has a parent
        {
            GameObject arrow = (GameObject)Instantiate(arrowPrefab, position, Quaternion.identity);
            arrow.GetComponent<SpriteRenderer>().sortingOrder = 3;
            //Right
            if ((node.GridPosition.X < node.Parent.GridPosition.X) && (node.GridPosition.Y == node.Parent.GridPosition.Y))
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            //Top Right
            else if ((node.GridPosition.X < node.Parent.GridPosition.X) && (node.GridPosition.Y > node.Parent.GridPosition.Y))
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 45);
            }
            //UP
            else if ((node.GridPosition.X == node.Parent.GridPosition.X) && (node.GridPosition.Y > node.Parent.GridPosition.Y))
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 90);
            }
            //TOP LEFT
            else if ((node.GridPosition.X > node.Parent.GridPosition.X) && (node.GridPosition.Y > node.Parent.GridPosition.Y))
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 135);
            }
            //LEFT
            else if ((node.GridPosition.X > node.Parent.GridPosition.X) && (node.GridPosition.Y == node.Parent.GridPosition.Y))
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 180);
            }
            //Bottom left
            else if ((node.GridPosition.X > node.Parent.GridPosition.X) && (node.GridPosition.Y < node.Parent.GridPosition.Y))
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 225);
            }
            //Bottom
            else if ((node.GridPosition.X == node.Parent.GridPosition.X) && (node.GridPosition.Y < node.Parent.GridPosition.Y))
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 270);
            }
            //Bottom right
            else if ((node.GridPosition.X < node.Parent.GridPosition.X) && (node.GridPosition.Y < node.Parent.GridPosition.Y))
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 315);
            }




        }

    }

    /// <summary>
    /// Creates a debug tile in the world
    /// </summary>
    /// <param name="worldPos">The tile's world position</param>
    /// <param name="color">The color of the tile</param>
    /// <param name="node">The actual node</param>
    private void CreateDebugTile(Vector3 worldPos, Color32 color, Node node = null)
    {
        //Instantiates a debug tile
        GameObject debugTile = (GameObject)Instantiate(debugTilePrefab,worldPos,Quaternion.identity);

        if (node != null)//If the tile has a node
        {
            //Then we need to calculate all the values
            DebugTile tmp = debugTile.GetComponent<DebugTile>();

            tmp.G.text += node.G;
            tmp.H.text += node.H;
            tmp.F.text += node.F;
        }

        //Sets the color of the tile
        debugTile.GetComponent<SpriteRenderer>().color = color;
    }
}
