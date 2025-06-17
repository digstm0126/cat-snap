using Unity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Unity.Mathematics;


namespace MMA505
{
    [System.Serializable]
    public class Intrist
    {
        [SerializeField] private GameObject _obj;
        [SerializeField] private int _intristVal; // 0-10
        [SerializeField] private int _fearVal; //from 0-10
        [SerializeField] private Vector3 _lastLocation;
        [SerializeField] private bool _inSite;




        public Intrist(GameObject obj, int intristVal, int fareVal)
        {

            _obj = obj;
            _intristVal = intristVal;
            _fearVal = fareVal;
            _lastLocation = _obj.transform.position;


        }

        public void UpdateStateFast(List<GameObject> objs)
        {
            _inSite = false;
            foreach (var obj in objs)
            {
                if (obj == _obj)
                {
                    _inSite = true;
                }
            }

            //if insite update location
            if (_inSite)
            {
                _lastLocation = _obj.transform.position;
            }

            //incress fare if its moving
            Rigidbody rb = _obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log($"manatued: {rb.linearVelocity.magnitude - 1}");
                _fearVal = math.clamp(_fearVal + (int)(rb.linearVelocity.magnitude - 1), 2, 10);
            }

        }

        public void UpdateStateSlow()
        {
            // in site intrit up
            if (_inSite)
            {
                _intristVal = math.clamp(_intristVal + 1, 0, 4);
            }
            else //fare down if not in site
            {
                _fearVal = math.clamp(_fearVal - 1, 0, 10);
            }
        }



        public void ResetFare(int fareVal)
        {
            _fearVal = fareVal;
        }

        public void SetIntrist(int intristVal)
        {
            _intristVal = math.clamp(intristVal, 0, 10);
        }

        public int GetIntristVal()
        {
            return _intristVal;
        }
        public int GetFearValue()
        {
            return _fearVal;
        }

        public Vector3 GetLastLocation()
        {
            return _lastLocation;
        }

        public GameObject GetGameObject()
        {
            return _obj;
        }

        public bool GetInsite()
        {
            return _inSite;
        }

    }
}