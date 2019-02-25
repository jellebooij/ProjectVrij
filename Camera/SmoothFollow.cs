using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {

    public float smoothTime;
    public Vector2 offset;
    public Transform target;
    Vector2 refPosition; 

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = (Vector2)target.position;
        transform.position += Vector3.forward * -10;
	}
}
