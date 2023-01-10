using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Monster target;

    private Tower parent;

    private Animator myAnimator;

	// Use this for initialization
	void Start ()
    {
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        MoveToTarget();
	}

    public void Initialize(Tower parent)
    {
        this.target = parent.Target;
        this.parent = parent;
    }

    private void MoveToTarget()
    {
        if (target != null && target.IsActive) //If the target isn't null and the target isn't dead
        {
            //Move towards the position
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * parent.ProjectileSpeed);

            //Calculates the direction of the projectile
            Vector2 dir = target.transform.position - transform.position;

            //Calculates the angle of the projectile
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //Sets the rotation based on the angle
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if (!target.IsActive)
        {
            GameManager.Instance.Pool.ReleaseObject(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            if (target.gameObject == other.gameObject)
            {
                target.TakeDamage(parent.Damage);

                GameManager.Instance.Pool.ReleaseObject(gameObject);
            }

            
        }

    }
}
