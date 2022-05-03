using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics
{
    public class PhysicsWorld : MonoBehaviour
    {
        public GameObject boxPrefab;
        [Space]
        public Box boxStaticTest;
        public List<Box> boxColliders = new List<Box>();
        public List<Body> boxBodies = new List<Body>();
        


        private void Start()
        {
            FindBoxes();
        }

        void FindBoxes()
        {
            var foundBoxes= GameObject.FindGameObjectsWithTag("Box");

            for (int i = 0; i < foundBoxes.Length; i++)
            {
                if (foundBoxes[i].GetComponent<Box>())
                {
                    boxColliders.Add(foundBoxes[i].GetComponent<Box>());
                }
                if (foundBoxes[i].GetComponent<Body>())
                {
                    boxBodies.Add(foundBoxes[i].GetComponent<Body>());
                }
            }
        }

        

        private void FixedUpdate()
        {

            


            //for (int a = 0; a < boxColliders.Count; a++)
            //{
            //    for (int b = 0; b < boxColliders.Count; b++)
            //    {
                    

            //        Respons(boxColliders[a], boxStaticTest);
            //        boxColliders[a].transform.position = new Vector3(boxColliders[a].X, boxColliders[a].Y, boxColliders[a].Z); 
            //    }
            //}
        }

        public void UpdateForces(Vector3 force, Body bodyObject)
        {
            //Calculte body forces
            bodyObject.acceleration = force / bodyObject.mass;
            bodyObject.totalForce = bodyObject.acceleration * bodyObject.mass;
            if (bodyObject.useGravity) bodyObject.totalForce += bodyObject.gravity;


            bodyObject.velocity = bodyObject.velocity + bodyObject.totalForce * Time.deltaTime;

            bodyObject.transform.position = bodyObject.transform.position +
                                            bodyObject.velocity * Time.deltaTime;
        }

        void Respons(Body bodyA, Body bodyB, float maxDistance = 0.1f)
        {
            if (bodyA == bodyB) return;

            float distance = Vector3.Distance(bodyA.transform.position, bodyB.transform.position);

            if (distance > maxDistance)
            {

            }
            

            #region Old

            //float normalX, normalY, normalZ;
            //float collisionTime = SweptAABB(boxA, boxB, out normalX, out normalY, out normalZ);

            //boxA.X += boxA.vx * collisionTime;
            //boxA.Y += boxA.vy * collisionTime;
            //boxA.Z += boxA.vz * collisionTime;

            //float remainingtime = 1.0f - collisionTime;

            ////Slied on collision
            //if (remainingtime > 0.0f)
            //{
            //    float dotProduct = (boxA.vx * normalY + boxA.vy * normalX) * remainingtime;
            //    boxA.vx = dotProduct * normalY;
            //    boxA.vy = dotProduct * normalX;
            //    Debug.Log("remainingtime > 0.0f");

            //}

            #endregion
        }

        float SweptAABB(Box b1, Box b2, out float normalX, out float normalY, out float normalZ)
        {
            float distEntryX, distEntryY, distEntryZ;
            float exitX, exitY, exitZ;

            #region Object Distances XYZ

            // Finds the distance between the objects on the near and far sides for both x and y
            if (b1.vx > 0.0f)
            {
                distEntryX = b2.X - (b1.X + b1.w);
                exitX = (b2.X + b2.w) - b1.X;
            }
            else
            {
                distEntryX = (b2.X + b2.w) - b1.X;
                exitX = b2.X - (b1.X + b1.w);
            }

            if (b2.vx > 0.0f)
            {
                distEntryY = b2.Y - (b1.Y + b1.h);
                exitY = (b2.Y + b2.h) - b1.Y;
            }
            else
            {
                distEntryY = (b2.Y + b2.h) - b1.Y;
                exitY = b2.Y - (b1.Y + b1.h);
            }

            if (b1.vz > 0.0f)
            {
                distEntryZ = b2.Z - (b1.Z + b1.d);
                exitZ = (b2.Z + b2.d) - b1.Z;
            }
            else
            {
                distEntryZ = (b2.Z + b2.d) - b1.Z;
                exitZ = b2.Z - (b1.Z + b1.d);
            }
            #endregion

            #region Timing Collison And Exit

            float xEntry, yEntry, zEntry;
            float xExit, yExit, zExit;

            if (b1.vx == 0.0f)
            {
                xEntry = float.MinValue;
                xExit = float.MaxValue;
            }
            else
            {
                xEntry = distEntryX / b1.vx;
                xExit = exitX / b1.vx;
            }

            if (b1.vy == 0.0f)
            {
                yEntry = float.MinValue;
                yExit = float.MaxValue;
            }
            else
            {
                yEntry = distEntryY / b1.vy;
                yExit = exitY / b1.vy;
            }

            if (b1.vz == 0.0f)
            {
                zEntry = float.MinValue;
                zExit = float.MaxValue;
            }
            else
            {
                zEntry = distEntryZ / b1.vz;
                zExit = exitZ / b1.vz;
            }
            #endregion

            #region Finds earliest/leates Collision

            float entryTime = Mathf.Max(distEntryX, distEntryY, distEntryZ);
            float exitTime = Mathf.Max(exitX, exitY, exitZ);

            // If there was no collision
            if (
                entryTime > exitTime || (xEntry < 0.0f && yEntry < 0.0f) ||
                (xEntry < 0.0f && zEntry < 0.0f) || (yEntry < 0.0f && zEntry < 0.0f) ||
                xEntry > 1.0f || yEntry > 1.0f || zEntry > 1.0f
                )
            {
                normalX = 0.0f;
                normalY = 0.0f;
                normalZ = 0.0f;
                return 1.0f;
            }
            else // If there was a collision
            {
                // Calculate normal of collided surface

                if (xEntry > yEntry && xEntry > zEntry)
                {
                    if (distEntryX < 0.0f)
                    {
                        normalX = 1.0f;
                        normalY = 0.0f;
                        normalZ = 0.0f;
                    }
                    else
                    {
                        normalX = -1.0f;
                        normalY = 0.0f;
                        normalZ = 0.0f;
                    }
                }
                else if (yEntry > zEntry)
                {
                    if (distEntryY < 0.0f)
                    {
                        normalX = 0.0f;
                        normalY = 1.0f;
                        normalZ = 0.0f;
                    }
                    else
                    {
                        normalX = 0.0f;
                        normalY = -1.0f;
                        normalZ = 0.0f;
                    }
                }
                else
                {
                    if (distEntryZ < 0.0f)
                    {
                        normalX = 0.0f;
                        normalY = 1.0f;
                        normalZ = 0.0f;
                    }
                    else
                    {
                        normalX = 0.0f;
                        normalY = -1.0f;
                        normalZ = 0.0f;
                    }
                }
            }
            #endregion

            return entryTime;
        }

    }
}

