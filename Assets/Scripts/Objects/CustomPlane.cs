using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Could have useed the meshColliders class for the plane objects aswell but its not to reliable, so that why this still exists
public class CustomPlane : MonoBehaviour
{
    public Vector3 Normal => transform.up;
    Vector3 Position => transform.position;
    [Space]
    public float bounciness = 0f;
    
    PlaneStruct planeStruct = new PlaneStruct();

    public void Response(Body sphere)
    {
        planeStruct.Position = Position;
        planeStruct.Normal = Normal;
        planeStruct.Response(sphere, bounciness);
    }

    public bool Raycast(Vector3 rayStart, Vector3 rayDir, out Vector3 hitPoint)
    {
        planeStruct.Position = Position;
        planeStruct.Normal = Normal;
        return planeStruct.Raycast(rayStart, rayDir, out hitPoint);
    }

    //Mainly use for debug resons
    public bool SphereCast(Vector3 rayStart, Vector3 rayDir, float sphereRadius, out Vector3 hitPoint)
    {
        planeStruct.Position = Position;
        planeStruct.Normal = Normal;
        return planeStruct.SphereCast(rayStart, rayDir, sphereRadius, out hitPoint);
    }
}

// Plane struct to be used by the customPlane coliders.
public struct PlaneStruct
{
    public Vector3 Normal;
    public Vector3 Position;


    public void Response(Body sphere, float bounciness)
    {
        if (!IsColliding(sphere)) return;

        

        if (IsSphereStatic(sphere))
        {
            sphere.transform.position = CorrectedPosition(sphere);
            sphere.ApplyForce(-sphere.mass * Physics.gravity);
        }
        else
        {
            sphere.velocity = Reflect(sphere.velocity, bounciness);
        }
    }

    public Vector3 Reflect(Vector3 v, float energyDissipation = 0f)
    {
        Vector3 r = (v - (2f - energyDissipation) * Vector3.Dot(v, Normal) * Normal);
        return r;
    }

    //Helper functions
    public float Distance(Body sphere)
    {
        Vector3 sphereToPlane = Position - sphere.transform.position;

        return Vector3.Dot(sphereToPlane, Normal);
    }

    public Vector3 Projection(Body sphere)
    {
        Vector3 sphereToProjection = Distance(sphere) * Normal;

        return sphere.transform.position + sphereToProjection;
    }


    public bool IsColliding(Body sphere)
    {
        //If we are oving away from the plane, skip
        if (Vector3.Dot(sphere.velocity, Normal) > 0f) return false;

        return Distance(sphere) >= 0f || Mathf.Abs(Distance(sphere)) <= sphere.radius;
    }

    public Vector3 CorrectedPosition(Body sphere)
    {
        return Projection(sphere) + Normal * sphere.radius;
    }

    public bool IsSphereStatic(Body sphere)
    {
        bool lowVelocity = sphere.velocity.magnitude < 0.07f;
        bool touchingThePlane = (CorrectedPosition(sphere) - sphere.transform.position).magnitude < 0.2f;

        return lowVelocity && touchingThePlane;
    }


    public bool Raycast(Vector3 rayStart, Vector3 rayDir, out Vector3 hitPoint)
    {
        if (Vector3.Dot(Normal, rayDir) >= 0f)
        {
            hitPoint = rayStart;
            return false;
        }

        float planeDistance = Vector3.Dot(Position, Normal);

        float hitDistance = -(Vector3.Dot(rayStart, Normal) - planeDistance) / Vector3.Dot(rayDir, Normal);

        hitPoint = rayStart + hitDistance * rayDir;

        return true;
    }

    public bool SphereCast(Vector3 rayStart, Vector3 rayDir, float sphereRadius, out Vector3 hitPoint)
    {
        if (Vector3.Dot(Normal, rayDir) >= 0f)
        {
            hitPoint = Vector3.zero;

            return false;
        }

        float planeDistance = Vector3.Dot(Position, Normal) + sphereRadius;

        float hitDistance = -(Vector3.Dot(rayStart, Normal) - planeDistance) / Vector3.Dot(rayDir, Normal);

        hitPoint = rayStart + hitDistance * rayDir;

        return true;
    }
}
