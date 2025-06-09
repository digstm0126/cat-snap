using UnityEngine;
using System;
using System.Collections;
using MMA505;

namespace MMA505
{

    [RequireComponent(typeof(Animator))]
    public class IkCatHead : MonoBehaviour
    {
        [SerializeField] private Animator _animatorController;
        [SerializeField] private bool _applyIK;
        [SerializeField] private Vector3 _target;
       


        private void OnAnimatorIK()
        {
            //ik hands
            if (_animatorController)
            {
                if (_applyIK)
                {
                    // Set the look target position, if one has been assigned
                    if (_target != null)
                    {
                        _animatorController.SetLookAtWeight(1);
                        _animatorController.SetLookAtPosition(_target);
                    }

                }
                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    _animatorController.SetLookAtWeight(0);
                }
            }
        }

        public void SwapIK(bool state)
        {
           
            _applyIK = state;
        
        }

        public void SetTarget(Vector3 target)
        {
            _target = target;
        }
    }
}
