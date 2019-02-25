using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour {


    Controller2D controller;

    Vector2 velocity;

    float gravity = 3;

	void Start () {
		
    controller = GetComponent<Controller2D>();
	}
	
	// Update is called once per frame
	void Update () {

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;

            }

            else
            {

                velocity.y = 0;
            }
        }

        velocity.y -= gravity;

       if(!controller.VerticalCast(-gravity * Time.deltaTime,transform.position).hasHit || controller.collisions.below)
       controller.Move(velocity * Time.deltaTime);

    }
}
