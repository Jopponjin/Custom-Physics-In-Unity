using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;
using FutureGames.GamePhysics;


public class PlayerInteraction : MonoBehaviour
{
    public CustomPlane gamePlane;
    public CustomMeshColider meshColider;
    [Space]
    public Body spherePrefab;
    public Body spawnedSphere;
    [Space]
   
    public bool hasShoot = false;
    public float shotForce = 10f;
    [Space]

    public Vector3 rayHit;
    public Vector3 shootDirection;
    Vector3 sphereOrigin;

    public GameObject aimPrefab;
    GameObject aimPoint;

    private void Start()
    {
        if (!aimPoint) aimPoint = Instantiate(aimPrefab);
    }

    private void Update()
    {
        MouseAim();

        GameInput();
    }

    void GameInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!hasShoot)
            {
                hasShoot = true;

                spawnedSphere = Instantiate(spherePrefab);
                spawnedSphere.transform.position = transform.position;
                spawnedSphere.velocity = Vector3.zero;
                spawnedSphere.ApplyForce(shootDirection * shotForce * 100f);

                StopAllCoroutines();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            hasShoot = false;
            if (spawnedSphere)
            {
                Destroy(spawnedSphere.gameObject);
                spawnedSphere = null;
            }
            
        }

        
    }

    void MouseAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (gamePlane.Raycast(ray.origin, ray.direction, out Vector3 rayHit))
        {
            Vector3 lookDirection = (rayHit - transform.position).normalized;
            
            shootDirection = lookDirection; //Debug direction cache
            sphereOrigin = transform.position;

            aimPoint.transform.position = new Vector3(rayHit.x, rayHit.y, -0.5f);
        }
    }

    private void OnDrawGizmos()
    {
       
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.75f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + shootDirection * 10f);

        if (!spawnedSphere) return;

        Vector3 closesHit = Vector3.zero;
        Vector3 closesNormal = Vector3.zero;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < spawnedSphere.planeList.Count; i++)
        {
            CustomPlane plane = spawnedSphere.planeList[i];
            if (!plane.SphereCast(transform.position, shootDirection, spawnedSphere.radius, out Vector3 rayHit)) continue;
            
            float distance = Vector3.Distance(transform.position, rayHit);
            //Debug.DrawLine(rayHit, rayHit + plane.Normal * 10f);

            if (distance < minDistance)
            {
                closesHit = rayHit;
                closesNormal = plane.Normal;
                minDistance = distance;
            }
        }

        

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(closesHit, spawnedSphere.radius);

        Gizmos.DrawLine(closesHit, closesHit + closesNormal * 10f);

        if (!meshColider) return;

        if (meshColider.Raycast(transform.position, shootDirection, out rayHit, out PlaneStruct hitPlane))
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(rayHit, spawnedSphere.radius);
            Gizmos.DrawLine(rayHit, rayHit + hitPlane.Normal);
        }   

    }
}
