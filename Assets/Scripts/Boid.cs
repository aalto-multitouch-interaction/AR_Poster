using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {
	Vector3 velocity;
	Vector3 acceleration;
	Vector3 startPosition;

	float cohesionRadius = 0.04f;
	float separateRadius = 0.005f;
	static float maxSpeed = 0.01f;
	float maxForce = maxSpeed / 10f;

	enum State {TRACKING, RETURNING, STOPPED};
	State state;

	float followMultiplier = 0.6f;

	// Use this for initialization
	public void Start () {
		velocity = new Vector3 (
			Random.Range (-maxSpeed, maxSpeed), 
			Random.Range (-maxSpeed, maxSpeed), 
			Random.Range (-maxSpeed, maxSpeed)
		);
		startPosition = this.transform.position;

		state = State.TRACKING;
	}
	
	// Update is called once per frame
	public void Update () {
		acceleration *= 0;

		switch (state) {
		case State.STOPPED:
			this.gameObject.GetComponent<Renderer> ().enabled = false;
			break;
		case State.TRACKING:
			applyRules ();
			constrainPosition ();
			move ();
			break;
		case State.RETURNING:
			acceleration += seek (startPosition);
			move ();
			if (Vector3.Distance(transform.position,startPosition) < 0.01) {
				stop ();
			}
			break;
		}
	}

	private void move() {
		velocity += acceleration;
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
		this.transform.position += velocity;
		this.transform.rotation = Quaternion.LookRotation(velocity.normalized);
		this.transform.Rotate(Vector3.up, -90);
	}

	private void stop() {
		state = State.STOPPED;
	}

	public void returnHome() {
		if (state == State.TRACKING) {
			state = State.RETURNING;
		}
	}

	public void fly() {
		state = State.TRACKING;
		this.gameObject.GetComponent<Renderer> ().enabled = true;
	}

	private void constrainPosition() {
		if (transform.position.y < 0f) {
			Vector3 desired = new Vector3 (velocity.x, maxSpeed, velocity.z);
			applyForce (seek (desired));

		} else if (transform.position.y > 0.2f) {
			Vector3 desired = new Vector3 (velocity.x, -maxSpeed, velocity.z);
			applyForce (seek (desired));
		}
	}

	private void applyForce(Vector3 force) {
		acceleration += force;
	}

	private void applyRules() {
		GameObject[] boids = Flock.boids;

		// Follow others
		Vector3 cohesionSum = Vector3.zero;
		Vector3 cohesionSteer = Vector3.zero;
		int cohesionCount = 0;

		// Avoid others
		Vector3 separateSum = Vector3.zero;
		Vector3 separateSteer = Vector3.zero;
		int separateCount = 0;

		// Follow
		Vector3 followSteer = Vector3.zero;


		Target[] targets = Flock.targets;
		float shortestDist = 1000;
		Target target = null;
		foreach (Target t in targets) {
			float d = Vector3.SqrMagnitude (t.position - this.transform.position);
			if (d < shortestDist) {
				shortestDist = d;
				target = t;
			}
		}

		if (target != null) {
			if (target.avoid) {
				Vector3 diff = transform.position - target.position;
				followSteer = diff.normalized * maxForce;
				followMultiplier = 1f;
			} else {
				followSteer = seek (target.position);
				followMultiplier = 0.6f;
			}
		}


		foreach (GameObject boid in boids) {
			float dist = Vector3.SqrMagnitude (boid.transform.position - transform.position);
			if (dist > 0 && dist < cohesionRadius) {
				// Cohesion
				cohesionSum += boid.transform.position;
				cohesionCount++;

				if (dist < separateRadius) {
					Vector3 diff = (this.transform.position - boid.transform.position).normalized;
					diff = diff / dist;
					separateSum += diff;
					separateCount++;
				}
			}
		}

		if (cohesionCount > 0) {
			cohesionSum /= cohesionCount;
			cohesionSteer = seek (cohesionSum);
		}

		if (separateCount > 0) {
			separateSum /= separateCount;
			separateSum = separateSum.normalized;
			separateSum *= maxSpeed;
			separateSteer = separateSum - velocity;
			separateSteer = Vector3.ClampMagnitude (separateSteer, maxForce);
		}

		cohesionSteer *= 0.5f;
		separateSteer *= 0.9f;
		followSteer *= followMultiplier;

		applyForce(cohesionSteer);
		applyForce(separateSteer);
		applyForce (followSteer);
	}

	private Vector3 seek(Vector3 target) {
		Vector3 desired = target - this.transform.position;
		desired = desired.normalized;
		desired *= maxSpeed;
		Vector3 steer = desired - velocity;
		steer = Vector3.ClampMagnitude (steer, maxForce);
		return steer;
	}
}
