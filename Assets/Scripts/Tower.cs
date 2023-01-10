using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    /// <summary>
    /// This is the projectiles type
    /// </summary>
    [SerializeField]
    private string projectileType;

    [SerializeField]
    private float projectileSpeed;

    private Animator myAnimator;

    [SerializeField]
    private int damage;

    public int Price { get; set; }

    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
    }

    /// <summary>
    /// The tower's sprite renderer
    /// </summary>
    private SpriteRenderer mySpriteRenderer;

    /// <summary>
    /// The tower's current target
    /// </summary>
    private Monster target;

    public Monster Target
    {
        get { return target; }
    }

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    /// <summary>
    /// A queue of monsters that the tower can attack
    /// </summary>
    private Queue<Monster> monsters = new Queue<Monster>();

    private bool canAttack = true;

    private float attackTimer;

    [SerializeField]
    private float attackCooldown;

	// Use this for initialization
	void Awake()
    {
        myAnimator = transform.parent.GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        Attack();
	}

    /// <summary>
    /// Selects the tower
    /// </summary>
    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
    }

    /// <summary>
    /// Makes the tower attack a target
    /// </summary>
    private void Attack()
    {
        if (!canAttack)//If we can't attack
        {
            //Count how much time has passed since last attack
            attackTimer += Time.deltaTime;

            //If the time passed is higher than the cooldown, then we need to reset
            //and be able to attack again
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        //If we don't have a target and we have more targets in range
        if (target == null && monsters.Count > 0)
        {
            target = monsters.Dequeue();
            
        }
        if (target != null && target.IsActive)//If we have a target that is active
        {
            if (canAttack)//If we can attack then we shoot at the target
            {
                Shoot();

                canAttack = false;
            }
         
        }
        else if (monsters.Count > 0)
        {
            target = monsters.Dequeue();
        }
        if (target != null && !target.Alive || target!= null && !target.IsActive)
        {
            target = null;
        }

    }

    /// <summary>
    /// Makes the tower shoot
    /// </summary>
    private void Shoot()
    {
        //Gets a projectile from the object pool
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

        //Makes sure that the projectile is instantiated on the towers position
        projectile.transform.position = transform.position;

        projectile.Initialize(this);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster") //Adds new monsters to the queu when they enter the range
        {
            monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            target = null;
        }
    }
}
