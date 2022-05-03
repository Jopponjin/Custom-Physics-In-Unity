using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayBody : MonoBehaviour
{
    //Current Body forces
    public Vector3 velocity;
    public Vector3 forceDirection;
    public Vector3 acceleration;
    public Vector3 totalForce;

    public Vector3 gravity = new Vector3(0, -9.82f, 0);
    public bool useGravity = true;
    public float mass = 1f;

    // Update is called once per frame
    void Update()
    {

        AddForce(new Vector3(0,0,0));



    }

    void AddForce(Vector3 force)
    {
        acceleration = force / mass;
        totalForce = acceleration * mass;
        if (useGravity) totalForce += gravity;

        velocity = velocity + totalForce * Time.deltaTime;
        transform.position = transform.position + velocity * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        var closesPointOnCol = other.bounds.ClosestPoint(transform.position);

        transform.position = closesPointOnCol;

        //AddForce(-gravity * 10f);
    }

}
