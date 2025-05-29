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
        }
        
        private void OnDestroy()
        {
            RegionManager.UpdateVisablEx -= poplulatItems;
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
    }
}