using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]

public class ThirdPersonSittingBehaviour : ThirdPersonControllerAbstractBehaviour
{
    Transform sitAnchorTransform = null;

    public float minDistanceToSittableObject = 0.3f;
    public float minRotationToSittableObject = 0.2f;

    public enum SittingState
    {
        NotSitting,
        PreparingToSit,
        Sitting
    };
    public SittingState sittingState = SittingState.NotSitting;

    public override bool ExecuteBehaviourIfRequired(ref string CurrentState)
    {
        if (sittingState == SittingState.PreparingToSit)
        {
            AnimatePreparingToSit();
            CurrentState = sittingState.ToString();
            return true;
        }
        else if (sittingState == SittingState.Sitting)
        {
            //Do Nothing
            CurrentState = sittingState.ToString();
            return true;
        }

        return false;
    }

    public void ToggleSitting(Transform _sitAnchorTransform)
    {
        //If we were sitting or preparing to sit already, stop
        if (sittingState == SittingState.Sitting || sittingState == SittingState.PreparingToSit)
        {
            ThirdPersonController.Instance.animatorController.SetBool("Sit", false);
            sittingState = SittingState.NotSitting;

        }

        //Otherwise, prepare to sit
        else
        {
            sittingState = SittingState.PreparingToSit;
            sitAnchorTransform = _sitAnchorTransform;
        }
    }

    void AnimatePreparingToSit()
    {
        Vector3 characterObjectVector = sitAnchorTransform.position - transform.position;

        //If we're beyond the min distance, walk there
        if (Vector3.Magnitude(characterObjectVector) > minDistanceToSittableObject)
        {
            //Walk to Chair
            ThirdPersonController.Instance.animatorController.SetBool("isWalking", true);
            ThirdPersonController.Instance.animatorController.SetFloat("WalkSpeedMultiplier", 2);

            characterObjectVector = sitAnchorTransform.position - transform.position;
            Quaternion lookDirection = Quaternion.LookRotation(characterObjectVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, 0.05f);
        }

        //Otherwise if our facing angle is too different from the anchor facing angle, rotate
        else if (Quaternion.Angle(transform.rotation, sitAnchorTransform.rotation) > minRotationToSittableObject)
        {
            //Face Chair Direction
            ThirdPersonController.Instance.animatorController.SetBool("isWalking", false);
            ThirdPersonController.Instance.animatorController.SetFloat("WalkSpeedMultiplier", 0);

            transform.position = sitAnchorTransform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, sitAnchorTransform.rotation, 360 * Time.deltaTime);
        }

        //Otherwise, sit
        else
        {

            transform.position = sitAnchorTransform.position;
            transform.rotation = sitAnchorTransform.rotation;

            ThirdPersonController.Instance.animatorController.SetBool("Sit", true);
            sittingState = SittingState.Sitting;
        }

    }
}
