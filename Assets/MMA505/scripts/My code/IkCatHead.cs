using UnityEngine;
using System;
using System.Collections;
using MMA505;

namespace MMA505
{
    public class IkCatHead : MonoBehaviour
    {
        [SerializeField] private Animator _animatorController;
        [SerializeField] private bool _applyIK;
        [SerializeField] private Vector3 _target;
        [SerializeField] private Transform _headBone;
        [SerializeField] private float rotationSpeed = 5f;
        private Quaternion _initialRotation;

        private void Start()
        {
            if (_headBone != null)
            {
                _initialRotation = _headBone.localRotation;
            }
        }

        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// It is called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            if (_animatorController || _headBone || _applyIK)
            {
                _headBone.transform.LookAt(_target);

            }
            else
            {
                _headBone.transform.rotation = _initialRotation;
            }

            
        }

        public void SwapIK(bool state)
        {
           
            _applyIK = true;
        
        }

        public void SetTarget(Vector3 target)
        {
            _target = target;
        }
    }
}
