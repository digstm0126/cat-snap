using UnityEngine;

namespace MMA505
{

    public class InsideRegion : Region
    {
        #region Initialisation & Destruction

        private void Awake()
        {
            RegionManager.UpdateVisablIn += populatObjectsToTest;
        }

        private void OnDestroy()
        {
            RegionManager.UpdateVisablIn += populatObjectsToTest;
        }

        #endregion
        

    }
}
    
