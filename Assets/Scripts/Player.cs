using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    //Player animation status
    public enum Status {idle, walk, run, charge, attack, idlebattle, die};
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
    public GameObject HP;

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
            lose();
        }

        //Fade damageImage
        if (damageImage.color.a > 0f) {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, damageFade * Time.deltaTime);
        }

        if (Time.timeScale == 0f && Input.GetKeyDown(restartKey)) {
            Time.timeScale = 1f;
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

    //Interaction between player and objects
    void OnTriggerEnter(Collider collidingObject) {
        //If collidingObject is an action object
        if (collidingObject.gameObject.tag == "Action") {
            //ActionText.text = "" +
            //    Possible action texts, from external object classes
            //    collidingObject.gameObject.GetComponent<Turret>() +
            //    collidingObject.gameObject.GetComponent<EmergencyGlass>() +
            //    collidingObject.gameObject.GetComponent<EmergencyStop>() +
            //    " " + actionKey;
            //ActionText.gameObject.SetActive(true);
        }
    }
    void OnTriggerStay(Collider collidingObject) {
        //If collidingObject is an action object
        if (collidingObject.gameObject.tag == "Action") {
            if (Input.GetKeyDown(actionKey)) {
                //ActionText.gameObject.SetActive(false);
                collidingObject.gameObject.SendMessage("Act", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    void OnTriggerExit(Collider collidingObject) {
        //If collidingObject is an action object
        if (collidingObject.gameObject.tag == "Action") {
            //ActionText.gameObject.SetActive(false);
        }
    }

    //Called by external game objects
    void TakeDamage(float damage) {
        currentHP -= damage;
        HP.transform.localScale = new Vector3(currentHP / maxHP, 1f, 1f);
        damageImage.color = new Vector4(damageColor.r, damageColor.g, damageColor.b, 1f);
    }

    //Called by external game objects
    public void getKill() {
        ScoreText.text = "" + ++kills;
    }

    //Called by external game objects
    public void win() {
        //GameObject.FindGameObjectWithTag("Finish").GetComponent<Light>().color = new Vector4(1, 1, 1, 1);
        ActionText.text = "You stopped the machines! You won! Press " + restartKey + " to play again.";
        ActionText.gameObject.SetActive(true);
        AudioSource.PlayClipAtPoint(winSound, transform.position);
        Time.timeScale = 0f;
    }

    public void lose() {
        currentStatus = Status.die;
        ActionText.text = "You died! Maybe someone else will stop the machines... Press " + restartKey + " to play again.";
        ActionText.gameObject.SetActive(true);
        AudioSource.PlayClipAtPoint(loseSound, transform.position);
        Time.timeScale = 0f;
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