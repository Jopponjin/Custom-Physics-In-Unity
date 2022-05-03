using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics
{
    public class Box : MonoBehaviour
    {
        //Collison info
        public Vector3 A; //Furthest point A into B
        public Vector3 B; //Furthest point B into A
        public Vector3 Normal; //B - A normalizedpoint B into A
        public bool hasCollision;

        [HideInInspector]
        public SphereCollider sphereCollider;
        [HideInInspector]
        public BoxCollider boxCollider;

        public Collider objectCollider;

        // Position of top-left corner 
        public float X;
        public float Y;
        public float Z;
        [Space]
        // Dimensions 
        public float w;
        public float h;
        public float d;
        public Vector3 center;
        [Space]
        // Velocity 
        public float vx;
        public float vy;
        public float vz;
        [Space]
        // Properties
        public float Bounciness = 0f;


        public Box(float height, float width, float depth)
        {
            h = height;
            w = width;
            d = depth;
        }

        private void Awake()
        {
            //Set Box dimesions
            w = gameObject.transform.localScale.x;
            h = gameObject.transform.localScale.y;
            d = gameObject.transform.localScale.z;

            //Set Box Transform
            X = gameObject.transform.position.x;
            Y = gameObject.transform.position.y;
            Z = gameObject.transform.position.z;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, transform.localScale * 1.001f);
        }
    }
}


