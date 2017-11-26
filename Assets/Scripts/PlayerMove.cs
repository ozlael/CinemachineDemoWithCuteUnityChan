using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : MonoBehaviour {
	[SerializeField] float moveSpeed = 6.0f;
	[SerializeField] float turnSpeed = 10.0f;
	[SerializeField] bool isCamDependentDir = true;
	[SerializeField] bool isFly = false;
	Animator anim;
	// What ever you want to move : NavMesh? Rigidbody? or just only Transform
	NavMeshAgent agent;
	Rigidbody rb;

	Quaternion rot;
	Vector3 moveDirection = Vector3.zero;
	bool isMoving = false;
	bool isRunning = false;


	void Start() {
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
		agent = GetComponent<NavMeshAgent> ();
		rot = transform.rotation;
	}

	void Update() {
		if (isFly)
			UpdateFlyMoveInput ();
		else
			UpdateFreeMoveInput ();

		if (agent != null)
			UpdateNav ();
		else
			UpdateMove ();

		UpdateRotation ();
		UpdateAnimMove();
		UpdateAnimJump ();
	}


	void UpdateNav() {
		if (agent != null) {
			agent.Move (moveDirection * Time.deltaTime);
		}
	}

	void FixedUpdate() {
		FixedUpdateMove ();
	}


	void UpdateFlyMoveInput() {
		rot = Quaternion.AngleAxis ( Input.GetAxis("Horizontal") * turnSpeed, Vector3.up) * transform.rotation;
		moveDirection = (rot * Vector3.forward) * Input.GetAxis("Vertical") * moveSpeed * 2;
		isMoving = true;
	}

	void UpdateRotation() {
		if (rb)
			rb.rotation = rot;
		else
			transform.rotation = rot;
	
	}

	void UpdateFreeMoveInput() {
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveDirection.Normalize ();

		isMoving = false;
		isRunning = false;
		if (moveDirection.sqrMagnitude > 0)
			isMoving = true;
		if( isCamDependentDir)
			moveDirection = Camera.main.transform.TransformDirection(moveDirection);

		if (isMoving) {
			if (Input.GetButton ("Fire3")) {
				isRunning = true;
			}
			moveDirection.y = 0;
			moveDirection.Normalize ();
			moveDirection = moveDirection * (isMoving ? moveSpeed : 0) * (isRunning ? 2f : 1f);

			rot = Quaternion.LookRotation (moveDirection.normalized);
		}
	}

	void FixedUpdateMove() {
		if (rb != null) {
			rb.velocity = moveDirection;
		}
	}

	void UpdateMove( ) {
		if (isMoving && rb == null) {
			transform.position += moveDirection * Time.deltaTime;
		}
	}

	void UpdateAnimJump() {
		if (Input.GetButtonDown ("Jump")) {
			anim.SetTrigger ("jump");
		}
	}

	void UpdateAnimMove() {
		if (anim) {
			anim.SetBool ("move", isMoving);
			anim.SetBool ("run", isRunning);
			anim.SetBool ("fly", isFly);
		}
	}
}


