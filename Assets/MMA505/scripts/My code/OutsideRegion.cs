using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MMA505
{
    public class OutsideRegion : Region
    {

        #region Initialisation & Destruction

        private void Awake()
        {
            RegionManager.UpdateVisablEx += poplulatItems;
            RegionManager.UpdatePortalEx += poplulatPortalsEx;
        }

        private void OnDestroy()
        {
            RegionManager.UpdateVisablEx -= poplulatItems;
            RegionManager.UpdatePortalEx -= poplulatPortalsEx;

        }

        #endregion

        private void poplulatItems(GameObject[] players, GameObject[] cats, GameObject[] regions)
        {
            List<GameObject> visiblePlayers = new List<GameObject>(players);
            List<GameObject> visibleCats = new List<GameObject>(cats);

            foreach (GameObject obj in regions)
            {
                if (obj != null)
                {
                    if (visibleCats != null && visibleCats.Contains(obj))
                    {
                        visibleCats.Remove(obj);
                    }
                    if (visiblePlayers != null && visiblePlayers.Contains(obj))
                    {
                        visiblePlayers.Remove(obj);
                    }
                }
            }

            populatObjectsToTest(players.ToArray(), cats.ToArray());

        }

        private void poplulatPortalsEx(GameObject[] portals, GameObject[] regions)
        {
            List<GameObject> Portals = new List<GameObject>(portals);

            foreach (GameObject obj in regions)
            {
                if (obj != null)
                {
                    if (Portals != null && Portals.Contains(obj))
                    {
                        Portals.Remove(obj);
                    }
                }
            }

            popluatePortals(portals.ToArray());

        }

        
        

    }
}