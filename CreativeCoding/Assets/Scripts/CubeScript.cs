using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour {

	public float rotateSpeed = 1.0f;
	Vector3 spinSpeed = Vector3.zero;
	Vector3 spinAxis = Vector3.up;
	private float scale = 2.0f;

	// Use this for initialization
	void Start () {
		spinSpeed = new Vector3(Random.value, Random.value, Random.value);
		spinAxis = Vector3.up;
		spinAxis.x = (Random.value - Random.value) * 0.1f;
	}
	
	public void SetSize(float size) 
	{
		transform.localScale = new Vector3(size, size, size);
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate(spinSpeed);
		transform.RotateAround(Vector3.zero, spinAxis, rotateSpeed);
	}
}
