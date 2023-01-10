using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : MonoBehaviour
{

    /// <summary>
    /// The units movement speed
    /// </summary>
    [SerializeField]
    private float speed;

    /// <summary>
    /// This stack contains the path that the Unit can walk on
    /// This path should be generated with the AStar algorithm
    /// </summary>
    private Stack<Node> path;

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Stat health;

    public float MyHealth
    {
        get { return health.CurrentValue; }
    }

    public void SetMyHealth(float myHealth)
    {
        this.health.CurrentValue = myHealth;
    }

    public void SetMyMaxHealth(float myHealth)
    {
        this.health.MaxVal = myHealth;
    }

    public float MyMaxHealth
    {
        get { return health.MaxVal; }
    }



    public bool Alive
    {
        get { return health.CurrentValue > 0; }
    }

    /// <summary>
    /// The Unit's grid position
    /// </summary>
    public Point GridPosition { get; set; }

    /// <summary>
    /// Indicates if the Unit is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The unit's next destination
    /// </summary>
    private Vector3 destination;

    private void Awake()
    {
        //Sets up references to the components

        spriteRenderer = GetComponent<SpriteRenderer>();
        health.Initialize();

        //Debug.Log("curr " + health.CurrentValue);
    }

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// Spawns the monster in our world
    /// </summary>
    public void Spawn(int health)
    {
        transform.position = LevelManager.Instance.BluePortal.transform.position;
        this.health.Bar.Reset();
        this.health.MaxVal = health;
        this.health.CurrentValue = this.health.MaxVal;

        //Starts to scale the monsters
        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1), false));

        //Sets the monsters path
        SetPath(LevelManager.Instance.Path);
    }

    /// <summary>
    /// Scales a monster up or down
    /// </summary>
    /// <param name="from">start scale</param>
    /// <param name="to">end scale</param>
    /// <returns></returns>
    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {
        //The scaling progress
        float progress = 0;

        //As long as the progress is les than 1, then we need to keep scaling
        while (progress <= 1)
        {
            //Scales themonster
            transform.localScale = Vector3.Lerp(from, to, progress);
            progress += Time.deltaTime;
            yield return null;
        }

        //Makes sure that is has the correct scale after scaling
        transform.localScale = to;

        IsActive = true;

        if (remove)
        {
            Release();
        }
    }

    /// <summary>
    /// Makes the unity move along the given path
    /// </summary>
    public void Move()
    {
        if (IsActive)
        {
            //Move the unit towards the next destination
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            //Checks if we arrived at the destination
            if (transform.position == destination)
            {
                //If we have a path and we have more nodes, then we need to keep moving
                if (path != null && path.Count > 0)
                {
                    //Sets the new gridPosition
                    GridPosition = path.Peek().GridPosition;

                    //Sets a new destination
                    destination = path.Pop().WorldPosition;
                }
            }
        }

    }

    /// <summary>
    /// Gives the Unit a path to walk on
    /// </summary>
    /// <param name="newPath">The unit's new path</param>
    /// <param name="active">Indicates if the unit is active</param>
    public void SetPath(Stack<Node> newPath)
    {
        if (newPath != null) //If we have a path
        {
            //Sets the new path as the current path
            this.path = newPath;

            //Sets the new gridPosition
            GridPosition = path.Peek().GridPosition;

            //Sets a new destination
            destination = path.Pop().WorldPosition;
        }

    }


    /// <summary>
    /// When the monster collides with something
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "RedPortal")//If we collide with the red portal
        {
            //Start scaling the monster down
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));

            //Plays the portal animation
            other.GetComponent<Portal>().Open();

            GameManager.Instance.Lives--;
        }

        if (other.tag == "Tile")
        {
            spriteRenderer.sortingOrder = other.GetComponent<TileScript>().GridPosition.Y;
        }
    }

    /// <summary>
    /// Releases a monster from the game into the object pool
    /// </summary>
    public void Release()
    {
        //Makes sure that it isn't active
        IsActive = false;

        //Makes sure that it has the correct start position
        GridPosition = LevelManager.Instance.BlueSpawn;

        //Removes the monster from the game
        GameManager.Instance.RemoveMonster(this);

        //Releases the object in the object pool
        GameManager.Instance.Pool.ReleaseObject(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (IsActive)
        {
            health.CurrentValue -= damage;

            if (health.CurrentValue <= 0)
            {
                GameManager.Instance.Currency += 5;

                GameManager.Instance.Scores++;

                GameManager.Instance.RemoveMonster(this);
                GameManager.Instance.Pool.ReleaseObject(gameObject);
                IsActive = false;

                GetComponent<SpriteRenderer>().sortingOrder--;

            }
        }

    }


}