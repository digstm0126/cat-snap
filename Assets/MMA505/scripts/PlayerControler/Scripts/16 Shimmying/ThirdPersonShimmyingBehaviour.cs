using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]

public class ThirdPersonShimmyingBehaviour : ThirdPersonHangingBehaviour
{
    protected class ShimmyingState: HangingState
    {
        public const int Shimmying = 3;

        public new static string ToString(int hangingState)
        {
            switch (hangingState)
            {
                case NotHanging:
                    return "Not Hanging";

                case Hanging:
                    return "Hanging";

                case Dropping:
                    return "Dropping";

                case Shimmying:
                    return "Shimmying";

                default:
                    return "Unknown";
            }
        }

    };

    public override bool ExecuteBehaviourIfRequired(ref string CurrentState)
    {
        if (hangingState == ShimmyingState.Shimmying)
        {
            ExecuteShimmyState();

            CurrentState = ShimmyingState.ToString(hangingState);
            return true;
        }
        else if (hangingState == HangingState.Hanging)
        {
            ExecuteHangingState();

            CurrentState = ShimmyingState.ToString(hangingState);
            return true;
        }

        return base.ExecuteBehaviourIfRequired(ref CurrentState);
    }

    void ExecuteShimmyState()
    {
        bool shimmyLeft = Input.GetAxis("Horizontal") < 0;
        bool shimmyRight = Input.GetAxis("Horizontal") > 0;

        ThirdPersonController.Instance.animatorController.SetBool("ShimmyLeft", shimmyLeft);
        ThirdPersonController.Instance.animatorController.SetBool("ShimmyRight", shimmyRight);
        if (shimmyLeft || shimmyRight)
            hangingState = ShimmyingState.Shimmying;
        else if (hangingState == ShimmyingState.Shimmying)
        {
            UpdateHangableAnchor();
            hangingState = ShimmyingState.Hanging;
        }
    }

    protected override void ExecuteHangingState()
    {
        base.ExecuteHangingState();
        ExecuteShimmyState();    
    }

    public override void Drop()
    {
        if (hangingState == HangingState.Hanging || hangingState == ShimmyingState.Shimmying)
        {
            ThirdPersonController.Instance.animatorController.SetBool("ShimmyLeft", false);
            ThirdPersonController.Instance.animatorController.SetBool("ShimmyRight", false);

            ThirdPersonController.Instance.animatorController.SetTrigger("Drop");
            ThirdPersonController.Instance.GetComponent<Rigidbody>().isKinematic = false;
            hangAnchorTransform = null;
            hangingState = HangingState.Dropping;
        }
    }


    protected override void LerpToAnchor()
    {
        if (!hangAnchorTransform) return;
        if (hangingState == ShimmyingState.Shimmying) return;

        if (Vector3.Distance(this.transform.position, hangAnchorTransform.position) > minLerpDistanceToObject)
        {
            this.transform.rotation = Quaternion.Lerp(transform.rotation, hangAnchorTransform.rotation, Time.deltaTime * lerpSpeed);
            this.transform.position = Vector3.Lerp(transform.position, hangAnchorTransform.position, Time.deltaTime * lerpSpeed);
        }
        else
        {
            this.transform.rotation = hangAnchorTransform.rotation;
            this.transform.position = hangAnchorTransform.position;
        }
    }

    public void EndShimmy()
    {
        if (hangingState == ShimmyingState.Shimmying)
            hangingState = HangingState.Hanging;

        if (!hangAnchorTransform) return;

        UpdateHangableAnchor();
        this.transform.rotation = hangAnchorTransform.rotation;
        this.transform.position = hangAnchorTransform.position;
    }

    void Dropped()
    {
        hangingState = HangingState.NotHanging;
    }

    private void FixedUpdate()
    {
        //LerpToAnchor();
    }

}
