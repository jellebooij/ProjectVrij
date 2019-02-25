/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{

    public LayerMask collisionMask;

    public float skinWith = 0.115f;
    public float maxAngle = 60;

    BoxCollider2D box;

    void Start()
    {
        box = GetComponent<BoxCollider2D>();
    }


    public collisionInfo Move(Vector2 velocity)
    {

        collisionInfo info = new collisionInfo();

        Debug.Log(HorizontalCast(velocity.x - 0.1f, transform.position).hitDistance);

        if (velocity.x != 0)
        {
            Debug.Log("kak");
            castInfo hor = HorizontalCast(velocity.x, transform.position);

            info.left = hor.info.left;
            info.righ = hor.info.righ;

            float surfaceAngle = Vector2.Angle(hor.hitNormal, Vector3.up);

            if ((surfaceAngle != 0) && surfaceAngle < maxAngle)
            {
                velocity = GetSlopeVelocity(surfaceAngle, velocity, ref info);

                return AscendSlopeMove(velocity);
            }
            else
            {
                velocity.x = hor.hitDistance;
            }

        }

        //.........................................................................................................................................

        if (velocity.y != 0)
        {

            castInfo vert = VerticalCast(velocity.y, (Vector2)transform.position + Vector2.right * velocity.x);

            velocity.y = vert.hitDistance;

            if (!info.below)
                info.below = vert.info.below;
            if (!info.above)
                info.above = vert.info.above;

        }

        transform.Translate(velocity);

        return info;

    }

    collisionInfo AscendSlopeMove(Vector2 velocity)
    {


        collisionInfo info = new collisionInfo();

        info.below = true;

        castInfo vert = VerticalCast(velocity.y, (Vector2)transform.position);

        velocity.y = vert.hitDistance;

        if (!info.below)
            info.below = vert.info.below;
        if (!info.above)
            info.above = vert.info.above;

        //.........................................................................................................................................


        castInfo hor = HorizontalCast(velocity.x, (Vector2)transform.position + Vector2.up * velocity.y);

        info.left = hor.info.left;
        info.righ = hor.info.righ;

        velocity.x = hor.hitDistance;

        transform.Translate(velocity);

        return info;

    }

    castInfo HorizontalCast(float distance, Vector2 origin)
    {

        castInfo info = new castInfo();

        info.hitNormal = Vector2.up;

        info.hitDistance = distance;

        Vector2 size = (Vector2)box.bounds.size - (Vector2.one * skinWith * 2);
        Vector2 direction = Vector2.right * Mathf.Sign(distance);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, direction, Mathf.Abs(distance) + skinWith, collisionMask);

        if (hit)
        {
            Debug.Log(hit.distance);
            info.hitDistance = (hit.distance - skinWith) * Mathf.Sign(distance);
            info.hitObject = hit.transform.gameObject;
            info.hasHit = true;
            info.hitNormal = hit.normal;

            if (distance < 0)
                info.info.left = true;
            else
                info.info.righ = true;

        }

        return info;
    }

    castInfo VerticalCast(float distance, Vector2 origin)
    {
        castInfo info = new castInfo();

        info.hitNormal = Vector2.up;

        info.hitDistance = distance;

        Vector2 size = (Vector2)box.bounds.size - (Vector2.one * skinWith * 2);
        Vector2 direction = Vector2.up * Mathf.Sign(distance);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, direction, Mathf.Abs(distance) + skinWith, collisionMask);


        if (hit)
        {
            info.hitDistance = (hit.distance - skinWith) * Mathf.Sign(distance);
            info.hitObject = hit.transform.gameObject;
            info.hasHit = true;
            info.hitNormal = hit.normal;

            if (distance < 0)
                info.info.below = true;
            else
                info.info.above = true;

        }

        return info;
    }

    Vector2 GetSlopeVelocity(float slopeAngle, Vector2 velocity, ref collisionInfo info)
    {
        Vector2 v = velocity;

        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocity = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (v.y <= climbVelocity)
        {
            v.y = climbVelocity;
            v.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            info.below = true;
        }

        return v;
    }

}

public struct castInfo
{
    public bool hasHit;
    public float hitDistance;
    public GameObject hitObject;
    public Vector2 hitNormal;

    public collisionInfo info;
}

public struct collisionInfo
{
    public bool above, below, left, righ, climbingSlope;
}

*/