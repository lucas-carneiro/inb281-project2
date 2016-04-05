using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    //Player animation status
    public enum Status {idle, walk, run, charge, attack, idlebattle, die, victory};
    public Status currentStatus;

	private Transform myTransform;

	public float walkSpeed = 5.0f;
	public float jumpForce = 350.0f;
	private float distToGround;

	public GameObject model;

    public float maxHP = 50f;
    private float currentHP;

    public float jumpCooldown = 0.1f;
    public float attackCooldown = 0.1f;
    private bool hasAttacked;
    private bool inCooldown;
    private float cooldownRemaining;

    //Object that represents HP
    public Slider HP;

    //Object that represents an image which appears when the player loses health
    public Image damageImage;
    public Color damageColor;
    public float damageFade = 5f;

    //Movement keys
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode leftKey2 = KeyCode.A;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode rightKey2 = KeyCode.D;
    public KeyCode jumpKey = KeyCode.UpArrow;
    public KeyCode jumpKey2 = KeyCode.Space;
    public KeyCode attackKey = KeyCode.C;
    public KeyCode attackKey2 = KeyCode.DownArrow;

    //Action variables
    public KeyCode actionKey = KeyCode.Alpha1;
    public KeyCode restartKey = KeyCode.Return;
    public Text ActionText;
    private int kills = 0;
    public Text ScoreText;

    public AudioClip winSound;
    public AudioClip loseSound;

    // Use this for initialization
    void Start () {
		myTransform = this.transform;
		distToGround = myTransform.GetComponent<Collider>().bounds.extents.y;
        currentHP = maxHP;
        damageColor.a = 0;
        inCooldown = false;
        hasAttacked = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (currentHP <= 0) {
            Lose();
        }

        //Fade damageImage
        if (damageImage.color.a > 0f) {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, damageFade * Time.deltaTime);
        }

        if (currentStatus == Status.victory && Input.GetKeyDown(restartKey)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
	}

    //Called inside PlayerAnimations
    //Reason: Controls do not work while an action is happening
	public void Controls(){
        //Idle
        if (hasAttacked) {
            currentStatus = Status.idlebattle;
        }
        else {
            currentStatus = Status.idle;
        }

		//Move Right
		if (Input.GetKey(rightKey) || Input.GetKey(rightKey2)) {
            currentStatus = Status.run;
            hasAttacked = false;
			myTransform.Translate(walkSpeed * Time.deltaTime, 0f, 0f);
		}

        //Move Left
        if (Input.GetKey(leftKey) || Input.GetKey(leftKey2)) {
            currentStatus = Status.run;
            hasAttacked = false;
            myTransform.Translate(-walkSpeed * Time.deltaTime, 0f, 0f);
		}        

        if (inCooldown) {
            cooldownRemaining -= jumpCooldown * Time.deltaTime;
            inCooldown = cooldownRemaining > 0f;
            if (!inCooldown) {
                cooldownRemaining = 0f;
            }
        }
        else if (CheckGrounded()){
            //Jump
            if (Input.GetKey(jumpKey) || Input.GetKey(jumpKey2)) {
                hasAttacked = false;
                myTransform.GetComponent<Rigidbody>().AddForce(0, jumpForce, 0);
                inCooldown = true;
                cooldownRemaining = jumpCooldown;
            }
            //Attack
            if (Input.GetKey(attackKey) || Input.GetKey(attackKey2)) {
                currentStatus = Status.attack;
                hasAttacked = true;
            }
        }

        if (!CheckGrounded()) {
            currentStatus = Status.charge;
        }
	}

	//Raycast down to check if grounded
	public bool CheckGrounded(){
		return Physics.Raycast(myTransform.position, -Vector3.up, distToGround + 0.1f);
	}

    void OnTriggerStay(Collider collidingObject) {
        //If collidingObject is an action object
        if (currentStatus == Status.attack && collidingObject.gameObject.tag == "Action") {
            if (Input.GetKeyDown(actionKey)) {
                collidingObject.gameObject.SendMessage("Act", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    //Called by external game objects
    void TakeDamage(float damage) {
        if (currentStatus != Status.die) {
            currentHP -= damage;
            HP.value = currentHP / maxHP;
            damageImage.color = new Vector4(damageColor.r, damageColor.g, damageColor.b, 1f);
        }
    }

    //Called by external game objects
    public void getKill() {
        ScoreText.text = "" + ++kills;
    }

    //Called by external game objects
    public void Win() {
        currentStatus = Status.victory;
        ActionText.text = "You found the treasure! You won! Press " + restartKey + " to play again.";
        ActionText.gameObject.SetActive(true);
        //AudioSource.PlayClipAtPoint(winSound, transform.position);
    }

    public void Lose() {
        currentStatus = Status.die;
    }

    //Platform parenting - Keep player parented to moving platforms
    void OnCollisionStay(Collision collidingObject){
		if(collidingObject.gameObject.tag == "Platform"){
			myTransform.parent = collidingObject.transform;
		}
	}

	//Platform de-parenting - Remove player from platform when no longer touching
	void OnCollisionExit(Collision collidingObject){
		if (collidingObject.transform.tag == "Platform"){
			myTransform.parent = null;
		}
	}
}