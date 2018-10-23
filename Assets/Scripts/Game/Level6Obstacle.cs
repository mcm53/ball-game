using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level6Obstacle : MonoBehaviour {

	public int Speed = 30;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.right, Speed * Time.deltaTime);
	}
}
