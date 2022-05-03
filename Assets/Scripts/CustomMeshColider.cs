using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code inspration taken from https://www.cs.princeton.edu/courses/archive/fall00/cs426/lectures/raycast/sld018.htm
// More or less we build our mesh colliders, and than add a edge raycast
// in order to be able to test aginst edges and than later on triangles.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomMeshColider : MonoBehaviour
{
    MeshFilter meshFilter;

    public struct Triangle
    {
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;
        public Vector3 N;
    } 

    Triangle[] triangles;

    public float Bousiness = 0.5f;

    private void Start()
    {
        GenerateTriangles();
    }

    //Generate colider mesh for the selected mesh
    void GenerateTriangles()
    {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        int[] meshTriangles = mesh.triangles;
        Vector3[] meshVerts = mesh.vertices;

        triangles = new Triangle[meshTriangles.Length / 3];

        for (int i = 0; i < triangles.Length; i++)
        {
             triangles[i].v1 = meshVerts[meshTriangles[i * 3]];
             triangles[i].v2 = meshVerts[meshTriangles[i * 3 + 1]];
             triangles[i].v3 = meshVerts[meshTriangles[i * 3 + 2]];

            Vector3 right = (triangles[i].v1 - triangles[i].v2).normalized;
            Vector3 forward = (triangles[i].v1 - triangles[i].v3).normalized;

            Vector3 up = Vector3.Cross(right, forward);

            triangles[i].N = up.normalized;
        }
    }
    
    public bool Raycast(Vector3 rayStart, Vector3 rayDir, out Vector3 hitPoint, out PlaneStruct hitPlane)
    {
        Vector3 closesHit = Vector3.zero;
        PlaneStruct closesHitPlane = new PlaneStruct();

        float minDistance = Mathf.Infinity;
        bool didHit = false;


        for (int i = 0; i < triangles.Length; i++)
        {
            Triangle triangle = triangles[i];

            if (!TriangelRaycast(triangle, rayStart, rayDir, out Vector3 rayHit)) continue;
            didHit = true;
            
            float distance = Vector3.Distance(rayStart, rayHit);

            if (distance < minDistance)
            {
                closesHit = rayHit;
                // Code here is not ot performant, would have fixed the normals and positions on initlaztion.... but couldent get it to work
                closesHitPlane.Normal = transform.TransformDirection(triangle.N);
                closesHitPlane.Position = transform.TransformPoint(triangle.v1);
                minDistance = distance;
            }
        }

        hitPoint = closesHit;
        hitPlane = closesHitPlane;
        return didHit;
    }

    
    bool TriangelRaycast(Triangle triangle, Vector3 rayStart, Vector3 rayDir, out Vector3 hitPoint)
    {
        //Making our vertex point to world space points
        Vector3 v1 = transform.TransformPoint(triangle.v1);
        Vector3 v2 = transform.TransformPoint(triangle.v2);
        Vector3 v3 = transform.TransformPoint(triangle.v3);

        Vector3 N = transform.TransformDirection(triangle.N);

        PlaneStruct planeStruct = new PlaneStruct();

        planeStruct.Position = v1;
        planeStruct.Normal = N;

        if (!planeStruct.Raycast(rayStart, rayDir, out hitPoint)) return false;


        if (!TestTriangleEdge(rayStart, v1, v2, hitPoint)) return false;
        if (!TestTriangleEdge(rayStart, v2, v3, hitPoint)) return false;
        if (!TestTriangleEdge(rayStart, v3, v1, hitPoint)) return false;

        return true;
    }

    //Edge test
    bool TestTriangleEdge(Vector3 rayStart, Vector3 v1, Vector3 v2, Vector3 hitPoint)
    {
        Vector3 vec1 = v1 - rayStart;
        Vector3 vec2 = v2 - rayStart;
        Vector3 n1 = Vector3.Cross(vec2, vec1).normalized;

        float d1 = Vector3.Dot(-rayStart, n1);

        if (Vector3.Dot(hitPoint, n1) + d1 < 0f) return false;

        return true;
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (triangles == null) return;

        for (int i = 0; i < triangles.Length; i++)
        {
            Vector3 v1 = transform.TransformPoint(triangles[i].v1);
            Vector3 v2 = transform.TransformPoint(triangles[i].v2);
            Vector3 v3 = transform.TransformPoint(triangles[i].v3);

            Vector3 N = transform.TransformDirection(triangles[i].N);


            Vector3 center = v1 + v2 + v3;
            center = center / 3;
            Gizmos.DrawLine(center, center + N);


        }


    }
}
