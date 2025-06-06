using UnityEngine;
using System.Collections.Generic;
using System;


namespace MMA505
{
    public class PlayerCamControler : MonoBehaviour
    {

        [SerializeField] private GameObject _PhotoCam;
        [SerializeField] private List<GameObject> _visableObjects = new List<GameObject>();
        [SerializeField] private List<(GameObject obj, float distance, string state)> _photoResults = new List<(GameObject, float, string)>();

        public void TakePhoto()
        {
            _visableObjects.Clear();
            _visableObjects.AddRange(_PhotoCam.GetComponent<CombinedFrustums>().GetVisableObjects());
            foreach (GameObject visableObj in _visableObjects)
            {
                string _state = "cool";
                float _dist = Vector3.Distance(visableObj.transform.position, _PhotoCam.transform.position);
                _photoResults.Add((visableObj, _dist, _state));
                Debug.Log(visableObj.name);
            }

        }
    }
}
