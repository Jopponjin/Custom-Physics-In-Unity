using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class Body : MonoBehaviour
{
    //Physical Body values
    public float radius => transform.localScale.x * 0.5f;
    public float mass = 1f;
    [Space]

    //Current Body forces
    public Vector3 velocity;
    public Vector3 forceDirection;
    public Vector3 acceleration;
    public Vector3 totalForce;
    [Space]

    //World forces
    public bool useGravity = true;
    public Vector3 gravity = new Vector3(0,-9.82f, 0);
    [Space]

    //Collison info
    public Vector3 Normal; 
    public bool hasCollision;
    [Space]
    public CustomPlane currentPlane;
    public List<CustomPlane> planeList = new List<CustomPlane>();
    public CustomMeshColider[] meshList;
    [Space]
    public CustomMeshColider currentMesh;

    private void Start()
    {
        var planeObjects = FindObjectsOfType<CustomPlane>();
        meshList = FindObjectsOfType<CustomMeshColider>();


        for (int i = 0; i < planeObjects.Length; i++)
        {
            planeList.Add(planeObjects[i].GetComponent<CustomPlane>());    
        }
    }

    private void FixedUpdate()
    {
        Step();

        // Velocity is constrained, because 2D game
        velocity.z = 0;

        for (int i = 0; i < planeList.Count; i++)
        {
            planeList[i].Response(this);
        }
        
    }

    //Mesh physics runs here
    public void Step()
    {
        totalForce = Vector3.zero;
        if (useGravity) totalForce = gravity;
        velocity = velocity + totalForce * Time.fixedDeltaTime;

        Vector3 newPosition = transform.position + velocity * Time.fixedDeltaTime;
        Vector3 rayDirection = (newPosition - transform.position).normalized;

        bool didHit = false;

        Debug.DrawRay(transform.position, rayDirection, Color.red);

        // Loop through mesh and pick the closes by racast
        for (int i = 0; i < meshList.Length; i++)
        {
            CustomMeshColider Mesh = meshList[i];

            if (!Mesh.Raycast(transform.position, rayDirection, out Vector3 hitPoint, out PlaneStruct hitPlane))
            {
                continue;
            }

            Debug.DrawLine(hitPoint, hitPoint + hitPlane.Normal * 3f, Color.red);

            float distance1 = Vector3.Distance(transform.position, newPosition);
            float distance2 = Vector3.Distance(transform.position, hitPoint);
            // Distance check see if we can make use of the plane in question
            if (distance1 < distance2)
            {
                Debug.DrawLine(hitPoint, hitPoint + hitPlane.Normal * 3f, Color.green);
                continue;
            }

            Debug.DrawLine(hitPoint, hitPoint + hitPlane.Normal * 3f, Color.blue, 10f);

            didHit = true;
            
            velocity = hitPlane.Reflect(velocity, 1f - Mesh.Bousiness);

            //velocity *= 0.95f; // uncomment for very simple "Friction"

            transform.position = hitPoint + hitPlane.Normal * 0.01f;

            break;
        }

        if (!didHit)
        {
            transform.position = newPosition;
        }
        
    }

    public void ApplyForce(Vector3 force)
    {
        //Add force to body
        totalForce += force;

        if (useGravity) totalForce += gravity;

        velocity = velocity + totalForce * Time.fixedDeltaTime;
    }  
}
