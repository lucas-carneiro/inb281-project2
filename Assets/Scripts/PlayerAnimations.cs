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

        //Play animations if no actions are being made
        if (!GetComponent<Animation>().IsPlaying(Player.Status.attack.ToString())) {
            player.Controls();
            GetComponent<Animation>().Play(player.currentStatus.ToString());
        }

		//Fix Player Mesh Transform Rotation
		if ((Input.GetKey(player.rightKey) || Input.GetKey(player.rightKey2))) {
			newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, 90, newRotation.eulerAngles.z);
			playerModel.transform.rotation = newRotation;
			
		} else if ((Input.GetKey(player.leftKey) || Input.GetKey(player.leftKey2))) {
			newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, -90, newRotation.eulerAngles.z);
			playerModel.transform.rotation = newRotation;
		}
		
	}
}
