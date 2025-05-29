using System;
using UnityEngine;

namespace MMA505
{
    public static class RegionManager
    {

        public static event Action<GameObject[], GameObject[], GameObject[]> UpdateVisablEx;
        public static event Action<GameObject[], GameObject[]> UpdateVisablIn;


        public static void UpdateVisabl(GameObject[] Players, GameObject[] cats, GameObject[] regionsIn)
        {

            UpdateVisablIn?.Invoke(Players, cats);
            UpdateVisablEx?.Invoke(Players, cats, regionsIn);


        }

    }
    
}
