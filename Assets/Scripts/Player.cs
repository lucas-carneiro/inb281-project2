using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private Transform myTransform;

	public float walkSpeed = 5.0f;
	public float jumpForce = 350.0f;
	private float distToGround;
    private bool oneJump;

	public GameObject model;

	// Use this for initialization
	void Start () {
		myTransform = this.transform;
		distToGround = myTransform.GetComponent<Collider>().bounds.extents.y;
        oneJump = false;
	}
	
	// Update is called once per frame
	void Update () {			
		Controls ();
	}

	void Controls(){

		//Move Right
		if (Input.GetKey ("d")) {
			myTransform.Translate(walkSpeed * Time.deltaTime, 0f, 0f);
		}

		//Move Left
		if (Input.GetKey ("a")) {
			myTransform.Translate(-walkSpeed * Time.deltaTime, 0f, 0f);
		}

		//Jumping
		if (Input.GetKeyDown ("space") && (CheckGrounded() || oneJump)) {
			myTransform.GetComponent<Rigidbody>().AddForce (0,jumpForce,0);
            oneJump = !oneJump;
		}
	}

	//Raycast down to check if grounded
	public bool CheckGrounded(){
		return Physics.Raycast(myTransform.position, -Vector3.up, distToGround + 0.1f);
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
