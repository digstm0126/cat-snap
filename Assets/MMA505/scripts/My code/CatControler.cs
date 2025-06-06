using System.Collections.Generic;
using MMA505;
using UnityEngine;

namespace MMA505
{
    public class CatControler : MonoBehaviour
    {
        [SerializeField] private CombinedFrustums _combinedFrustum;

        public enum CatStates { run, walk };
        [SerializeField] private CatStates _currentState = CatStates.walk;




        // Update is called once per frame
        void Update()
        {
            CheckForPlayer();
        }

        private void Move()
        {

        }

        private void CheckForPlayer()
        {
            List<GameObject> objects = _combinedFrustum.GetVisableObjects();
            foreach (GameObject obj in objects)
            {
                if (obj.tag == "Player")
                {
                    _currentState = CatStates.run;
                    Debug.Log("player");
                }
                _currentState = CatStates.walk;
            }
        }
    }
}
