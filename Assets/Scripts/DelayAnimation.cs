using UnityEngine;
using System.Collections;

public class DelayAnimation : MonoBehaviour {

	private Animation myAnimation;
	public float delay = 0f;
	private float remainingDelay;

	// Use this for initialization
	void Start () {
		myAnimation = GetComponent<Animation>();
		remainingDelay = delay;
	}
	
	// Update is called once per frame
	void Update () {
		if (remainingDelay <= 0f){
			myAnimation.Play(myAnimation.clip.name);
		}
		else{
			remainingDelay -= Time.deltaTime;
		}
	}
}
