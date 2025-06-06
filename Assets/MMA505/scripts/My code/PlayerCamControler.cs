using UnityEngine;
using System.Collections.Generic;
using System;


namespace MMA505
{
    public class PlayerCamControler : MonoBehaviour
    {

        [SerializeField] private GameObject _PhotoCam;
        [SerializeField] private List<GameObject> _visableObjects = new List<GameObject>();
        public static event Action<Photo> OnPhotoTake;

        public void TakePhoto()
        {
            _visableObjects.Clear();
            _visableObjects.AddRange(_PhotoCam.GetComponent<CombinedFrustums>().GetVisableObjects());
            foreach (GameObject visableObj in _visableObjects)
            {
                CatControler.CatStates _state = visableObj.GetComponent<CatControler>().getState();
                int _dist = (int) Vector3.Distance(visableObj.transform.position, _PhotoCam.transform.position);
                Photo _newPhoto = new Photo(visableObj, _dist, _state);
                OnPhotoTake?.Invoke(_newPhoto);
            }
        }
    }
    public class Photo
    {
        [SerializeField] private GameObject _obj;
        [SerializeField] private int _dist;
        [SerializeField] private CatControler.CatStates _state;
        [SerializeField] private Photo _foundPhoto;

        public Photo(GameObject obj, int dist, CatControler.CatStates state)
        {
            _obj = obj;
            _dist = dist;
            _state = state;
        }

        public override string ToString()
        {
            return $"Cat: {_obj.name}\nLess then {_dist} m away\nWhile they are {_state}\n\n";
        }

        public GameObject GetObject()
        {
            return _obj;
        }

        public float GetDist()
        {
            return _dist;
        }

        public CatControler.CatStates GetState()
        {
            return _state;
        }

        public Photo GetFoundPhoto()
        {
            return _foundPhoto;
        }

        public void SetFoundPhoto(Photo photo)
        {
            _foundPhoto = photo;
        }
    }
}
