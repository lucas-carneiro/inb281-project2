using UnityEngine;

public class Spider : MonoBehaviour {

	private Transform myTransform;

	public float moveSpeed = 2.0f;

	//Patrolling Variables
	public GameObject[] patrolPoints;
	private int currentPatrolPoint = 0;
	public float patrolPointDistance = 1.0f;

    public float maxHP = 20f;
    private float currentHP;
    public float damage = 10f;
    public float attackCooldown = 1f;
    private bool inCooldown = false;
    private float cooldownRemaining = 0f;

    public enum Status { walk, taunt, attack, hit, die };
    public Status currentStatus;

    public float range = 10f;
    public GameObject raycaster;

    // Use this for initialization
    void Start () {
		myTransform = this.transform;
        currentHP = maxHP;
    }
	
	// Update is called once per frame
	void Update () {
        if (inCooldown) {
            cooldownRemaining -= attackCooldown * Time.deltaTime;
            inCooldown = cooldownRemaining > 0f;
        }

        if (!GetComponent<Animation>().IsPlaying(Status.die.ToString())) {
            if (!GetComponent<Animation>().IsPlaying(Status.attack.ToString()) &&
                !GetComponent<Animation>().IsPlaying(Status.hit.ToString())) {
                if (aimPlayer()) {
                    currentStatus = Status.taunt;
                }
                else {
                    Patrol();
                }
                //Play animation
                GetComponent<Animation>().Play(currentStatus.ToString());
            }
        }
        else {
            AnimationState state = GetComponent<Animation>()["die"];
            if (state.time > state.length) {
                GameObject player = GameObject.FindWithTag("Player");
                player.SendMessage("getKill", SendMessageOptions.DontRequireReceiver);
                Destroy(myTransform.parent.gameObject);
            }
        }
    }

    bool aimPlayer() {
        //Raycast Detection
        RaycastHit hit;
        Vector3 aim = new Vector3(Mathf.Sign(transform.rotation.y), 0, 0);

        if (Physics.Raycast(raycaster.transform.position, aim, out hit, range)) {

            //If hit has "Player" tag...
            if (hit.transform.tag == "Player") {
                //Draw red debug line
                Debug.DrawLine(raycaster.transform.position, hit.point, Color.red);
                return true;
            }
            else {
                //Draw green debug line
                Debug.DrawLine(raycaster.transform.position, hit.point, Color.green);
            }
        }

        return false;
    }

    void Attack(GameObject enemy) {
        currentStatus = Status.attack;
        inCooldown = true;
        cooldownRemaining = attackCooldown;
        GetComponent<Animation>().Play(currentStatus.ToString());
        enemy.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerEnter(Collider collidingObject) {
        //If collidingObject is an action object (in this case, another enemy)
        //Solution: Try to walk to another patrol point
        if (collidingObject.gameObject.tag == "Action") {
            if(currentPatrolPoint == patrolPoints.Length - 1)
                currentPatrolPoint = 0;
            else
                currentPatrolPoint++;
        }
    }                

    void OnTriggerStay(Collider collidingObject) {
        //If collidingObject is an action object
        if (aimPlayer() && collidingObject.gameObject.tag == "Player" && !inCooldown && currentStatus != Status.die && currentStatus != Status.hit) {
            Attack(collidingObject.gameObject);
        }
    }

    //Called by external game objects
    public void Act() {
        if (aimPlayer()) {
            TakeDamage(maxHP / 4);
        }
        else { //OHKO from behind (weak spot)
            TakeDamage(maxHP);
        }
    }

    void TakeDamage(float damage) {
        currentHP -= damage;
        if (currentHP <= 0) {
            currentStatus = Status.die;
        }
        else {
            currentStatus = Status.hit;
        }
        //Play animation
        GetComponent<Animation>().Play(currentStatus.ToString());
    }

    //Spider Action - Patrol between provided points
    void Patrol(){

		//Snap rotate towards current patrol point
		myTransform.LookAt (patrolPoints [currentPatrolPoint].transform.position);

		//Move in direction of patrol point
		myTransform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        //Walk
        currentStatus = Status.walk;

		//Close to/arrived at patrol point. Switch to next/first patrol point
		if (Vector3.Distance (myTransform.position, patrolPoints[currentPatrolPoint].transform.position) < patrolPointDistance) {
			
			if(currentPatrolPoint == patrolPoints.Length - 1)
				currentPatrolPoint = 0;
			else
				currentPatrolPoint++;
		}
	}
}
