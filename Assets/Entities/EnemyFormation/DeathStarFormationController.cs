using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStarFormationController : MonoBehaviour {
	public GameObject enemyPrefab;		//	GameObject to instantiate the enemy's ships
	public float width = 10f;			//	Width to draw the enemy's formation gizmo
	public float height = 5f;			//	Height to draw the enemy's formation gizmo
	public float speed = 5f;			//	Speed of the enemy's ships horizontal movement.
    public Vector3 deathPosition;

	private bool movingRight = true;	//	Flag to know if the ship is moving rightwards or lftwards
	private float xMax;					//	Right boundary of our gamespace
	private float xMin;					//	Left boundary of our gamespace
    private bool one = false;           //  Boolean to only spawn one boss
    private int bossAppeared = -1;
    private GameObject boss;

	// Use this for initialization
	void Start () {
		//	We find the WorldPoint coordinates of our scene, using the ViewportToWorldPoint method
		//	in our main camera.
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		Vector3 leftBoundary = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera));
		Vector3 rightBoundary = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera));

		//	We user the WorldPoint coordinates of our scene to set the boundaries for the enemy's formation
		//	horizontal movement.  We use a padding so the ship never touches the screen edge.
		xMax = rightBoundary.x;
		xMin = leftBoundary.x;

		//	We spawn our enemy's ships
		//SpawnUntilFull();	//	Previously SpawnEnemies ();
	}

    // Makes the Death Star appear in a slideIn
    private void Update()
    {
        if(bossAppeared == 0)
        {
            Vector3 dir = new Vector3(0, 3.7f, 0) - boss.transform.position;
            float distance = dir.magnitude;
            dir = dir.normalized;
            float move = speed * Time.deltaTime;

            if (move > distance)
            {
                move = distance;
            }
            boss.transform.Translate(dir * move);
        }
    }

    //	This method spawn all the enemy's ships at once.
    void SpawnEnemies() {
		//	We instantiate an enemy ship for each spot in the enemy's formation
		foreach (Transform child in transform) {
			GameObject enemy = Instantiate (enemyPrefab, child.transform.position, Quaternion.identity) as GameObject;
			enemy.transform.parent = child;
		}
	}

	//	This method spawn an enemy's ship at a time.  If the method detects that there is another free
	//	position, then it calls itself once more after a given delay.
	public void SpawnUntilFull() {
        if (one == false)
        {
            // Spawn the boss when the boolean one is false, and then change it to true, so more don't spawn
            boss = Instantiate(enemyPrefab, deathPosition, Quaternion.identity) as GameObject;
            bossAppeared++;
            one = true;
        }
        //enemy.transform.parent = freePosition;
	}

	//	Draws a wire cube as a gizmo for the enemy's formation in our gamespace
	public void OnDrawGizmos() {
		Gizmos.DrawWireCube (transform.position, new Vector3 (width, height));
	}

	//	Checks if all of the enemy ships are destroyed.
	bool AllMembersDead() {
        //	The method iterates througouth all the transform's children.  If there is at least one
        //	enemy ship still alive, it returns false.
        
		foreach (Transform childPositionGameObject in transform) {
			if (childPositionGameObject.childCount > 0) {
				return false;
			}
            
        }

		//	If none of the enemy's ships is alive, it returns true.
		return true;
	}
}
