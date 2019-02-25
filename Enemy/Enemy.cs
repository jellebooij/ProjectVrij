using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float moveSpeed = 5;
    public float gravity = 3;
    public float jumpVelocity = 20;
    public Transform target;
    Controller2D controller;
    Vector2 velocity;

    Vector2 scale;

    public bool jumpPrevious = false;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        scale = transform.localScale;
    }

    
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

        transform.localScale = (Mathf.Sign(velocity.x) * scale.x * Vector2.right) + scale.y * Vector2.up;

        Vector2 input = new Vector2(Mathf.Sign(target.transform.position.x - transform.position.x),0);

        velocity.x = input.x * moveSpeed;

        velocity.y -= gravity;

        castInfo i = controller.HorizontalCast((velocity.x * 3) * Time.deltaTime, transform.position);

        if (i.hasHit && Vector2.Angle(i.hitNormal,Vector2.up) > controller.maxAngle)
        {
            velocity.y = jumpVelocity;
        }



        controller.Move(velocity * Time.deltaTime);

    }
}
