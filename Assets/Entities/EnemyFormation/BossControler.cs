using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControler : MonoBehaviour {
    public GameObject projectile;           //	The object we use to instantiate the enemy's laser beam
    public float health = 150f;             //	The enemy's health
    public float projectileSpeed = 10f;     //	The speed of the enemy's laser beam
    public float shotsPerSeconds = 0.5f;    //	The number of shots per second our enemy shoots.
    public int scoreValue = 150;            //	Points obtained for destroying this ship
    public AudioClip fireSound;             //	Sound played whenever the enemy fires.
    public AudioClip deathSound;            //	Sound played whenever the enemy ship is destroyed.

    private Rigidbody2D rgb;                //	A link to the enemy's laser beam rigid body
    private ScoreKeeper scoreKeeper;        //	A link to the ScoreKeeper script

    // Use this for initialization
    void Start()
    {
        scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
    }

    // Update is called once per frame
    void Update()
    {
        //	We use the deltaTime and the shotsPerSecond properties to compute a shooting probability.
        float probability = shotsPerSeconds * Time.deltaTime;

        //	If a random generated value is less than the computed shooting probability, then the enemy
        //	ship shoots a laser beam.
        if (Random.value < probability)
        {
            Fire();
        }
    }

    //	This method fires a laser beam from the enemy to the player.
    void Fire()
    {
        //	We instantiate the laser bean and give it a negative velocity in the y axis.  We offset the
        //	beam's position 1 unit below the enemy0s ship, because we do not want an instant collision
        //	between them.
        //	Vector3 startPosition = transform.position + new Vector3 (0, -1, 0);
        //	GameObject missile = Instantiate (projectile, startPosition, Quaternion.identity) as GameObject;
        GameObject missile = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        rgb = missile.gameObject.GetComponent<Rigidbody2D>();
        rgb.velocity = new Vector2(0, -projectileSpeed);

        //	We play the fireSound Clip
        AudioSource.PlayClipAtPoint(fireSound, transform.position);
    }

    //	Called everytime our object collides with a trigger collider
    void OnTriggerEnter2D(Collider2D collider)
    {
        //	We try to identify the object that collided with the enemy's ship as a projectile (laser beam).
        Projectile missile = collider.gameObject.GetComponent<Projectile>();

        //	If the enemy's ship collided with a laser beam, we decrease the ship's health in the amount
        //	of damage set by the projectile.  If the ship's health is zero or less, then we destroy
        //	the ship.
        if (missile)
        {
            health -= missile.getDamage();
            missile.Hit();      // The missile is destroyed upon collision with our ship.
            if (health <= 0)
            {
                //	We call the Die() method;
                Die();
            }
        }
    }

    void Die()
    {
        //	We play the deathSound clip and then we destroy the object.
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
        Destroy(gameObject);
        //	We add the scoreValue to the score.
        scoreKeeper.Score(scoreValue);
        GameObject.Find("LevelManager").GetComponent<LevelManager>().LoadLevel("Win");
    }
}

