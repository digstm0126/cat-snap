using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMA505
{

    [Serializable]
    public class Hearing : MonoBehaviour
    {
        //this script colects all of the game objects that can be heard

        [SerializeField] private GameManager _gameManager;
        [SerializeField] private GameObject _currentRegion; //current regrion
        [SerializeField] private List<GameObject> _gameObjectsInSound = new List<GameObject>();
        [SerializeField] private float _hearingRadius = 10f;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateVisableObjects();
        }


        public void UpdateVisableObjects()
        {
            // Clear the list of game objects in the frustum
            _gameObjectsInSound.Clear();


            findCurrentRegion(_gameManager.getAllRegions());


            // If the camera is inside a region
            if (_currentRegion != null)
            {
                // Determine the visible objects in that region based on the camera frustum
                List<GameObject> hearableObjects = FindHearableObjects(_currentRegion.GetComponent<Region>());

                if (hearableObjects != null)
                {
                    _gameObjectsInSound = hearableObjects;
                }
            }
        }

        public void findCurrentRegion(GameObject[] regionsIn)
        {

            // Calculate what region the camera is in
            _currentRegion = null;

            // Loop through each region reset
            foreach (GameObject region in regionsIn)
            {
                Region objRegion = region.GetComponent<Region>();


                // camera region
                if (objRegion.containsPoint(transform.position))
                {
                    _currentRegion = region;
                    break;
                }
            }
        }

        // Find visible objects in a region, based on a frustum and all portal frustums
        List<GameObject> FindHearableObjects(Region region)
        {
            // Get the list of potentially visible objects in the region
            // (i.e. all the objects in the region which are tested for visibility)
            List<GameObject> potentiallyVisibleObjects = new List<GameObject>();
            potentiallyVisibleObjects.AddRange(region.getVisibGameObjects());

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

                    if (Vector3.Distance(spherePos, transform.position) >= _hearingRadius)
                        visibleObjects.Add(visibleObject);
                }

            }

            // return the list of visible objects in this region, and all
            // other portal linked regions
            return visibleObjects;
        }
        
        private void OnDrawGizmos()
        {
            // Set the color for the hearing radius visualization
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f); // Semi-transparent green
            
            // Draw a wire sphere for the hearing radius
            Gizmos.DrawWireSphere(transform.position, _hearingRadius);
            
            // Draw a solid sphere with transparency
            Gizmos.color = new Color(0f, 1f, 0f, 0.05f); // Very transparent green
            Gizmos.DrawSphere(transform.position, _hearingRadius);
        }
        
    }
}
