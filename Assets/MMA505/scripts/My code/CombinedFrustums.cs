using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MMA505;
using System;

/* This class implements portal culling
 * 
 * PROD321 - Interactive Computer Graphics and Animation 
 * Copyright 2021, University of Canterbury
 * Written by Adrian Clark
 */

namespace MMA505
{
    public class CombinedFrustums : MonoBehaviour
    {


        //game manager
        [SerializeField] private GameManager _GameManager;
        
        // The camera we will be generating portal frustums from
        [SerializeField] private Camera portalCamera;

        //camra region
        [SerializeField] private GameObject cameraRegion;

        //visable objects
        public List<GameObject> gameObjectsInFrustum = new List<GameObject>();

        // The frustum of our camera
        [SerializeField] private Frustum _CameraFrustum;


        // Start is called before the first frame update
        void Start()
        {
            // this and update are like this if i want to set them up with a manger latter.
            SetupSite(); 
        }


        void SetupSite()
        {

            _GameManager = FindObjectOfType<GameManager>();

            // If the occlusion camera hasn't been defined, try get the
            // camera tagged with MainCamera
            if (portalCamera == null)
            {
                portalCamera = Camera.main;
            }
            
            // Create a new GameObject for our camera frustum
            GameObject cameraFrustumGO = new GameObject("Camera Frustum");
            // Set it's layer to this layher
            cameraFrustumGO.layer = _GameManager.getFrustumLayer();
            // Set it's parent to this gameobject (so it updates with the camera)
            cameraFrustumGO.transform.SetParent(transform, false);
            // Create the frustum for this camera
            _CameraFrustum = Frustum.CreateFrustumFromCamera(portalCamera, cameraFrustumGO, _GameManager.getFrustumMaterial() , Color.blue);

        }



        // Update is called once per frame
        void Update()
        {
            updateVisableObjects();
        }



        //update visable objects
        public void updateVisableObjects()
        {
            // Clear the list of game objects in the frustum
            gameObjectsInFrustum.Clear();


            findCurrentRegion(_GameManager.getAllRegions());


            // If the camera is inside a region
            if (cameraRegion != null)
            {
                // Determine the visible objects in that region based on the camera frustum
                List<GameObject> visibleObjects = FindVisibleObjects(cameraRegion.GetComponent<Region>(), _CameraFrustum);

                if (visibleObjects != null)
                {
                    //check if it is in the frustom of any ocuding objects
                    gameObjectsInFrustum = FindGameObjectsNotOccluded(visibleObjects);
                }
            }
        }


        public void findCurrentRegion(GameObject[] regionsIn)
        {

            // Calculate what region the camera is in
            cameraRegion = null;

            // Loop through each region reset
            foreach (GameObject region in regionsIn)
            {
                Region objRegion = region.GetComponent<Region>();
                // Reset all the portals to hide their frustums
                objRegion.ResetPortals(this);

                // If the camera is inside a region, set that region as the
                // camera region
                if (objRegion.containsPoint(portalCamera.transform.position))
                {
                    cameraRegion = region;
                    break;
                }
            }
        }

