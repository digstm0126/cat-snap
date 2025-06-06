using UnityEngine;
using System;
using System.Collections;
using MMA505;

namespace MMA505
{

    [RequireComponent(typeof(Animator))]
    public class IkHandCam : MonoBehaviour
    {
        [SerializeField] private Animator _animatorController;
        [SerializeField] private bool _applyIK;
        [SerializeField] private GameObject _PhotoCam;
        [SerializeField] private GameObject _CamRgihtHand;


        private void OnAnimatorIK()
        {
            //ik hands
            if (_animatorController)
            {
                if (_applyIK)
                {
                    // Set the look target position, if one has been assigned
                    if (_PhotoCam != null)
                    {
                        _animatorController.SetLookAtWeight(1);
                        _animatorController.SetLookAtPosition(_PhotoCam.transform.position);
                    }

                    // Set the right hand target position and rotation, if one has been assigned
                    if (_CamRgihtHand != null)
                    {
                        _animatorController.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        _animatorController.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        _animatorController.SetIKPosition(AvatarIKGoal.RightHand, _CamRgihtHand.transform.position);
                        _animatorController.SetIKRotation(AvatarIKGoal.RightHand, _CamRgihtHand.transform.rotation);
                    }
                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    _animatorController.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    _animatorController.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    _animatorController.SetLookAtWeight(0);
                }
            }
        }

        public void SwapIK()
        {
            if (_applyIK == true)
            {
                _applyIK = false;
            }
            else
            {
                _applyIK = true;
            }
        }
    }
}
