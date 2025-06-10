using Unity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;


namespace MMA505
{
    public class Intrist
    {
        [SerializeField] private GameObject _obj;
        [SerializeField] private CatControler _catIntristed;
        [SerializeField] private int _intristVal; // 0-10
        [SerializeField] private int _fearVal; //from 0-10
        [SerializeField] private Vector3 _lastLocation;


        public Intrist(GameObject obj, CatControler cat)
        {

            _obj = obj;
            _intristVal = 5;
            _catIntristed = cat;

        }

        public void UpdateState()
        {
            bool isTarget = this == _catIntristed.GetTarget();
            int bravery = _catIntristed.GetComponent<CatControler>().GetBravery();

            //if obj is moving faster then bravery then far gose up down if not moving fast
            Rigidbody rb = _obj.GetComponent<Rigidbody>();
            if (!rb || rb.linearVelocity.magnitude >= bravery)
            {
                _fearVal = Mathf.Clamp(_fearVal + Mathf.RoundToInt(rb.linearVelocity.magnitude), 0, 10);
                Debug.Log($"fare: {_fearVal}");
                Debug.Log($"mag: {Mathf.RoundToInt(rb.linearVelocity.magnitude)} ");
            }
            else
            {
                _fearVal = Mathf.Clamp(_fearVal - 1, 0, 10);
            }

            //if the target intrist gose down and if not gose up
            if (isTarget)
            {
                _intristVal = Mathf.Clamp(_intristVal - 1, 0, 10);
            }
            else
            {
                _intristVal = Mathf.Clamp(_intristVal + 1, 0, 10);
            }

            _lastLocation = _obj.transform.position;
        }

        public void ResetFare()
        {
            _fearVal = 5;
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

    }
}