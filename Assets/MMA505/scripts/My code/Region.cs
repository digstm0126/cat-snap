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
        // An array of visibility spheres for these game objects
        [SerializeField] private GameObject[] visibGameObjects;

        // An array of portals leading out of this region
        [SerializeField] private Portal[] portals;


    
        public GameObject[] getVisibGameObjects()
        {
            return visibGameObjects;
        }

        public Portal[] getPortals()
        {
            return portals;
        }

        public void popluatePortals(GameObject[] portalsObj)
        {
            List<Portal> portalsNew = new List<Portal>();

            // Add their renderers to the list
            foreach (GameObject obj in portalsObj)
            {
                Portal portal = obj.GetComponent<Portal>();
                if (portal != null && containsPoint(obj.transform.position))
                {
                    portalsNew.Add(portal);
                }
            }

            portals = portalsNew.ToArray();
        }

        public void populatObjectsToTest(GameObject[] players, GameObject[] cats)
        {
            List<GameObject> objToTest = new List<GameObject>();

            // adds all players to list
            foreach (GameObject obj in players)
            {
                if (containsPoint(obj.transform.position))
                {
                    objToTest.Add(obj);
                }
            }

            // adds all cats to list
            foreach (GameObject obj in cats)
            {
                if (containsPoint(obj.transform.position))
                {
                    objToTest.Add(obj);
                }
            }

            // Return the array of renderers
            visibGameObjects = objToTest.ToArray();
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