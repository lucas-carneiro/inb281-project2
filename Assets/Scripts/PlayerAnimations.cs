using UnityEngine;
using System.Collections;

public class PlayerAnimations : MonoBehaviour {

	private Transform myTransform;
	
	//Animation Variables & Settings
	public float jumpAnimationSpeed = 0.25f;
	
	public GameObject playerModel;
	
	Player player;

	// Use this for initialization
	void Start () {
		myTransform = this.transform;
		
		player= GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		
		//Animation Setup and Settings
		GetComponent<Animation>().Play ("idle");
		GetComponent<Animation>() ["charge"].speed = jumpAnimationSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		
		Quaternion newRotation = playerModel.transform.rotation;

		//Play Animations
		if (player.CheckGrounded () == true && Input.GetKey ("d") || player.CheckGrounded () == true && Input.GetKey ("a")) {
			GetComponent<Animation>().Play ("run");
		} else if (player.CheckGrounded () == false) {
			GetComponent<Animation>().Play ("charge");
		} else {
			GetComponent<Animation>().Play ("idle");
		}

		//Fix Player Mesh Transform Rotation
		if (Input.GetKeyDown ("d")) {
			newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, 90, newRotation.eulerAngles.z);
			playerModel.transform.rotation = newRotation;
			
		} else if (Input.GetKeyDown ("a")) {
			newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, -90, newRotation.eulerAngles.z);
			playerModel.transform.rotation = newRotation;
		}
		
	}
}
