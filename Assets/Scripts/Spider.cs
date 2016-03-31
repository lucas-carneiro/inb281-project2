﻿using UnityEngine;

public class Spider : MonoBehaviour {

	private Transform myTransform;

	public float moveSpeed = 2.0f;

	//Patrolling Variables
	public GameObject[] patrolPoints;
	private int currentPatrolPoint = 0;
	public float patrolPointDistance = 1.0f;

    public enum Status { walk, taunt, attack, hit, death };
    public Status currentStatus;

    // Use this for initialization
    void Start () {
		myTransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		Patrol ();
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
