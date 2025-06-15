using Unity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Unity.Mathematics;


namespace MMA505
{
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

            
        }

        public void UpdateState(List<GameObject> objs)
        {
            _inSite = false;
            foreach (var obj in objs)
            {
                if (obj == _obj)
                {
                    _inSite = true;
                }
            }

            if (_inSite)
            {
                _intristVal = math.clamp(_intristVal + 1, 0 , 10);
                _lastLocation = _obj.transform.position;
            }
            else
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