        // finds and retrus gameObjectsNotOccluded
        public List<GameObject> FindGameObjectsNotOccluded(List<GameObject> visibilityGameObjects)
        {
            // make list of gameObjectsNotOccluded
            List<GameObject> gameObjectsNotOccluded = new List<GameObject>();

            // Create a list of visibile objects, and populate it will all objects
            // to start with (we will remove objects as we determine they are not
            // visible
            List<GameObject> visibleObjects = new List<GameObject>(visibilityGameObjects);

            // Loop through each occlusion frustum and
            foreach (GameObject occlusionObj in _GameManager.getOcludingObjects())
            {
                // Create lists for the frustum's plane's centers and normals
                List<Vector3> transformedFrustumCenters = new List<Vector3>();
                List<Vector3> transformedFrustumNormals = new List<Vector3>();

                // Get the frustum plane's centers and normals and store them in our lists
                occlusionObj.GetComponent<OcclusionFrustum>().CalcFrustum(portalCamera, ref transformedFrustumCenters, ref transformedFrustumNormals);

                float normalLength = 5;
                // Draw the normals for our occlusion frustum planes
                for (int j = 0; j < transformedFrustumCenters.Count; j++)
                    Debug.DrawRay(transformedFrustumCenters[j], transformedFrustumNormals[j] * normalLength, Color.white);

                // Loop through all our visible objects and dose a test for visablity
                for (int i = 0; i < visibleObjects.Count; i++)
                {
                    // Assume it's in the frustum by default
                    bool inFrustum = true;

                    // Get the position of this objects bounding sphere
                    Vector3 objPos;

                    //the bounds of the sphere
                    float bounds;

                    //fuckkkkkkk not to bad. each object should have the sphere atached witch can be found.
                    // Calculate the radius of the object's bounding sphere
                    Transform visablityShpere = visibleObjects[i].transform.Find("VisibilitySphere");
                    if (visablityShpere != null)
                    {
                        bounds = visablityShpere.localScale.x;
                        objPos = visablityShpere.position;

                        // Loop through all our transformed frustum planes
                        for (int j = 0; j < transformedFrustumCenters.Count; j++)
                        {
                            // Calculate the h distance of the point relative to each of the frustum planes
                            // the formula for h distance is h = (P - P_0) . N
                            // Where P is the point we are wanting to check what side of the plane it lays on
                            // P_0 is a point on the plane we're testing, and N is the planes normal
                            float h = Vector3.Dot((objPos - transformedFrustumCenters[j]), transformedFrustumNormals[j]);

                            // If the h value is greater than the outer bounds limit
                            if (h > -bounds)
                            {
                                // The object is at least *partly* out of the occlusion volume
                                // set "inFrustum" to false and break
                                inFrustum = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //chat gpt is nice for gening error stuff (if i set up my code to throw an error i can just ask ai to wright it.) so i dont have to and then ill aculy do this stuff.
                        throw new System.Exception($"VisibilitySphere not found for object: {visibleObjects[i].name}. Ensure each object has a child named 'VisibilitySphere' with appropriate settings.");
                    }



                    // Otherwise, if it was within all the bounds
                    if (inFrustum)
                    {
                        // It's completely in the frustum, remove it from
                        // the list of visible objects
                        visibleObjects.RemoveAt(i);

                        // Reduce i because we have removed this object from the list
                        i--;
                    }
                }
            }

            // // Loop through each game object we're testing
            // for (int i = 0; i < gameObjectsToTestForOcclusion.Length; i++)
            // {
            //     // Get its visibility sphere
            //     SphereCollider sphereCollider = visibilitySpheres[i].GetComponent<SphereCollider>();

            //     // Get the visibility sphere's mesh renderer
            //     MeshRenderer mr = sphereCollider.GetComponent<MeshRenderer>();

            //     // if the visiblity sphere is a visible object
            //     if (visibleObjects.Contains(sphereCollider))
            //     {
            //         // Add this object to the game objects in frustum list
            //         gameObjectsNotOccluded.Add(gameObjectsToTestForOcclusion[i].gameObject);
            //     }
            // }

            gameObjectsNotOccluded.AddRange(visibleObjects);
            return gameObjectsNotOccluded;
        }


       // Find visible objects in a region, based on a frustum and all portal frustums
       List<GameObject> FindVisibleObjects(Region region, Frustum frustum)
        {
            // Get the list of potentially visible objects in the region
            // (i.e. all the objects in the region which are tested for visibility)
            List<GameObject> potentiallyVisibleObjects = new List<GameObject>();
            potentiallyVisibleObjects.AddRange(region.getVisibGameObjects());

            // Update the frustum for this frame
            frustum.UpdateFrustum();

            // Loop through each of the portals in this region
            foreach (GameObject PortalOjt in region.getPortals())
            {
                Portal p = PortalOjt.GetComponent<Portal>();
                // If this portal is within this frustum
                if (frustum.containsPortal(p))
                    // Get all the visible objects in the region that portal
                    // links to, using the frustum created by that portal
                    // relevant to the camera
                    // Note: This is a recursive function, so will traverse room
                    // by room based on which portals are visible in the current
                    // frustum. The objects which are returned by this function
                    // will be visible in all regions below this one (i.e. they
                    // will be contained by every frustum below this one)
                    potentiallyVisibleObjects.AddRange(FindVisibleObjects(p.nextRegion, p.GetUpdatedFrustum(this, portalCamera)));
            }

            // Create a list of objects which are actually visible
            List<GameObject> visibleObjects = new List<GameObject>();

            if (potentiallyVisibleObjects != null)
            {

                // Loop through all our potentially visible objects
                foreach (GameObject visibleObject in potentiallyVisibleObjects)
                {

                    Transform visablityShpere = visibleObject.transform.Find("VisibilitySphere");

                    // Get the position of this objects bounding sphere
                    Vector3 spherePos = visablityShpere.position;

                    // Calculate the radius of the object's bounding sphere
                    float bounds = visablityShpere.localScale.x;
                    //float bounds = sphereCollider.radius * sphereCollider.transform.localScale.x;

                    // If the current frustum contains the object, it must be visible
                    // (any object in a region below this one has already been determined
                    // to be visible to all portal frustums below this)
                    if (frustum.containsPoint(spherePos, bounds))
                        visibleObjects.Add(visibleObject);
                }

            }

            // return the list of visible objects in this region, and all
                // other portal linked regions
                return visibleObjects;
        }

    }
}