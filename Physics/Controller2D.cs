using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{

    public LayerMask collisionMask;
    
    public float maxAngle = 60;

    public CollisionInfo collisions;

    public override void Start()
    {
        base.Start();
    }



    public castInfo HorizontalCast(float distance, Vector2 origin)
    {

        float s = skinWidth * 2;

        castInfo info = new castInfo();

        info.hitNormal = Vector2.up;

        info.hitDistance = distance;

        Vector2 size = (Vector2)box.bounds.size - (Vector2.one * s * 2);
        Vector2 direction = Vector2.right * Mathf.Sign(distance);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, direction, Mathf.Abs(distance) + s, collisionMask);

        if (hit)
        {
            info.hitDistance = (hit.distance - s) * Mathf.Sign(distance);
            info.hitObject = hit.transform.gameObject;
            info.hasHit = true;
            info.hitNormal = hit.normal;
        }

        return info;
    }

    public castInfo VerticalCast(float distance, Vector2 origin)
    {
        float s = skinWidth * 2;

        castInfo info = new castInfo();

        info.hitNormal = Vector2.up;

        info.hitDistance = distance;

        Vector2 size = (Vector2)box.bounds.size - (Vector2.one * s * 2);
        Vector2 direction = Vector2.up * Mathf.Sign(distance);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, direction, Mathf.Abs(distance) + s, collisionMask);


        if (hit)
        {
            info.hitDistance = (hit.distance - s) * Mathf.Sign(distance);
            info.hitObject = hit.transform.gameObject;
            info.hasHit = true;
            info.hitNormal = hit.normal;
        }

        return info;
    }

    public castInfo HorizontalCast(float distance, Vector2 origin, GameObject searchObject)
    {

        float s = skinWidth * 2;

        castInfo info = new castInfo();

        info.hitNormal = Vector2.up;

        info.hitDistance = distance;

        info.hasHit = false;

        Vector2 size = (Vector2)box.bounds.size - (Vector2.one * s * 2);
        Vector2 direction = Vector2.right * Mathf.Sign(distance);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, size, 0, direction, Mathf.Abs(distance) + s, collisionMask);
        RaycastHit2D hit = new RaycastHit2D();

        bool hasHit = false;

        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].transform.gameObject == searchObject)
            {
                hit = hits[i];
                hasHit = true;
                break;
            }
        }


        if (hasHit)
        {
            info.hitDistance = (hit.distance - s) * Mathf.Sign(distance);
            info.hitObject = hit.transform.gameObject;
            info.hasHit = true;
            info.hitNormal = hit.normal;
        }

        return info;
    }






    public void Move(Vector2 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
            
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);


            

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
               
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 velocity)
    {

        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref velocity);
            SlideDownMaxSlope(maxSlopeHitRight, ref velocity);
        }


        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);



                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != 0 && slopeAngle <= maxAngle)
                    {
                        if (Mathf.Sign(hit.normal.x) == directionX)
                        {
                            if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                            {
                                float moveDistance = Mathf.Abs(velocity.x);
                                float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                                velocity.y -= descendVelocityY;

                                collisions.slopeAngle = slopeAngle;
                                collisions.descendingSlope = true;
                                collisions.below = true;
                            }
                        }
                    }
                }
            
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }

    }


}







public struct castInfo
{
    public bool hasHit;
    public float hitDistance;
    public GameObject hitObject;
    public Vector2 hitNormal;
}

public struct CollisionInfo
{
    public bool above, below;
    public bool left, right;

    public bool climbingSlope;
    public bool descendingSlope;
    public float slopeAngle, slopeAngleOld;
    public Vector3 velocityOld;
    public bool slidingDownMaxSlope;
    public Vector2 slopeNormal;



    public void Reset()
    {
        above = below = false;
        left = right = false;
        climbingSlope = false;
        descendingSlope = false;

        slidingDownMaxSlope = false;
	    slopeNormal = Vector2.zero;

        slopeAngleOld = slopeAngle;
        slopeAngle = 0;
    }
}