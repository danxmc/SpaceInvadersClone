using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public float speed = 15.0f;		//	The speed at which the ship is moving
	public float padding = 1f;		//	To avoid hitting the edge of the screen
	public GameObject projectile;	//	The object we use to instantiate a laser beam
	public float projectileSpeed;	//	The speed of our laser beam
	public float firingRate = 0.2f;	//	How fast we can instantiate our laser beam
	public float health = 250f;		//	Our ships health
	public AudioClip fireSound;		//	Sound played whenever the player fires
    public Text text;

	private Rigidbody2D rgb;		//	A linkt to our beam rigid body

	float xMin;						//	Left boundary of our gamespace
	float xMax;						//	Right boundary of our gamespace

	// We initialize our controller
	void Start () {
		//	We find the WorldPoint coordinates of our scene, using the ViewportToWorldPoint method
		//	in our main camera.
		float distance = transform.position.z - Camera.main.transform.position.z;
		Vector3 leftmost = Camera.main.ViewportToWorldPoint(new Vector3(0,0,distance));
		Vector3 rightmost = Camera.main.ViewportToWorldPoint(new Vector3(1,0,distance));
        text.text = health.ToString();

		//	We user the WorldPoint coordinates of our scene to set the boundaries for our ship
		//	horizontal movement.  We use a padding so the ship never touches the screen edge.
		xMin = leftmost.x + padding;
		xMax = rightmost.x - padding;
	}

	//	This method fires a laser beam from the player to the enemies.
	void Fire () {
		//	We instantiate the laser bean and give it a positive velocity in the y axis.  We offset the
		//	beam's position 1 unit above our ship, because we do not want an instant collision between
		//	them.
		Vector3 offset = new Vector3(0, 1, 0);
		GameObject beam = Instantiate (projectile, transform.position + offset, Quaternion.identity) as GameObject;
		rgb = beam.GetComponent<Rigidbody2D> ();
		rgb.velocity = new Vector3 (0, projectileSpeed, 0);

		//	We play the fireSound Clip
		AudioSource.PlayClipAtPoint (fireSound, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		//	If the user presses the space bar, we instantiate a laser beam and set its speed at 
		//	the projectileSpeed value.  We use GetKeyDown method instead of the GetKey method because
		//	we do not want to instantiate several beams at a time if the user keeps the space bar
		//	pressed.  However, we use the InvokeRepeating method because do want to shoot several
		//  beams if the space bar remains pressed, but not all at once.
		if (Input.GetKeyDown (KeyCode.Space)) {
			InvokeRepeating ("Fire", 0.000001f, firingRate);
		}

		//	If the user stops pressing the space bar, we cancel the invoke to the Fire method.
		if (Input.GetKeyUp (KeyCode.Space)) {
			CancelInvoke ("Fire");
		}

		//  If the user presses the Left Arrow or Right Arrow keys, we transform the ship's
		//  position by the speed amount: negative if the user pressed the Left Arrow key,
		//  and positive if the user pressed the Right Arrow key.
		if (Input.GetKey (KeyCode.LeftArrow)) {
			//  transform.position += new Vector3 (-speed * Time.deltaTime, 0, 0);
			transform.position += Vector3.left * speed * Time.deltaTime;
		} else {
			if (Input.GetKey (KeyCode.RightArrow)) {
				//	transform.position += new Vector3 (speed * Time.deltaTime, 0, 0);
				transform.position += Vector3.right * speed * Time.deltaTime;
			}
		}

		//	We normalize the position of our ship, so it never goes off-screen.  We save our
		//  ship's current position in a Vector3 named pos.  Then, we use the Mathf.Clamp()
		//	method to make sure the x value is always between xMin and xMax.  Finally, we set
		//	our position using our new validated x value.
		Vector3 pos = transform.position;
		float newX = Mathf.Clamp (pos.x, xMin, xMax);
		transform.position = new Vector3 (newX, pos.y, pos.z);
	}

	//	Called everytime our object collides with a trigger collider
	void OnTriggerEnter2D(Collider2D collider) {
		//	We try to identify the object that collided with us as a projectile (laser beam).
		Projectile missile = collider.gameObject.GetComponent<Projectile> ();

		//	If our ship collided with a laser beam, we decrease our ship's health in the amount
		//	of damage set by the projectile.  If the ship's health is zero or less, then we destroy
		//	our ship.
		if (missile) {
			health -= missile.getDamage ();
			missile.Hit ();  	// The missile is destroyed upon collision with our ship.
            text.text = health.ToString();
            if (health <= 0) {
				Die ();
			}
		}
	}

	//	This method destroys the player's ship
	void Die() {
		LevelManager levelManager = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
		levelManager.LoadLevel ("Win Screen");
		Destroy (gameObject);
	}
}
