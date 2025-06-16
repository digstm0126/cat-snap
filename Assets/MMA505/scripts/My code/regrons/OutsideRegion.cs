using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace MMA505
{
    public class OutsideRegion : Region
    {

        #region Initialisation & Destruction

        private void Awake()
        {
            RegionManager.UpdateVisablEx += poplulatItemsEx;
            RegionManager.UpdatePortalEx += poplulatPortalsEx;
        }

        private void OnDestroy()
        {
            RegionManager.UpdateVisablEx -= poplulatItemsEx;
            RegionManager.UpdatePortalEx -= poplulatPortalsEx;

        }

        #endregion

        private void poplulatItemsEx(GameObject[] players, GameObject[] cats, GameObject[] regions)
        {

            List<GameObject> visiblePlayers = new List<GameObject>(players);
            List<GameObject> visibleCats = new List<GameObject>(cats);

            foreach (GameObject region in regions)
            {
                if (region != null)
                {
                    foreach (GameObject cat in region.GetComponent<Region>().getVisibGameObjects())
                    {
                        //if region contanse cat
                        if (visibleCats.Contains(cat))
                        {
                            visibleCats.Remove(cat);   
                        }
                    }

                    foreach (GameObject obj in region.GetComponent<Region>().getVisibGameObjects())
                    {
                        //if region contanse play checks all items but if play is player kinda messy oh whell very messy
                        if (visiblePlayers.Contains(obj))
                        {
                            visiblePlayers.Remove(obj);
                        }
                    }
                }
            }

            populatObjectsToTest(visiblePlayers.ToArray(), visibleCats.ToArray());

        }

        private void poplulatPortalsEx(GameObject[] portalsIn, GameObject[] regions)
        {
            List<GameObject> portals = new List<GameObject>(portalsIn);

            if (regions != null)
            {
                foreach (GameObject region in regions)
                {
                    if (portals != null)
                    {
                        // Create a temporary list of portals to remove
                        List<GameObject> portalsToRemove = new List<GameObject>();
                        
                        foreach (GameObject portal in portals)
                        {
                            GameObject[] regionsPortals = region.GetComponent<Region>().getPortals();
                            if (regionsPortals != null)
                            {
                                if (portal != null && regionsPortals.Contains(portal))
                                {
                                    portalsToRemove.Add(portal);
                                }
                            }
                        }

                        // Remove all marked portals after the foreach loop
                        foreach (GameObject portal in portalsToRemove)
                        {
                            portals.Remove(portal);
                        }
                    }
                }
            }

            popluatePortals(portals.ToArray());

        }

        
        

    }
}