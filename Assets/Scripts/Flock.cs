using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {
	public GameObject boidPrefab;

	// area for particles: x: -0.5-0.5; y: 0-0,2; z: -0.3, 0.6
	// start area: x: 0,05-0,3 y: 0, z: -0,2-0

	static int numBoids = 200;
	static int numTargets = 10;

	public static GameObject[] boids = new GameObject[numBoids];
	public static Target[] targets = new Target[numTargets];

	SphereCollider container;
	float timeToSleep;

	void Start () {
		for (int i = 0; i < numTargets; i++) {
			targets [i] = new Target ();
		}

		for (int i = 0; i < numBoids; i++) {
			float x = Random.Range (0.05f, 0.3f);
			float y = 0;
			float z = Random.Range (-0.2f, 0f);

			Vector3 pos = new Vector3 (x,y,z);


			GameObject obj = Instantiate (boidPrefab, pos, Quaternion.identity);
			obj.transform.parent = gameObject.transform;
			obj.transform.localScale = new Vector3(0.003333434f, 0.003333434f, 0.003333434f);
			boids [i] = obj;
		}

		timeToSleep = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.realtimeSinceStartup > timeToSleep) {
			stopBoids ();
		}
		foreach (Target t in targets) {
			t.updatePos ();
		}

		if (Input.anyKey) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				targets [0].setPos (hit.point);	
				targets [0].avoid = true;
			}
		} else {
			targets [0].avoid = false;
		}
	}

	void startBoids() {
		foreach (GameObject boid in boids) {
			boid.SendMessage("fly");
		}
	}

	void stopBoids() {
		foreach (GameObject boid in boids) {
			boid.SendMessage("returnHome");
		}
	}

	void OnMouseDown() {
		startBoids ();
		timeToSleep = Time.realtimeSinceStartup + 10.0f;
	}
}
