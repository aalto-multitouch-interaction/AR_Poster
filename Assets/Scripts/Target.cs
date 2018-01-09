using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// area for particles: x: -0.5-0.5; y: 0-0,2; z: -0.3, 0.6

public class Target {
	public Vector3 position;

	private float noiseSeed;
	private float offset;
	private float increment;

	private static float minX = -0.5f *2;
	private static float maxX = 0.5f*2;
	private static float minY = 0f;
	private static float maxY = 0.01f;
	private static float minZ = -0.3f*2;
	private static float maxZ = 0.6f*2;

	public bool avoid = false;

	public Target () {
		noiseSeed = Random.Range (0f, 100f);
		offset = Random.Range (0f, 100f);
		increment = Random.Range (0.005f, 0.01f);

		float x = minX + Mathf.PerlinNoise (noiseSeed, Time.time*0.1f) * (maxX - minX);
		float y = minY + Mathf.PerlinNoise (noiseSeed + offset, Time.time*0.1f) * (maxY - minY);
		float z = minZ + Mathf.PerlinNoise (noiseSeed + offset * 2, Time.time*0.1f) * (maxZ - minZ);

		position = new Vector3 (x, y, z);
		noiseSeed += increment;
	}

	public void updatePos() {
		float x = minX + Mathf.PerlinNoise (noiseSeed, 0) * (maxX - minX);
		float y = minY + Mathf.PerlinNoise (noiseSeed + offset, 0) * (maxY - minY);
		float z = minZ + Mathf.PerlinNoise (noiseSeed + offset * 2, 0) * (maxZ - minZ);

		position.x = x;
		position.y = y;
		position.z = z;

		noiseSeed += increment;
	}

	public void setPos(Vector3 pos) {
		this.position = pos;
	}


}

