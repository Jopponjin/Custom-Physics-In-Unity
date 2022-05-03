using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyData : MonoBehaviour
{
    public float mass = 1f;
    public float Bounciness = 0f;
    public Vector3 colliderBounds;

    //Current Body forces
    public Vector3 velocity;
    public Vector3 direction;
    public Vector3 acceleration;
    public Vector3 totalForce;


}
