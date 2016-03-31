using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimations : MonoBehaviour {
	
	//Animation Variables & Settings
	public float jumpAnimationSpeed = 0.25f;
	
	public GameObject playerModel;
    private bool isDead;
	Player player;

	// Use this for initialization
	void Start () {
        isDead = false;
		
		player= GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		
		//Animation Setup and Settings
		GetComponent<Animation>().Play ("idle");
		GetComponent<Animation>() ["charge"].speed = jumpAnimationSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		
		Quaternion newRotation = playerModel.transform.rotation;

        //Play animations if no actions are being made
        if (!GetComponent<Animation>().IsPlaying(Player.Status.die.ToString())) {
            if (isDead) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
            if (!GetComponent<Animation>().IsPlaying(Player.Status.attack.ToString())) {
                if (player.currentStatus != Player.Status.die) {
                    player.Controls();
                }
                GetComponent<Animation>().Play(player.currentStatus.ToString());

                //Fix Player Mesh Transform Rotation
                if ((Input.GetKey(player.rightKey) || Input.GetKey(player.rightKey2))) {
                    newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, 90, newRotation.eulerAngles.z);
                    playerModel.transform.rotation = newRotation;

                }
                else if ((Input.GetKey(player.leftKey) || Input.GetKey(player.leftKey2))) {
                    newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, -90, newRotation.eulerAngles.z);
                    playerModel.transform.rotation = newRotation;
                }
            }
        }
        else {
            isDead = true;
        }		
	}
}
