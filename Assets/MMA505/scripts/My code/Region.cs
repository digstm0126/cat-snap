using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MMA505;
using UnityEngine.Pool;


/* This class represents a "region" for portal culling
 * A region is defined as a space which contains objects
 * to test for visibility, as well as portals which lead
 * out of the region into other regions. A region also
 * possibly contains the camera
 * 
 * PROD321 - Interactive Computer Graphics and Animation 
 * Copyright 2021, University of Canterbury
 * Written by Adrian Clark
 */

namespace MMA505
{
    public class Region : MonoBehaviour
    {
        // An array of game objects to test for visibility
        public GameObject[] gameObjectsToTestForVisibility;

        // An array of visibility spheres for these game objects
        [HideInInspector]
        public GameObject[] visibGameObjects;

        // An array of portals leading out of this region
        public Portal[] portals;

        // Get the portal culling script
        CombinedFrustums CombinedFrustums;


        // Start is called before the first frame update
        void Start()
        {
            //change to manager script
            // Get the portal culling script
            CombinedFrustums = FindObjectOfType<CombinedFrustums>();

            //popluate gameObjectsToTestForVisibility with all game objects inside of it
            gameObjectsToTestForVisibility = populatObjectsToTest();

            //poplulat portals
            portals = popluatePortals();

            // Instantiate the visibility spheres list based on the number of game
            // objects to test for visibility
            visibGameObjects = new GameObject[gameObjectsToTestForVisibility.Length];

            //each gameobject is 
            addVisibilitySphere();


        }

        //adda sphere site colider to each object in objToTest for visablity
        public void addVisibilitySphere()
        {
            
            // Loop through each game object to add a visablity sphere
            for (int i = 0; i < gameObjectsToTestForVisibility.Length; i++)
            {

                //dont need this part as i just wat refrince to the game object
                // If the game object already has a visibility sphere
                if (gameObjectsToTestForVisibility[i].transform.Find("VisibilitySphere") == null)
                {
                    // Create a new sphere
                    GameObject visibilitySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    // Set the gameobjects name to "VisibilitySphere"
                    visibilitySphere.name = "VisibilitySphere";
                    // Set it's parent as the game object
                    visibilitySphere.transform.SetParent(gameObjectsToTestForVisibility[i].transform, false);
                    // Set it's size to the the game objects bounding box's extent magnitude * 2
                    // (the magnitude is the half the size)
                    Renderer renderer = gameObjectsToTestForVisibility[i].GetComponent<Renderer>();
                    Vector3 boundsExtents = renderer.bounds.extents;
                    visibilitySphere.transform.localScale = Vector3.one * boundsExtents.magnitude * 2;
                    // Get the sphere's Renderer and update the material to our visibility sphere material, and turn off shadows
                    Renderer rendererSphere = visibilitySphere.GetComponent<Renderer>();
                    renderer.material = CombinedFrustums.visibilitySphereMaterial;
                    rendererSphere.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    rendererSphere.receiveShadows = false;
                }
            }
        }

        public Portal[] popluatePortals()
        {
            List<Portal> portals = new List<Portal>();

            GameObject[] portalsObj = GameObject.FindGameObjectsWithTag("portal");

            // Add their renderers to the list
            foreach (GameObject obj in portalsObj)
            {
                Portal portal = obj.GetComponent<Portal>();
                if (portal != null && containsPoint(obj.transform.position))
                {
                    portals.Add(portal);
                }
            }

            return portals.ToArray();
        }

        public GameObject[] populatObjectsToTest()
        {
            List<GameObject> objToTest = new List<GameObject>();

            // Find all game objects with the tag "Player" or "Cat"
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] catObjects = GameObject.FindGameObjectsWithTag("Cat");

            // adds all players to list
            foreach (GameObject obj in playerObjects)
            {

                if (containsPoint(obj.transform.position))
                {
                    objToTest.Add(obj);
                }
            }

            // adds all cats to list
            foreach (GameObject obj in catObjects)
            {
                if (containsPoint(obj.transform.position))
                {
                    objToTest.Add(obj);
                }
            }

            // Return the array of renderers
            return objToTest.ToArray();
        }

        // Returns true if this region contains a point
        public bool containsPoint(Vector3 worldPoint)
        {
            // Get the mesh filter of this region
            MeshFilter mf = GetComponent<MeshFilter>();
            // return true if the point transformed into model space is inside
            // the bounds of the mesh attached to the mesh filter
            return mf.mesh.bounds.Contains(transform.InverseTransformPoint(worldPoint));
        }

        //not useing visablity spheres as to many camras
        // Reset all our visibility spheres to not visible (red)
        // public void ResetVisibilitySpheres()
        // {
        //     // Loop through each visibility sphere
        //     foreach (SphereCollider sphereCollider in visibilitySpheres)
        //     {
        //         // Get it's mesh renderer
        //         MeshRenderer mr = sphereCollider.GetComponent<MeshRenderer>();
        //         // Set the materials colour to red
        //         mr.material.color = new Color(1, 0, 0, .5f);
        //     }
        // }

        // Reset all our portal frustums
        public void ResetPortals(CombinedFrustums CombinedFrustums)
        {
            // Loop through all the portals
            foreach (Portal p in portals)
                // Hide the frustum
                p.HideFrustum(CombinedFrustums);
        }
    }
}