using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5;
    public float gravity = 3;
    public float jumpVelocity = 20;

    public float checkDistance = 0.5f;
    public float distToInteractble;

    Controller2D controller;
    Vector2 velocity;

    public GameObject pushObject;

    public bool jumpPrevious = false;

    void Start()
    {
        controller = GetComponent<Controller2D>();
    }

    // Update is called once per frame
    void Update()
    {


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




        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocity.x = input.x * moveSpeed;

        velocity.y -= gravity;




        castInfo side = controller.HorizontalCast(checkDistance * Mathf.Sign(velocity.x), transform.position);

        if (side.hitObject != null && Input.GetAxisRaw("Fire1") != 0)
        {
            if (side.hitObject.tag == "box")
            {
                pushObject = side.hitObject;
            }
        }

        if (pushObject != null)
        {
            PushBox(pushObject,side);
            return;
        }
        
        if (Input.GetAxisRaw("Jump") != 0 && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }
        
        controller.Move(velocity * Time.deltaTime);

    }


    void PushBox(GameObject box,castInfo info)
    {


        Controller2D boxController = box.GetComponent<Controller2D>();

        if (Input.GetAxisRaw("Fire1") != 0 && controller.VerticalCast(-1, transform.position).hasHit && box != null)
        {


            float dist = GetDistanceToInteractable(box.transform.position, boxController);


            if (Mathf.Abs(Mathf.Abs(dist) - distToInteractble) < 0.1f && velocity.x != 0)
            {
          
                if (Mathf.Sign(transform.position.x - box.transform.position.x) == Mathf.Sign(velocity.x))
                {

                    //Pulling
                    controller.Move(velocity * Time.deltaTime);

                    float m = GetDistanceToInteractable(box.transform.position, boxController);

                    if(m == 100)
                    {
                        DisconectFromInteractable();
                        return;
                    }
                   

                    boxController.Move(Vector2.right * -(m - (distToInteractble * Mathf.Sign(m))) - (Vector2.up * Time.deltaTime));
                }
                else
                {

                    //Pusing
                    boxController.Move((Vector2.right * velocity.x - Vector2.up) * Time.deltaTime);
                 
                    float m = GetDistanceToInteractable(box.transform.position, boxController);

                    if (m == 100)
                    {
                      
                        DisconectFromInteractable();
                        return;
                    }

                    controller.Move(((Vector2.right * (m - (distToInteractble * Mathf.Sign(m)))) + (Vector2.up * velocity.y * Time.deltaTime)));
                    
                }
            }

            if (dist != distToInteractble)
            {
                
                float d = (Mathf.Abs(dist) - distToInteractble) * 30;
                controller.Move(((Vector2.right * Mathf.Sign(box.transform.position.x - transform.position.x) * d) - Vector2.up * gravity) * Time.deltaTime);
            }



        }
        else
        {
            Debug.Log("kak");
            DisconectFromInteractable();
        }
    }

    void DisconectFromInteractable()
    {
        controller.Move(velocity * Time.deltaTime);
        pushObject = null;
    }

    float GetDistanceToInteractable(Vector2 interPos, Controller2D boxController)
    {
        float dist = 0;

        if ((interPos.y - boxController.box.bounds.size.y / 2) < (transform.position.y - controller.box.bounds.size.y / 2))
        {
            //chacter staat hoger
            castInfo i = controller.HorizontalCast(Mathf.Abs(transform.position.x - interPos.x) * Mathf.Sign(interPos.x - transform.position.x), transform.position, pushObject);

            if (i.hasHit)
                dist = i.hitDistance;
        }
        else
        {
            //box staat hoger
            castInfo i = boxController.HorizontalCast(Mathf.Abs(interPos.x - transform.position.x) * Mathf.Sign(transform.position.x - interPos.x), interPos, gameObject);

            if(i.hasHit)
                dist = -i.hitDistance;

        }

    
        return dist;
    }



}
