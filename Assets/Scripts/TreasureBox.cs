using UnityEngine;
using System.Collections;

public class TreasureBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    //If player reaches treasure, they win
    void OnTriggerEnter(Collider collidingObject) {
        if (collidingObject.gameObject.tag == "Player") {
            GetComponent<Animation>().Play("open");
            collidingObject.gameObject.SendMessage("Win", SendMessageOptions.DontRequireReceiver);
        }
    }
}
